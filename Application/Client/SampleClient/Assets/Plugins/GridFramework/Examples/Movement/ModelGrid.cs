using UnityEngine;
using GridFramework.Grids;
using GridFramework.Renderers.Rectangular;

namespace GridFramework.Examples.Movement {
	[RequireComponent(typeof (RectGrid))]
	[RequireComponent(typeof (Parallelepiped))]
	public class ModelGrid : MonoBehaviour {
	
		// Awake is called before Start()
		void Awake() {
			// We will build the matrix based on the grid that is attached to
			// this object.  All entries are true by default, then each
			// obstacle will mark its entry as false.
			var grid = GetComponent<RectGrid>();
			var para = GetComponent<Parallelepiped>();

			ForbiddenTiles.Initialize(grid, para);
		}
		
		void OnGUI() {
			var rect = new Rect (10, 10, 250, 200);
			var text = ForbiddenTiles.MatrixToString();
			GUI.TextArea(rect, text);
		}
	}
}
