using UnityEngine;

namespace GridFramework.Examples.Movement {
	public class BlockSquare : MonoBehaviour {
		// Start() is called after Awake(), this ensures that the matrix has
		// already been built.
		void Start() {
			// Set the entry that corresonds to the obstacle's position as
			// false.
			ForbiddenTiles.RegisterSquare(transform.position, false);
		}
	}
}
