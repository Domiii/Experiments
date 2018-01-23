using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshShaper2D : MonoBehaviour {
	public float speed = 2;
	public float scale = 2;

	/// <summary>
	/// The higher, the less detail you will see, unless you have a lot of octaves.
	/// </summary>
	public float frequency = 1;

	/// <summary>
	/// The amount of levels of detail you want to see (each twice as finely scaled as the previous)
	/// </summary>
	public int octaves = 8;
	public int seed = 23455898;

	public bool isMoving = true;
	public float time;

	float seedX, seedY;

	protected MeshFilter meshFilter;
	protected Vector3[] baseVertices;
	protected Vector3[] baseNormals;

	void Awake() {
		meshFilter = GetComponent<MeshFilter> ();

		var mesh = meshFilter.mesh;
		baseVertices = mesh.vertices;
		baseNormals = mesh.normals;

		Random.InitState (seed);
		seedX = Random.Range (1.0f, 2) * 14486;
		seedY = Random.Range (1.0f, 2) * 47940;

		time = Time.time;
	}

	void Update() {
		PullNormals ();

		if (isMoving) {
			time += Time.deltaTime;
		}
	}

	static float min = -.9f, max= .9f;

	/// <summary>
	/// Extend mesh randomly along normal
	/// </summary>
	void PullNormals() {
		var mesh = meshFilter.mesh;

		var vs = new Vector3[baseVertices.Length];
		//var vs = mesh.vertices;

		var xOffset = speed * time + seedX;
		var yOffset = speed * time + seedY;

		for (var i = 0; i < vs.Length; ++i) {
			var u = baseVertices[i];
			var v = vs [i];

			var x = u.x;
			var z = u.z;

			var val = SampleNoise(xOffset, yOffset, x, z);

			// randomly move along the normal
			var n = baseNormals [i];
			v = u + n * scale * val;
			vs[i] = v;
		}

		mesh.vertices = vs;

		mesh.RecalculateNormals ();
		mesh.RecalculateTangents ();
		mesh.RecalculateBounds ();
	}

	public float SampleNoise(float x, float y, float x0 = 0, float y0 = 0) {
		float val = 0;
		var gain = 1.0f;
		for (var o = 0; o < octaves; o++) {
			val += Mathf.PerlinNoise (x0 + x * gain / frequency, y0 + y * gain / frequency) / gain;
			gain *= 2.0f;
		}

		// re-scale, so delta will (mostly) be between 0 and 1
		min = Mathf.Min (min, val);
		max = Mathf.Max (max, val);
		val = (val - min) / (max-min);

		return val;
	}
}