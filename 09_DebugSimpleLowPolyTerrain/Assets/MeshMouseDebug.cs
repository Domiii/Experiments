using UnityEngine;
using System.Collections;


/// This script draws a debug line around mesh triangles
/// as you move the mouse over them.
/// @see https://docs.unity3d.com/ScriptReference/RaycastHit-triangleIndex.html
public class MeshMouseDebug : MonoBehaviour
{
	Camera cam;
	public Color color = Color.yellow;

	void Start ()
	{
		cam = GetComponent<Camera> ();
		if (cam == null) {
			cam = Camera.main;
		}
	}

	void Update ()
	{
		var ray = cam.ScreenPointToRay (Input.mousePosition);
		DebugMeshAtCursor (ray);
	}

	public Vector3? DebugMeshAtCursor(Ray ray)
	{
//		if (cam == null) {
//			cam = Camera.current;
//			if (cam == null) {
//				return null;
//			}
//		}

		RaycastHit hit;
		if (!Physics.Raycast (ray, out hit, 10000))
			return null;

		MeshCollider meshCollider = hit.collider as MeshCollider;
		if (meshCollider == null || meshCollider.sharedMesh == null)
			return null;

		Mesh mesh = meshCollider.sharedMesh;
		Vector3[] vertices = mesh.vertices;
		int[] triangles = mesh.triangles;

		var i0 = hit.triangleIndex * 3 + 0;

		Vector3 p0 = vertices [triangles [i0]];
		Vector3 p1 = vertices [triangles [i0 + 1]];
		Vector3 p2 = vertices [triangles [i0 + 2]];
		Transform hitTransform = hit.collider.transform;
		p0 = hitTransform.TransformPoint (p0);
		p1 = hitTransform.TransformPoint (p1);
		p2 = hitTransform.TransformPoint (p2);

		Debug.DrawLine (p0, p1, color);
		Debug.DrawLine (p1, p2, color);
		Debug.DrawLine (p2, p0, color);

		// i0 is the first vertex of the triangle that was hit
		meshCollider.SendMessage ("OnMeshDebug", i0, SendMessageOptions.DontRequireReceiver);

		return hit.point;
	}
}
