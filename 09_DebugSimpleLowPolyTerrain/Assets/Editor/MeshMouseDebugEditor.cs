using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MeshMouseDebug))]
public class MeshMouseDebugEditor : Editor {
	void OnEnable()
	{
		SceneView.onSceneGUIDelegate += SceneGUI;
	}

	void SceneGUI(SceneView sceneView)
	{
		// This will have scene events including mouse down on scenes objects
		Event cur = Event.current;
		if (cur.isMouse) {
			var t = (MeshMouseDebug)target;
			var ray = HandleUtility.GUIPointToWorldRay (cur.mousePosition);
			var posRef = t.DebugMeshAtCursor (ray);

			if (posRef.HasValue) {
//				var pos = posRef.Value;
//				Handles.color = Color.cyan;
//				Handles.SphereHandleCap (0, pos, Quaternion.identity, 1, EventType.Repaint);
//				var cube = GameObject.CreatePrimitive (PrimitiveType.Cube);
//				cube.transform.position = pos;
			}
		}
	}

	public override void OnInspectorGUI ()
	{
		
		base.OnInspectorGUI ();
	}


	[DrawGizmo(GizmoType.Selected | GizmoType.Active)]
	static void DrawGizmoForMyScript(MeshMouseDebug t, GizmoType gizmoType)
	{
		//		if (Vector3.Distance(position, Camera.current.transform.position) > 10f)
		//			Gizmos.DrawIcon(position, "MyScript Gizmo.tiff");

	}
}


