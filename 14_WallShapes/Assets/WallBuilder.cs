using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

// see: https://github.com/mariuszgromada/MathParser.org-mXparser
using org.mariuszgromada.math.mxparser;

public class WallBuilder : MonoBehaviour
{
	public string y = "sin(x^2)*x^2 / 10";
	public float xStart = -10;
	public float xEnd = 10;
	public float gap = .01f;

	public MeshFilter prefab;

	[HideInInspector]
	[SerializeField]
	Bounds? prefabBounds;

	public int NX {
		get {
			if (prefabBounds.HasValue) {
				return Mathf.RoundToInt((xEnd - xStart) / XStep);
			}
			return 0;
		}
	}

	public float XStep { 
		get {
			if (prefabBounds.HasValue) {
				var sw = prefabBounds.Value.extents.x * 2;
				return sw + gap;
			}
			return float.NaN;
		}
	}

	public float YStep {
		get {
			if (prefabBounds.HasValue) {
				var sh = prefabBounds.Value.extents.y * 2;
				return sh + gap;
			}
			return float.NaN;
		}
	}

	int ComputeNY(float h) {
		return Mathf.RoundToInt(h * XStep / YStep);
	}

	public void Build ()
	{
		Clear ();

		var go = Instantiate (prefab);
		prefabBounds = go.sharedMesh.bounds;
		DestroyImmediate (go);

		var xStep = XStep;

		var f = new Function ("f(x) = " + y);
		var isValid = true;
		for (var i = 0; i < NX; ++i) {
			var x = xStart + i * XStep;

			var height = (float)f.calculate (x);
			if (float.IsPositiveInfinity (height)) {
				// just make it very large
				height = 1e6f;
			} else if (float.IsNegativeInfinity (height)) {
				// just make it very small
				height = -1e6f;
			}

			if (float.IsNaN (height)) {
				isValid = false;
				height = 0;
			}

			var pos = new Vector3 (x, height, 0);
			var ny = ComputeNY (height);

			for (var j = 0; j < ny; ++j) {
				pos.y = j * YStep;
				go = Instantiate (prefab, transform);
				go.transform.localPosition = pos;
			}
		}

		if (!isValid) {
			print ("Could not evaluate function for all values of x. Make sure that your function expression is valid.");
		}
	}

	/// <summary>
	/// Delete all children (bricks).
	/// 把所有的小孩都　｢清掉｣.
	/// </summary>
	public void Clear ()
	{
		for (var i = transform.childCount - 1; i >= 0; --i) {
			DestroyImmediate (transform.GetChild(i).gameObject);
		}
	}
}

#if UNITY_EDITOR
[CustomEditor (typeof(WallBuilder))]
public class WallBuilderEditor : Editor
{
	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();

		var t = (WallBuilder)target;

		GUILayout.Label ("nX: " + t.NX);

		if (GUILayout.Button ("Build!")) {
			t.Build ();
		} else if (GUILayout.Button ("Clear")) {
			t.Clear ();
		}
	}
}
#endif