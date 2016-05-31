using UnityEngine;
using System.IO; //needed for the StringReader class
using GridFramework.Grids;
using GridFramework.Renderers.Rectangular;

namespace GridFramework.Examples.TerrainMesh {
	[RequireComponent(typeof(RectGrid))]
	[RequireComponent(typeof(Parallelepiped))]
	public class TerrainMeshBuilder : MonoBehaviour {
#region  Public variables
		/// <summary>
		///   Plain text file with (preferably) integer numbers for the height.
		/// </summary>
		public TextAsset _heightMap;

		/// <summary>
		///   Used for the rendering the plane.
		/// </summary>
		public Material _planeMaterial;

		/// <summary>
		///   Used for the wire frame rendering.
		/// </summary>
		public Material _lineMaterial;
#endregion  // Public variables
		
#region  Private variables
		private string heightString;

		/// <summary>
		///   Amount of rows (X-direction towards positive).
		/// </summary>
		private int rows;

		/// <summary>
		///   Amount of columns (Z-direction towards negative).
		/// </summary>
		private int columns;

		/// <summary>
		///   After the heights have been read form the height map store them
		///   in this integer array.
		/// </summary>
		private int[] heights;
		
		/// <summary>
		///   The mesh used for terrain, we will be referencing it often.
		/// </summary>
		private Mesh _mesh;
	
		private MeshFilter _mf;
		private MeshCollider _mc;
		private MeshRenderer _mr;
		private RectGrid _grid;
#endregion  // Private variables
	
#region  Callback methods
		void Awake() {
			_mf   = gameObject.AddComponent<MeshFilter>();
			_mc   = gameObject.AddComponent<MeshCollider>();
			_mr   = gameObject.AddComponent<MeshRenderer>();
			_grid = GetComponent<RectGrid>();
			
			// Create the mesh, then assign it to the components that need it
			// and show the height map in the GUI.
			BuildMesh();
			AssignMesh();
			UpdateHeightString();

			// Disable the renderer, looks better that way.
			var para = GetComponent<Parallelepiped>();
			para.enabled = false;
		}

		void OnGUI() {
			const int x = 10, y = 10, w = 250, h = 200;
			var rect = new Rect (x, y, w, h);
			GUI.TextArea(rect, heightString);
		}
		
		void OnMouseOver() {
			// The index of the nearest vertex we are above (in a real game you
			// would not want to call this every frame)
			var index      = HandleMouseInput();
			var leftClick  = Input.GetMouseButtonDown(0);
			var rightClick = Input.GetMouseButtonDown(1);

			if (leftClick) { // when the player left-clicks
				RaiseVertex(index, 1);
			} else if (rightClick) { // when the player right-clicks
				RaiseVertex(index, -1);
			}
		}

		void OnRenderObject() {
			// This is not necessarily the best way to achieve wireframe
			// rendering, but it gets the job done. For a real game you should
			// look for a better solution.
			_planeMaterial.SetPass(0);
			GL.PushMatrix();
			GL.MultMatrix(transform.localToWorldMatrix); 
			GL.Begin(GL.LINES);
			GL.Color(Color.black);
			
			var triCount = _mesh.triangles.Length / 3;
			for (var i = 0; i < triCount; ++i) {
				GL.Vertex(_mesh.vertices[_mesh.triangles[i * 3 + 0]]);
				GL.Vertex(_mesh.vertices[_mesh.triangles[i * 3 + 1]]);
				
				GL.Vertex(_mesh.vertices[_mesh.triangles[i * 3 + 1]]);
				GL.Vertex(_mesh.vertices[_mesh.triangles[i * 3 + 2]]);
				
				GL.Vertex(_mesh.vertices[_mesh.triangles[i * 3 + 2]]);
				GL.Vertex(_mesh.vertices[_mesh.triangles[i * 3 + 0]]);
			}
			
			GL.End();
			GL.PopMatrix();
		}
#endregion  // Callback methods
		
#region  Private methods
		private void BuildMesh() {
			var sr = new StringReader(_heightMap.text);
			var text = StringReaderToStrig(sr);
			
			_mesh = new Mesh();
			_mesh.Clear();
			
			heights = new int[rows * columns];
			
			for (var i = 0; i < text.Length; ++i) {
				heights[i] = (int)char.GetNumericValue(text[i]);
			}
			
			var vertices  = AssignVertices(_mesh);
			AssignTriangles(_mesh);
			AssignUVs(_mesh, vertices);
			
			CleanupMesh(_mesh);
		}
		
		private void AssignMesh() {
			_mf.mesh = _mesh;
			_mc.sharedMesh = _mesh;
			// Add a nice transparent shader to the renderer.
			_mr.sharedMaterial = _lineMaterial;
		}
	
		/// <summary>
		//    Handle mouse input by casting a ray from the cursor through the
		//    camera at the mesh collider and returning the index of the
		//    nearest vertex.
		/// </summary>
		private int HandleMouseInput() {
			var input = Input.mousePosition;
			RaycastHit hit;
			_mc.Raycast(Camera.main.ScreenPointToRay(input), out hit, Mathf.Infinity);

			var gridPoint = _grid.WorldToGrid(hit.point);
			var i = Mathf.RoundToInt(-gridPoint.z) + 1;
			var j = Mathf.RoundToInt(gridPoint.x) + 1;

			return RowColumn2Index(i, j);
		}

		private Vector3[] AssignVertices(Mesh mesh) {
			var vertices = new Vector3[rows * columns];
			
			for (var i = 1; i <= rows; i++){
				for (var j = 1; j <= columns; j++) {
					// Pick the height from text. The row and column get
					// translated into the index within the long text string.
					var index = RowColumn2Index(i, j);
					var h = heights[index];
					// Now assign the vertex depending on its row, column and
					// height (vector in local space; row, column and height in
					// grid space)
					var  gridVertex = new Vector3(j - 1, h, -i + 1);
					var worldVertex = _grid.GridToWorld(gridVertex);
					var localVertex = transform.InverseTransformPoint(worldVertex);
					vertices[index] = localVertex;
				}
			}

			// Assign the vertices to the mesh
			mesh.vertices = vertices;
			return vertices;
		}

		private int[] AssignTriangles(Mesh mesh) {
			// 3 vertices per triangle, two triangles per square, one square
			// between two columns/rows.
			var amount = 3 * 2 * (rows - 1) * (columns - 1);
			var triangles = new int[amount];
			var counter = 0; // this will keep track of where in the triangles array we currently are
			
			for (var i = 1; i < rows; i++) {
				for (var j = 1; j < columns; j++){
					// assign the vertex indices in a clockwise direction
					triangles[counter + 0] = RowColumn2Index(i + 0, j + 0);
					triangles[counter + 1] = RowColumn2Index(i + 0, j + 1);
					triangles[counter + 2] = RowColumn2Index(i + 1, j + 0);
					triangles[counter + 3] = RowColumn2Index(i + 1, j + 0);
					triangles[counter + 4] = RowColumn2Index(i + 0, j + 1);
					triangles[counter + 5] = RowColumn2Index(i + 1, j + 1);
					counter += 6; // increment the counter for the next two triangles (six vertices)
				}
			}

			// Assign the triangles to the mesh
			mesh.triangles = triangles;
			return triangles;
		}

		private Vector2[] AssignUVs(Mesh mesh, Vector3[] vertices) {
			// Add some dummy UVs to keep the shader happy or else it
			// complains, but they are not used in this example.
			var uvs = new Vector2[vertices.Length];
	        for (var k = 0; k < uvs.Length; ++k) {
	            uvs[k] = new Vector2(vertices[k].x, vertices[k].y);
	        }
	        mesh.uv = uvs;
	        return uvs;
		}

		private void CleanupMesh(Mesh mesh) {
			mesh.RecalculateNormals();
			mesh.RecalculateBounds();
			mesh.Optimize();
		}
		
		/// <summary>
		///   Raise (or lower) a vertex with given index by a certain amount
		///   along the Y-axis.
		/// </summary>
		/// <param name="index">
		///   Index of the vertex into the array.
		/// </param>
		/// <param name="amount">
		///   By how much to raise the vertex, negative values lower the vertex.
		/// </param>
		private void RaiseVertex(int index, int amount) {
			// Extract the vertices of the mesh, then raise the vertex and
			// assign the vertices back.
			var vertices = _mesh.vertices;
			var up = amount * _grid.Up;
			vertices[index] += up;
			_mesh.vertices = vertices;
	
			CleanupMesh(_mesh);
	
			// To update the mesh collider remove its mesh and then re-assign
			// it again.
			_mc.sharedMesh = null;
			_mc.sharedMesh = _mesh;
			
			heights[index] += amount;
			UpdateHeightString();
		}
	
		/// <summary>
		///   Reads the heights from a string reader.
		/// </summary>
		/// <param name="tr">
		///   An initialized text reader.
		/// </param>
		/// <remarks>
		///   As a side effect the number of rows and colums is also set. The
		///   number of columns is the length of the first line, so all lines
		///   must have the same length.
		/// </remarks>
		private string StringReaderToStrig(TextReader tr) {
			var line = tr.ReadLine();
			var text = "";
			columns = line.Length;

			while (line != null) {
				text += line;
				++rows;
				line = tr.ReadLine();
			}
			return text;
		}
		
		private void UpdateHeightString() {
			const string boilerplate = "Left-click a vertex to raise it by one" +
				"one unit, right-click to lower it.\n\nHeight Map:\n";

			heightString = boilerplate;
			for (var i = 1; i <= rows; ++i) {
				for (var j = 1; j <= columns; ++j) {
					var index = RowColumn2Index(i, j);
					// Add the entries from the heights array
					heightString += " " + heights[index] + " ";
				}
				// Line break after reaching the end of the row
				heightString += "\n";
			}
		}

		/// <summary>
		///   Convert a row-column pair to an array index.
		/// </summary>
		/// <remarks>
		///   <para>
		///     The index is calculated by going through each row until the end
		///     and then jumping to the beginning of the next row (rows and
		///     columns start at 1).
		///   </para>
		/// </remarks>
		int RowColumn2Index(int row, int column) {
			return (row - 1) * columns + column - 1;
		}
#endregion  // Private methods
	}
}
