using UnityEngine;
using GridFramework.Grids;

// this is a simple script to generate a square mesh to hold the (sprite) image of the dial

namespace GridFramework.Examples.RotaryDial {
	/// <summary>
	///   Build the rotary dial sprite at runtime.
	/// </summary>
	/// <remarks>
	///   This constructs a two-triangle mesh we use as the sprite. The
	///   material has already been assigned to the object in the editor.
	/// </remarks>
	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]
	[RequireComponent(typeof(MeshCollider))]
	[RequireComponent(typeof(PolarGrid))]
	public class RotaryDialSprite : MonoBehaviour {

		void Awake () {
			const float size = 5;

			var mesh = BuildMesh(size);
			var mf = GetComponent<MeshFilter>();
			var mc = GetComponent<MeshCollider>();

			mf.mesh       = mesh;
			mc.sharedMesh = mesh;
		}

		/// <summary>
		///   Builds a simple two-triangle mesh.
		/// </summary>
		/// <param name="size">
		///   Size of sprite (it's a square).
		/// </param>
		Mesh BuildMesh(float size = 1.0f) {
			var mesh = new Mesh();

			mesh.vertices = new [] {
				new Vector3(-size, -size, 0),  // bottom left
				new Vector3(-size,  size, 0),  // top left
				new Vector3( size,  size, 0),  // top right 
				new Vector3( size, -size, 0)   // bottom right
			};

			var tris =  new [] { 0, 1, 2, 0, 2, 3 };

			var uvs  = new [] {
				Vector2.zero,
				Vector2.up,
				Vector2.one,
				Vector2.right
			};

			mesh.triangles = tris;
			mesh.uv = uvs;

			mesh.Optimize();
			return mesh;
		}
	}
}
