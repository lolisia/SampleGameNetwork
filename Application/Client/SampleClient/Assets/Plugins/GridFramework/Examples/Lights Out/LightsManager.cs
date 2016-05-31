using UnityEngine;
using GridFramework.Grids;

// This script declares a delegate and defines an event. The event will be
// fired when a switch is clicked and passes the grid as well as the switch's
// coordinates in grid space. Other than that it has nothing to do with grid
// Framework. The event is handled and fired in a seperate script.

namespace GridFramework.Examples.LightsOut {
	public static class LightsManager {
		/// <summary>
		///   Declare a delegate to react when a switch is pressed.
		/// </summary>
		/// <remarks>
		///   <para>
		///     The delegate passes arguments the coordinates of the switch and
		///     which grid to use.
		///   </para>
		/// </remarks>
		public delegate void SwitchingHandler(Vector3 switchCoordinates, PolarGrid grid);

		public static event SwitchingHandler OnHitSwitch;
			
		/// <summary>
		/// </summary>
		/// <param name="reference">
		///   Position of the switch that was pressed.
		/// </param>
		/// <param name="grid">
		///   Grid we use for gameplay.
		/// </param>
		/// <remarks>
		///   <para>
		///     This function broadcasts a signal (an event) once a switch has
		///     been hit. Static means we don't need to use any specific
		///     instance of this function.
		///   </para>
		/// </remarks>
		public static void SendSignal(Vector3 reference, PolarGrid grid){
			//always make sure there a subscribers to the event, or you get errors
			if(OnHitSwitch != null)
				OnHitSwitch(reference, grid);
		}
	}
}
