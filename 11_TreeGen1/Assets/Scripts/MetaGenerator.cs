using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class MetaGenerator : MonoBehaviour
{
	public int gridW = 3;
	public int gridH = 3;
	public float itemGap = .1f;

	public Vector3 itemSize = new Vector3 (1, 1, Mathf.Infinity);

	public bool fitToGrid = true;
	public bool keepAspectRatio = true;

	public GameObject generator;
	public Transform parent;

	public void ClearAll() {
		for (var i = parent.childCount - 1; i >= 0; --i) {
			var t = parent.GetChild (i);
			DestroyImmediate (t.gameObject);
		}
	}

	public void GenerateAll ()
	{
		if (parent == null) {
			parent = generator.transform;
		}
		ClearAll ();
		var comps = generator.GetComponents <MonoBehaviour> ();
		var genComps = comps.Where (c => c is IGenerator).Cast<IGenerator> ();
		if (genComps.Count() == 0) {
			Debug.LogError ("Invalid Generator object must have at least one component implementing IGenerator");
			return;
		}

		var genComp = genComps.First ();
		for (var j = 0; j < gridH; ++j) {
			var y = j * (itemSize.y + itemGap);
			for (var i = 0; i < gridW; ++i) {
				var go = Generate (genComp);

				var x = i * (itemSize.x + itemGap);

				go.transform.localPosition = new Vector3 (x, y, 0);
			}
		}
	}

	public GameObject Generate (IGenerator gen)
	{
		var seed = Random.Range (0, 65536);

		GameObject go;
		if (gen is IMeshGenerator) {
			var obj = ((IMeshGenerator)gen).GenerateMesh (seed);
			Process (obj);
			go = obj.gameObject;
		} else if (gen is ISpriteGenerator) {
			var obj = ((ISpriteGenerator)gen).GenerateSprite (seed);
			Process (obj);
			go = obj.gameObject;
		} else {
			Debug.LogError ("Generator type not (yet) supported");
			return null;
		}

		go.transform.parent = parent;

		return go;
	}


	void Process (MeshFilter meshFilter)
	{
		var mesh = meshFilter.sharedMesh;
		var bounds = mesh.bounds;

		if (fitToGrid) {
			var ratioX = 2*bounds.extents.x / itemSize.x; 
			var ratioY = 2*bounds.extents.y / itemSize.y;
			var ratioZ = 2*bounds.extents.z / itemSize.z;

			var maxDim = Mathf.Max (
				             ratioX, ratioY, ratioZ
			             );

			if (maxDim > 1) {
				Vector3 scale;
				if (keepAspectRatio) {
					scale = Vector3.one / maxDim;
				} else {
					scale = new Vector3 (
						Mathf.Max (1, ratioX),
						Mathf.Max (1, ratioY),
						Mathf.Max (1, ratioZ)
					);
				}
				meshFilter.transform.localScale = scale;
			}
		}
	}

	void Process (SpriteRenderer spriteRenderer)
	{
		// TODO!
	}

}
