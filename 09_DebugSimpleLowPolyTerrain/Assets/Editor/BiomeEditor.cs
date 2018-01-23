using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Biome))]
public class BiomeEditor : Editor {
	public override void OnInspectorGUI ()
	{
		var b = (Biome)target;

//		if (GUILayout.Button ("Randomize")) {
//			b.Randomize ();
//		}

		base.OnInspectorGUI ();
	}
}