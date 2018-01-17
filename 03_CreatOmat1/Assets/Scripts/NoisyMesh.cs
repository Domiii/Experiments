using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoisyMesh : MonoBehaviour {
	public float speed = .5f;
	public float scale = .3f;
	public int seed = 23455898;

	public bool isMoving = true;
	public float time;

	float seedX, seedY, seedZ;

	MeshFilter meshFilter;
	Perlin noise = new Perlin ();
	Vector3[] baseVertices;

	void Awake() {
		meshFilter = GetComponent<MeshFilter> ();

		Random.InitState (seed);
		seedX = Random.Range (0, 1);
		seedY = Random.Range (2, 4);
		seedZ = Random.Range (5, 7);

		time = Time.time;
	}

	void Update() {
		Deform2 ();

		if (isMoving) {
			time += Time.deltaTime;
		}
	}

	void Deform2() {
		var mesh = meshFilter.mesh;
		//var vs = mesh.vertices;

		if (baseVertices == null) {
			baseVertices = mesh.vertices;
		}

		var vs = new Vector3[baseVertices.Length];
		//var vs = mesh.vertices;


		var noiseDx = time * speed + seedX;
		var noiseDy = time * speed + seedY;
		var noiseDz = time * speed + seedZ;


		for (var i = 0; i < vs.Length; ++i) {
			var v = baseVertices[i];

			// convert to spherical coordinates
			var r = v.magnitude;
			var theta = Mathf.Acos(v.z / r);
			//var phi = Mathf.Atan (v.y / (v.x + .1f));

			var inX = r;
			var inY = theta;
			var inZ = Mathf.PI;

			var dx = noise.Noise (inX + noiseDx, inY + noiseDx, inZ + noiseDx);
			var dy = noise.Noise (inX + noiseDy, inY + noiseDy, inZ + noiseDy);
			var dz = noise.Noise (inX + noiseDz, inY + noiseDz, inZ + noiseDz);

			v.x += dx * scale;
			v.y += dy * scale;
			v.z += dz * scale;
			vs[i] = v;
		}

		mesh.vertices = vs;

		mesh.RecalculateNormals ();
		mesh.RecalculateTangents ();
		mesh.RecalculateBounds ();
	}


	/// <summary>
	/// The overall direction obviously is following the opposite of the positive x,y,z direction.
	/// There is some odd correlation and regularity to this method: The same pattern repeats all the time.
	/// For speed = 1, the pattern can be observed to repeat for every multiple of 1.
	/// For speed = .6, the pattern can be observed to repeat for every multiple of 5.
	/// For speed = .8, the pattern can be observed to repeat for every multiple of 4.
	/// </summary>
	void Deform1() {
		var mesh = meshFilter.mesh;
		//var vs = mesh.vertices;

		if (baseVertices == null) {
			baseVertices = mesh.vertices;
		}

		//var vs = new Vector3[baseVertices.Length];
		var vs = mesh.vertices;

		var noiseDx = time * speed + seedX;
		var noiseDy = time * speed + seedY;
		var noiseDz = time * speed + seedZ;


		for (var i = 0; i < vs.Length; ++i) {
			var v = baseVertices[i];

			var inX = v.x;
			var inY = v.y;
			var inZ = v.z;

			var dx = noise.Noise (inX + noiseDx, inY + noiseDx, inZ + noiseDx);
			var dy = noise.Noise (inX + noiseDy, inY + noiseDy, inZ + noiseDy);
			var dz = noise.Noise (inX + noiseDz, inY + noiseDz, inZ + noiseDz);

			v.x += dx * scale;
			v.y += dy * scale;
			v.z += dz * scale;
			vs[i] = v;
		}

		mesh.vertices = vs;

		mesh.RecalculateNormals ();
		mesh.RecalculateTangents ();
		mesh.RecalculateBounds ();
	}
}
