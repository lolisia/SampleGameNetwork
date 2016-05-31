using UnityEngine;
using GridFramework.Grids;

// This script creates an extension method for the Grid class, which means it
// adds a new method to the class without the need to alter the source code of
// the class. Once declared you can use the method just like any normal class
// method.
// 
// One of the design principles behind Grid Framework says that the Grid class
// and all its methods are abstract and that the exact implementation lies in
// the sublasses. However, since extensions methods are static they cannot be
// abstract, it's simply part of the language design of C#, so I had to do a
// workaround. The method performs some preperation and then it picks the
// implementation based on the exact type of grid. Note that those functions
// are not extension methods, they are just regular methods of this static
// class.

namespace GridFramework.Examples.LightsOut {
	/// <summary>
	///   Extension methods for this example.
	/// </summary>
	public static class Extensions {
		/// <summary>
		///   Whether two points are adjacent in a grid.
		/// </summary>
		/// <param name="grid">
		///   Basis for comparison.
		/// </param>
		/// <param name="a">
		///   First coordinate in grid coordinates.
		/// </param>
		/// <param name="b">
		///   Second coordinate in grid coordinates.
		/// </param>
		public static bool IsAdjacent(this PolarGrid grid, Vector3 a, Vector3 b) {
			// Tiles are adjacent but not diagonal if the sum of the unsigned
			// difference of their coordinates is less than 1.
			var sectors = grid.Sectors;
			a = grid.WorldToGrid(a);
			b = grid.WorldToGrid(b);

			// Wrap-around edge case.
			if (Delta(a.y, sectors) <= .75f && b.y <= .75f) {
				b.y += sectors;
			}
			if (Delta(b.y, sectors) <= .75f && a.y <= .75f) {
				a.y += sectors;
			}

			// This will math adjacent but not diagonal tiles.
			var adjacent = Delta(a.x, b.x) + Delta(a.y, b.y) <= 1.25;

			return adjacent;
		}

		/// <summary>
		///   Absolute difference between two values.
		/// </summary>
		private static float Delta(float a, float b) {
			return Mathf.Abs(a - b);
		}
	}
}
