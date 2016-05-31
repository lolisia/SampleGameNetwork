using UnityEngine;
using GridFramework.Grids;

// This script performs two tasks. For one, if the object it is attached to
// gets clicked it will fire off an event (defined in another script) which
// will be handled by this script as well. When the event is received the
// script compares the object's grid coordinates and the coordinates of the
// object that was clicked to decide what to do next.  This example uses
// delegates and events. While it would have been possible to do this in
// another way, delegates and events are the most elegant and best performing
// solution, since they are native to .NET
// 
// This script demonstrates how you can use grid coordinates for game logic
// with tiles that seem to work together but don't actually know anything about
// each other, giving you great freedom in designing your levels.

namespace GridFramework.Examples.LightsOut {
	/// <summary>
	///   The behaviour of every individual light switch.
	/// </summary>
	public class LightsBehaviour : MonoBehaviour {
#region  Public variables
		/// <summary>
		///   Material to use when the light is *on*.
		/// </summary>
		public Material _onMaterial;

		/// <summary>
		///   Material to use when the light is *off*.
		/// </summary>
		public Material _offMaterial;
		
		/// <summary>
		///   Current state of the switch.
		/// </summary>
		/// <remarks>
		///   <para>
		///     The state of the switch (intial set is done in the editor,rest
		///     at runtime).
		///   </para>
		/// </remarks>
		public bool _isOn;
		
		/// <summary>
		///   The grid we want to use for our game logic.
		/// </summary>
		public PolarGrid _grid;
#endregion  // Public variables

#region  Callback methods
		void Awake() {
			SwitchLights();
		}
		
		void OnEnable() {
			LightsManager.OnHitSwitch += OnHitSwitch;
		}
		
		void OnDisable() {
			LightsManager.OnHitSwitch -= OnHitSwitch;
		}
		
		void OnMouseUpAsButton() {
			LightsManager.SendSignal(transform.position, _grid);
		}

		void OnGUI() {
			const string guiMessage = "Click a tile and all adjacent tiles swap their colour, "+
				"the player's goal is to turn off all lights. This example uses events and "+
				"delegates to make all tiles compare their grid position to the clicked one's "+
				"grid position to decide whether to swap colours. The tiles themeselves don't "+
				"know anything about the rest of the grid.\n\nThe tiles of the polar grid are "+
				"using custom meshes constructed from code, rather than placing the tiles by hand.";

			var x = Screen.width - 10 - 425;
			var y = Screen.height - 10 - 130;
			const int w = 425;
			const int h = 130;
			var rect = new Rect (x, y, w, h);

			GUI.TextArea (rect, guiMessage);
		}
#endregion  // Callback methods

#region  Private methods
		/// <summary>
		///   Gets called upon the event <c>OnHitSwitch</c>.
		/// </summary>
		/// <param name="reference">
		///   Position of the clicked switch in grid coordinates.
		/// </param>
		/// <param name="grid">
		///   The grid we use for reference.
		/// </param>
		private void OnHitSwitch(Vector3 reference, PolarGrid grid) {
			// Don't do anything if this light doesn't belong to the grid we
			// use.
			if (grid != _grid) {
				return;
			}
			
			// Check if this light is adjacent to the switch; this is an
			// extenion method that always picks the method that belongs to the
			// specific grid type. The implementation is in another file.
			var isAdjacent = grid.IsAdjacent(transform.position, reference);

			if (isAdjacent) {
				_isOn = !_isOn;
				SwitchLights();
			}
		}
#endregion  // Private methods
		
#region  Public methods
		/// <summary>
		///   Toggles the material of the ligh.
		/// </summary>
		public void SwitchLights() {
			var lightRenderer = gameObject.GetComponent<Renderer>();
			lightRenderer.material = _isOn ? _onMaterial : _offMaterial;
		}
#endregion  // Public methods
	}
}
