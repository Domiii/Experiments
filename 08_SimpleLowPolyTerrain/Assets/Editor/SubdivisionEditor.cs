using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Subdivision))]
public class SubdivisionEditor : Editor {
	public override void OnInspectorGUI ()
	{
		var b = (Subdivision)target;

		if (GUILayout.Button ("Subdivide!")) {
			b.Build ();
		}

		base.OnInspectorGUI ();
	}
}