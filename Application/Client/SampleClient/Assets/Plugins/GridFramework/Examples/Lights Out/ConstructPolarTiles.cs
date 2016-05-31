using UnityEngine;
using GridFramework.Grids;
using GridFramework.Renderers.Polar;

// This script automates the task of placing tiles for the lights-out game
// manually. It will loop in a circular fashion through the grid and place
// tiles based on ther grid coordinates. Once a tile object has been pu it
// place the appropriate components are dded and set up.
// 
// For each tile we generate a custom mesh based on the tile's position in the
// grid. Every tile has a unique shape, but we can automate it as well by using
// the tile's grid coordinates to calculate the vertices and connect them to
// triangles. The same mesh is then used for collision as well.
// 
// This script demonstrates how to use Grid Framework both for object placement
// and for mesh generation. The generated meshed fit together nicely and appear
// as one seamless object, but they stay separate.

namespace GridFramework.Examples.LightsOut {
	[RequireComponent (typeof (PolarGrid))]
	[RequireComponent (typeof (Cylinder))]
	public class ConstructPolarTiles : MonoBehaviour {
#region  Private variables
		/// <summary>
		///   The grid used to place the tiles on.
		/// </summary>
		private PolarGrid _grid;

		/// <summary>
		///   The grid renderer used for the smoothness of tiles.
		/// </summary>
		private Cylinder _cylinder;
#endregion  // Private variables
		
#region  Public variables
		/// <summary>
		///   Material to use for the on state.
		/// </summary>
		public Material _onMaterial;

		/// <summary>
		///   Material to use for the off state.
		/// </summary>
		public Material _offMaterial;
#endregion  // Public variables

#region  Callback methods
		public void Awake () {
			_grid     = GetComponent<PolarGrid>();
			_cylinder = GetComponent<Cylinder>();

			var rings = Mathf.FloorToInt(_cylinder.RadialTo);
			var sectors = _grid.Sectors;
			
			// Loop through the rings, in every ring loop through the sectors.
			// For every sector create a new object at the center of the face.
			for (var ring = 0; ring < rings; ++ring) {
				for (var sector = 0; sector < sectors; ++sector) {
					var go = BuildObject(ring, sector);
					SetComponents(go, ring, sector);
				}
			}
		}
#endregion  // Callback methods
		
#region  Private methods
		/// <summary>
		///   Instantiates a light switch.
		/// </summary>
		/// <param name="ring">
		///   Ring of the object.
		/// </param>
		/// <param name="sector">
		///   Sector of the object.
		/// </param>
		private GameObject BuildObject (int ring, int sector) {
			// instantiate the object and give it a fitting name
			var go = new GameObject();
			go.name = "polarBlock_" + ring + "_" + sector;
			
			// Position it at the face's center and make it a child of the grid
			// (just for cleanliness, no special reason)
			var position = new Vector3(ring + .5f, sector + .5f, 0);
			go.transform.position = _grid.GridToWorld(position);
			go.transform.parent = transform;
			
			return go;
		}

		/// <summary>
		///   Set up the components for the lights-out game.
		/// </summary>
		/// <param name="go">
		///   GameObject to set the components at.
		/// </param>
		/// <param name="ring">
		///   Layer of the object.
		/// </param>
		/// <param name="sector">
		///   Sector of the object.
		/// </param>
		private void SetComponents(GameObject go, int ring, int sector) {
			// Add a mesh filter, a mesh renderer, a mesh collider and then
			// construct the mesh (also pass the iteration steps for reference)
			go.AddComponent<MeshRenderer>();
			var mf = go.AddComponent<MeshFilter>();
			var mc = go.AddComponent<MeshCollider>();
			var  t = go.transform;
			BuildMesh(t, mf, mc, ring, sector);
			
			// Add the script for lights and set up the variables
			var lb = go.AddComponent<LightsBehaviour>();
			lb._onMaterial  = _onMaterial;
			lb._offMaterial = _offMaterial;
			lb._grid = _grid;

			// Perform a light switch back and forth so the object picks up the
			// newly assigned materials (kind of messy, but harmless enough)
			lb.SwitchLights();
			lb.SwitchLights();
		}
		
		/// <summary>
		///   Construct the mesh of the tile.
		/// </summary>
		/// <param name="t">
		///   Transform of the tile.
		/// </param>
		/// <param name="mf">
		///   Mesh filter of the tile.
		/// </param>
		/// <param name="mc">
		///   Mesh collider of the tile.
		/// </param>
		/// <param name="ring">
		///   Layer of the object.
		/// </param>
		/// <param name="sector">
		///   Sector of the object.
		/// </param>
		private void BuildMesh(Transform t, MeshFilter mf, MeshCollider mc, int ring, int sector) {
			var smoothness = _cylinder.Smoothness;
			var subSectors = smoothness + 1;
			var mesh = new Mesh();
			mesh.Clear();
			
			// the vertices of our new mesh, separated into two groups
			var innerVertices = new Vector3[subSectors];  // closer to the center
			var outerVertices = new Vector3[subSectors];  // away from the center

			for (var k = 0; k < subSectors; ++k) {
				var subSector = sector + (float)k / smoothness;

				var innerVertex = new Vector3(ring, subSector, 0);
				var outerVertex = innerVertex + Vector3.right;

				innerVertex = _grid.GridToWorld(innerVertex);
				outerVertex = _grid.GridToWorld(outerVertex);

				innerVertices[k] = t.InverseTransformPoint(innerVertex);
				outerVertices[k] = t.InverseTransformPoint(outerVertex);
			}
			
			// This is where the actual vertices go. For each inner vertex its
			// outer counterpart has the same index plus 'subSectors', this
			// will be relevant later
			var vertices = new Vector3[2 * subSectors];

			innerVertices.CopyTo(vertices, 0);
			outerVertices.CopyTo(vertices, subSectors);
			mesh.vertices = vertices;
			
			// Now we have to assign the triangles. For each smoothing step we
			// need two triangles and each triangle is three indices
			var triangles = new int[6 * smoothness];
			var counter = 0; // keeps track of the current index
			for (var k = 0; k < smoothness; ++k) {
				// triangles are assigned in a clockwise fashion
				triangles[counter] = k;
				triangles[counter+1] = k + subSectors + 1;
				triangles[counter+2] = k + subSectors;
				triangles[counter+3] = k + 1;
				triangles[counter+4] = k + subSectors + 1;
				triangles[counter+5] = k;

				counter += 6; // increment the counter for the next six indices
			}
			mesh.triangles = triangles;
			
			// Add some dummy UVs to keep the shader happy or else it
			// complains, but they are not used in this example
			var uvs = new Vector2[vertices.Length];
        	for (var k = 0; k < uvs.Length; ++k) {
            	uvs[k] = new Vector2(vertices[k].x, vertices[k].y);
        	}
        	mesh.uv = uvs;
			
			// the usual cleanup
			mesh.RecalculateNormals();
			mesh.RecalculateBounds();
			mesh.Optimize();
			
			// assign the mesh  to the mesh filter and mesh collider
			mf.mesh = mesh;
			mc.sharedMesh = mesh;
		}
#endregion  // Private methods
	}
}
