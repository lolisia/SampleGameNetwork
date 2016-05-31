using UnityEngine;
using GridFramework.Grids;
using GridFramework.Renderers.Rectangular;
using GridFramework.Extensions.Nearest;

using CoordinateSystem = GridFramework.Grids.RectGrid.CoordinateSystem;

// This script is not a MonoBehaviour, it's a public static class that doesn't
// inherit from anything. The class has three functions: it stores a
// two-dimensional array of bool values (I will call this array matrix from now
// on and its entries squares), it sets and returns the value of squares based
// on which world positions they correspond to.  Each square represents a face
// of a rectangular grid on the XY plane and its position in the matrix mirrors
// its position in the grid with (0,0) being the lower left face, the first
// index denotes the row and the second index the column of a face in the grid.
// This class works together with two other scripts, SetupForbiddenFiles and
// BlockSquare.  The matrix is built first by SetupForbiddenFiles in the
// Awake() function to make sure it is built before any Start() function gets
// called. It just calls the matrix-building method and passes which grid to
// use for reference. Then each obstacle fires the Start() method from its
// BlockSquare script to set their tiles as forbidden. The player's movement
// script makes use of the CheckSquare method to find out if a face is
// forbidden or not, but it doesn't write anything to the matrix.
// 
// This script demonstrates how you can use Grid Framework to store information
// about individual tiles apart from the objects they belong to in a format
// accessible to all objects in the scene.  The information does not have to be
// limited to just boolean values, you can extend this approach to store any
// other information you might have and build more complex game machanics
// around it.
// 
// NOTE: the purpose of the "originSquare" variable might not be clear at
// first.  In matrix coordinates the lower left square has (0,0) and all the
// other square are relative to it. However, if your grid does not start at the
// origin the matrix coordinates and the grid coordinates don't match, i.e. if
// your grid starts in (-1,2) then a square that's 3 units to the right and one
// up has grid coordinates (2,3) and matrix coordinates (3,1). In order to
// properly convert between matrix and grid we need to be able to tell the
// square's position relative to the lower left square. If your grid starts at
// (0,0) this would all be irrelevant

namespace GridFramework.Examples.Movement {
	public static class ForbiddenTiles {
#region  Public variables
		/// <summary>
		///   Two-dimensional array of bool values.
		/// </summary>
		/// <remarks>
		///   <para>
		///     <c>True</c> means the tile is legal to step on.
		///   </para>
		/// </remarks>
		public static bool[,] _allowedTiles;

		/// <summary>
		///   The grid everything is based on.
		/// </summary>
		public static RectGrid _grid;

		/// <summary>
		///   The renderer of the grid.
		/// </summary>
		public static Parallelepiped _renderer;

		/// <summary>
		///   The grid coordinates of the lower left square used for reference
		///   (X and Y only).
		/// </summary>
		public static int[] _originSquare;
#endregion  // Public variables
		
#region  Public methods
		/// <summary>
		///   Builds the matrix and sets everything up, gets called by a script
		///   attached to the grid object.
		/// </summary>
		public static void Initialize(RectGrid grid, Parallelepiped renderer){
			_grid = grid;
			_renderer = renderer;

			// Builds a default matrix that has all entries set to true.
			BuildMatrix();

			// Stores the X and Y grid coordinates of the lower left square.
			SetOriginSquare();
		}
		
		/// <summary>
		///   Takes the grids size or rendering range and builds a matrix based
		///   on that.
		/// </summary>
		/// <remarks>
		///   <para>
		///     All entries are set to true.
		///   </para>
		/// </remarks>
		public static void BuildMatrix(){
			// Amount of rows and columns, either based on size or rendering
			// range (first entry rows, second one columns)
			var size = SetMatrixSize();

			// Build a default matrix
			_allowedTiles = new bool[size[0], size[1]];
							
			// The matrix is initially empty, so all tiles are allowed
			for (var i = 0; i < size[0]; ++i) {
				for (var j = 0; j < size[1]; ++j) {
					_allowedTiles[i, j] = true;
				}
			}
		}
		
		/// <summary>
		///   Takes world coordinates, finds the corresponding square and sets
		///   that entry to either true or false.
		/// </summary>
		/// <remarks>
		///   <para>
		///     Use it to disable or enable squares
		///   </para>
		/// </remarks>
		public static void RegisterSquare(Vector3 vec, bool status){
			// First find the square that belongs to that world position, then
			// set its value
			var square = GetSquare(vec);
			_allowedTiles[square[0],square[1]] = status;
		}
		
		/// <summary>
		///   Takes world coodinates, finds the corresponding square and
		///   returns the value of that square.
		/// </summary>
		/// <remarks>
		///   <para>
		///     Use it to cheack if a square is forbidden or not.
		///   </para>
		/// </remarks>
		public static bool CheckSquare(Vector3 vec){
			var square = GetSquare(vec);
			return _allowedTiles[square[0],square[1]];
		}
		
		/// <summary>
		///   This returns the matrix as a string so you can read it yourself,
		///   like in a GUI for debugging (nothing grid-related going on here,
		///   feel free to ignore it)
		/// </summary>
		/// <remarks>
		///   <para>
		///   There is nothing grid-related going on here, feel free to ignore
		///   this method.
		///   </para>
		/// </remarks>
		public static string MatrixToString(){
			const string falseString = "X";
			const string trueString  = "_";

			var text = "Occupied fields are "
				+ falseString + ", free fields are "
				+ trueString +":\n\n";

			var height = _allowedTiles.GetLength(1);
			var width  = _allowedTiles.GetLength(0);

			for(var j = height - 1; j >= 0; --j) {
				for(var i = 0; i < width; ++i) {
					var allowed = _allowedTiles[i,j];
					var entry = ( allowed ? trueString: falseString) + " ";
					text += entry;
				}
				text += "\n";
			}
			return text;
		}
#endregion  // Public methods

#region  Private methods
		/// <summary>
		///   How large should the matrix be?
		/// </summary>
		/// <remarks>
		///   <para>
		///     Depends on whether we use "size" or "custom rendering range"
		///   </para>
		/// </remarks>
		private static int[] SetMatrixSize() {
			var size = new int[2];
			for(var i = 0; i < 2; ++i){
				var from = Mathf.CeilToInt( _renderer.From[i]);
				var to   = Mathf.FloorToInt(_renderer.To[i]  );

				size[i] = to - from;
			}
			return size;
		}

		/// <summary>
		///   Stores the grid coodinates of the lower left square.
		/// </summary>
		/// <remarks>
		///   <para>
		///     This is needed to properly map a grid's face to a matrix entry.
		///     First we find out its world coordinates, then we translate then
		///     to grid coordinates.
		///   </para>
		/// </remarks>
		private static void SetOriginSquare() {
			var from = _renderer.From;

			_originSquare = new [] {
				Mathf.RoundToInt(from.x),
				Mathf.RoundToInt(from.y)
			};
		}
		
		/// <summary>
		///   Takes world coordinates and finds the corresponding square.
		/// </summary>
		/// <remarks>
		///   <para>
		///     The result is returned as an int array that contains that
		///     square's position in the matrix
		///   </para>
		/// </remarks>
		private static int[] GetSquare(Vector3 vec){
			const CoordinateSystem system = CoordinateSystem.Grid;

			var cell = _grid.NearestCell(vec, system);

			var square = new [] {
				Mathf.FloorToInt(cell.x) - _originSquare[0],
				Mathf.FloorToInt(cell.y) - _originSquare[1],
			};

			return square;
		}
#endregion  // Private methods
	}
}
