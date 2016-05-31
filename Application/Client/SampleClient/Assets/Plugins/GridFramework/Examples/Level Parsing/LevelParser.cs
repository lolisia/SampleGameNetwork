using UnityEngine;
using System.Collections.Generic; //needed for the generic list class
using System.IO;                  //needed for the StringReader class
using GridFramework.Grids;

namespace GridFramework.Examples.LevelParsing {
	/// <summary>
	///   Reads a text file, pases its contents and instantiates blocks based
	///   on that.
	/// </summary>
	/// <remarks>
	///   <para>
	///     The level files and block prefabs are assigned to this script, then
	///     the script keeps cycling through the files. Every block
	///     instantiated is kept track of in oder to be be able to remove them
	///     again.
	///   </para>
	///   <para>
	///     This example is using a hexagonal grid, but it can be just as well
	///     be used for any other type of grid.
	///   </para>
	/// </remarks>
	[RequireComponent(typeof(Grid))]
	public class LevelParser : MonoBehaviour {
#region  Private variables
		/// <summary>
		///   The grid we place blocks on.
		/// </summary>
		private HexGrid _grid;
		
		/// <summary>
		///   Which level from the levels array to load.
		/// </summary>
		private int _level;

		/// <summary>
		///   In order to delete all the blocks we need to keep track of them.
		/// </summary>
		private List<GameObject> blocks;

		private List<string> _levelData = new List<string>();
#endregion  // Private variables

#region  Public variables
		/// <summary>
		///   An array of text files to be read.
		/// </summary>
		public TextAsset[] _levelFiles;
			
		// Prefabs for our objects
		/// <summary>
		///   Prefab for the red object.
		/// </summary>
		public GameObject _red;

		/// <summary>
		///   Prefab for the green object.
		/// </summary>
		public GameObject _green;

		/// <summary>
		///   Prefab for the blue object.
		/// </summary>
		public GameObject _blue;
#endregion  // Public variables
		
#region  Callback methods
		public void Awake(){
			_grid = GetComponent<HexGrid>();
			blocks = new List<GameObject>();

			BuildLevel(_levelFiles[_level], _grid, _red, _green, _blue);
		}

		void OnGUI(){
			const string message = "Try Another Level";
			var buttonRect = new Rect(10, 10, 150, 50);
			var  levelRect = new Rect(10, Screen.height - 10 - 200, 130, 130);

			if (GUI.Button(buttonRect, message)) {
				// Increment the level counter; using % makes the number revert
				// back to 0 once we have reached the limit. Also clear the
				// current level data.
				_level = (_level + 1) % _levelFiles.Length;
				_levelData.Clear();

				BuildLevel(_levelFiles[_level], _grid, _red, _green, _blue);
			}

			var levelString = "Level text data:\n\n";
			foreach (var line in _levelData) {
				levelString += line + "\n";
			}
			GUI.TextArea(levelRect, levelString);
		}
#endregion  // Callback methods

#region  Private methods
		/// <summary>
		///   Spawns blocks based on a text file and a grid.
		/// </summary>
		private void BuildLevel(TextAsset levelFile, HexGrid grid, GameObject r, GameObject g, GameObject b) {
			if (!r || !g || !b) {
				throw new System.NullReferenceException("Blocks references are null.");
			}
			
			// loop though the list of old blocks and destroy all of them, we
			// don't want the new level on top of the old one. Destroying the
			// blocks doesn't remove the reference to them in the list, so
			// clear the list
			foreach (var go in blocks) {
				Destroy(go);
			}
			blocks.Clear();

			
			// Setup the reader, a variable for storing the read line and keep
			// track of the number of the row we just read
			var reader = new StringReader(levelFile.text);
			var row = 0;
			
			// Read the text file line by line as long as there are lines
			for (var line = reader.ReadLine(); line != null; line = reader.ReadLine(), ++row) {
				_levelData.Add(line);
				//read each line character by character
				for (var column = 0; column < line.Length; ++column) {
					var position = grid.HerringUpToWorld(new Vector3(column, row, 0));
					var block = Char2Object(line[column], r, g, b);
					CreateBlock(block, position);
				}
			}
		}
		
		/// <summary>
		///   Spawn a block in the level.
		/// </summary>
		private void CreateBlock(GameObject block, Vector3 position){
			if (!block) {
				return;
			}
			var obj = Instantiate(block, position, Quaternion.identity) as GameObject;
			//add that block into our list of blocks
			blocks.Add(obj);
		}

		private GameObject Char2Object(char c, GameObject r, GameObject g, GameObject b) {
			GameObject block = null;
			switch (c) {
				case 'R': block = r; break;
				case 'G': block = g; break;
				case 'B': block = b; break;
			}
			return block;
		}
#endregion  // Private methods
	}
}
