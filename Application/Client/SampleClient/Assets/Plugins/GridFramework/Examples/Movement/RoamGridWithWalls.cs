using UnityEngine;
using GridFramework.Grids;
using GridFramework.Renderers.Rectangular;
using GridFramework.Extensions.Align;

// This script is similar to the RoamGrid script from the grid-based movement
// example.  In addition this script will check before each move whether the
// target tile is forbidden or not and if it is it will pick another dircetion.
// The information which tiles are forbidden and which aren't is stored in a
// public static bool matrix.
// 
// This script demonstrates how you can use Grid Framework to store information
// about individual tiles apart from the objects they belong to in a format
// accessible to all objects in the scene.

namespace GridFramework.Examples.Movement {
	public class RoamGridWithWalls : MonoBehaviour {
#region  Private variables
		private RectGrid _grid;
		private Parallelepiped _renderer;

		/// <summary>
		///   Whether the object is to move or not.
		/// </summary>
		private bool _doMove;

		/// <summary>
		///   Whether the object will move to.
		/// </summary>
		private Vector3 _goal;

		/// <summary>
		///   How fast to move.
		/// </summary>
		private float _moveSpeed;
#endregion  // Private variables

#region  Public variables
		/// <summary>
		///   How long it takes to move from one tile to another
		/// </summary>
		public float _stepTime = 1.0f;
#endregion  // Public variables
		
		
#region  Callback methods
		void Start(){
			_grid = ForbiddenTiles._grid;
			_renderer = ForbiddenTiles._renderer;
		
			_grid.AlignTransform(transform);
		}
		
		void Update(){
			if(_doMove){
				Move();
			} else {
				StopMove();
			}
		}
#endregion  // Callback methods
	
#region  Private methods
		private void Move() {
			//move towards the desination
			var t = _moveSpeed * Time.deltaTime;
			var newPosition = transform.position;

			newPosition.x = Mathf.MoveTowards(transform.position.x, _goal.x, t);
			newPosition.y = Mathf.MoveTowards(transform.position.y, _goal.y, t);

			transform.position = newPosition;

			// Check if we reached the destination (use a certain tolerance so
			// we don't miss the point becase of rounding errors)
			var deltaX = Mathf.Abs(transform.position.x - _goal.x);
			var deltaY = Mathf.Abs(transform.position.y - _goal.y);
			if( deltaX < 0.01f && deltaY < 0.01f) {
				_doMove = false;
			}
		}

		private void StopMove() {
			// Pick a new destination face randomly, then see if it is allowed.
			// If not try again.
			do {
				_goal = FindNextFace();
			} while (!ForbiddenTiles.CheckSquare(_goal));

			// Calculate speed by dividing distance (one of the two
			// distances will be 0, we need the other one) through time
			var deltaX = Mathf.Abs(transform.position.x - _goal.x);
			var deltaY = Mathf.Abs(transform.position.y - _goal.y);
			_moveSpeed = Mathf.Max(deltaX, deltaY) / _stepTime;
			// Resume movement with the new goal
			_doMove = true;
		}

		private Vector3 FindNextFace(){
			// We will be operating in grid space, so convert the position
			var newPosition = _grid.WorldToGrid(transform.position);
			
			newPosition += RandomDirection();
			newPosition  = ConstrainBounds(newPosition);

			return _grid.GridToWorld(newPosition);
		}

		private Vector3 RandomDirection() {
			var i = Random.Range(0, 4);
			var direction = Vector3.zero;

			switch (i) {
				case 0:
					direction = new Vector3(1,0,0);
					break;
				case 1:
					direction = new Vector3(-1,0,0);
					break;
				case 2:
					direction = new Vector3(0,1,0);
					break;
				case 3:
					direction = new Vector3(0,-1,0);
					break;
			}

			return direction;
		}

		private Vector3 ConstrainBounds(Vector3 target) {
			// If we would wander off beyond the size of the grid turn the
			// other way around
			for(int i = 0; i < 2; i++){
				var beyondLowerBound = target[i] < _renderer.From[i];
				var beyondUpperBound = target[i] > _renderer.To[i];

				if(beyondLowerBound) {
					target[i] += 2f;
				}

				if(beyondUpperBound) {
					target[i] -= 2f;
				}
			}
			return target;
		}
#endregion  // Private methods
	}
}
