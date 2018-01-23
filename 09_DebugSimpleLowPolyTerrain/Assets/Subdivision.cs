using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class Subdivision : MonoBehaviour {
	static readonly float InversePI = 1/Mathf.PI;

	public float detailLevel = 1;

	static Vector2 V3toUV(Vector3 p)
	{
		var d = p.normalized;
		var u = Mathf.Atan2(-d.z, -d.x) * InversePI * 0.5f + 0.5f;
		var v = 0.5f - Mathf.Asin(-d.y) * InversePI;
		return new Vector2(u, v);
	}

	public void Build() {
		var renderer = GetComponent<MeshRenderer> ();
		var filter = GetComponent<MeshFilter> ();
		var mesh = filter.sharedMesh;

		var vertexList = new List<Vector3>(mesh.vertices);
		var indexList = new List<int>(mesh.triangles);

		for (var i = 0; i < detailLevel; i++) {
			GeometryUtil.Subdivide (vertexList, indexList, true);
		}

		// compute UV coords
		var uvList = new List<Vector2> ();
		for (var i = 0; i < vertexList.Count; i++) {
			//var v = vertexList [i] = Vector3.Normalize (vertexList [i]);
			uvList.Add (V3toUV(vertexList[i]));
		}

		var vertices = vertexList.ToArray ();


		// create new mesh
		mesh = filter.mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.uv = uvList.ToArray();
		mesh.SetTriangles(indexList, 0, true);

		mesh.RecalculateNormals ();
		mesh.RecalculateTangents ();

		#if UNITY_EDITOR
		UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
		#endif

	}
}