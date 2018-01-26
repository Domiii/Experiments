using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wasabimole.ProceduralTree;

public class TreeGen : MonoBehaviour, IMeshGenerator {
	public MeshFilter GenerateMesh(int seed) {
		var go = new GameObject (string.Format ("Tree_{0:X4}", seed));
		var procTree = go.AddComponent<ProceduralTree>();
		procTree.RandomizeTree (seed);

		return procTree.GetComponent<MeshFilter>();
	}
}
