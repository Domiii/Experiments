using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Chain2D))]
public class Chain2DEditor : Editor {
	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();

		var chain = (Chain2D)target;

		if (GUILayout.Button ("Clear")) {
			chain.Clear ();
		}

		else if (GUILayout.Button ("Build Chain")) {
			chain.Build ();
		}
	}
}
