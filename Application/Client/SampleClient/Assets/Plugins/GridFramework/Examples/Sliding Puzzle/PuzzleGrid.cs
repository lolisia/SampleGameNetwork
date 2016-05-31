using UnityEngine;
using GridFramework.Grids;
using GridFramework.Renderers.Rectangular;

namespace GridFramework.Examples.SlidingPuzzle {
	/// <summary>
	///   Behaviour that initializes the puzzle when the game starts with the
	///   grid and parallelepiped renderer.
	/// </summary>
	[RequireComponent(typeof(RectGrid))]
	[RequireComponent(typeof(Parallelepiped))]
	public class PuzzleGrid : MonoBehaviour {
		const string guiMessage =
			"Unit's built-in physics system is great for 3D games " +
			"with realistic behaviour, but sometimes you need more " +
			"basic predictable and 'video-gamey' behaviour. This " +
			"example doesn't use physics at all, instead it keeps track of " +
			"which squares are occupied and which are free, then it " +
			"restrictsmovement accordingly by clamping the position vectors.";
	
		// Awake is being called before Start; this makes sure we have a matrix
		// to begin with when we add the blocks
		void Awake() {
			// because of how we wrote the accessor this will also immediately
			// build the matrix of our level
			var grid = gameObject.GetComponent<RectGrid>();
			var para = gameObject.GetComponent<Parallelepiped>();
			SlidingPuzzle.InitializePuzzle(grid, para);
		}
		
		// visualizes the matrix in text form to let you see what's going on
		void OnGUI(){
			const int w1 = 400;
			const int h1 = 100;
			const int w2 = 250;
			const int h2 = 150;
			const int  x =  10;
			const int y1 =  10;

			var y2 = Screen.height - 10 - 250;

			var rect1 = new Rect(x, y1, w1, h1);
			var rect2 = new Rect(x, y2, w2, h2);

			var matrixString = SlidingPuzzle.MatrixToString();

			GUI.TextArea(rect1, guiMessage);
			GUI.TextArea(rect2, matrixString);
		}
	}
}
