using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour {
	public string objectName = "Wall";
	public GameObject prefab;
	public Vector3 minScale = new Vector3 (5, 1, 1);
	public Vector3 maxScale = new Vector3(20, 20, 1);
	public float spawnDelay = 1;

	bool spawning;

	void Update () {
		var wall = GameObject.Find (objectName);

		if (wall == null && !spawning) {
			// spawn new wall!
			spawning = true;
			Invoke ("SpawnWall", spawnDelay);
		}
	}

	IEnumerator SpawnWall() {
		var rand = new Vector3 (Random.value, Random.value, Random.value);
		var s = minScale + Vector3.Scale (rand, (maxScale - minScale));

		var pos = Vector3.up * s.y/2;
		var go = (GameObject)Instantiate (prefab, pos, Quaternion.identity, transform);
		go.name = objectName;

		// random size
		go.transform.localScale = s;

		// random color
		go.GetComponent<MeshRenderer> ().material.color = Random.ColorHSV ();


		spawning = false;

		return null;
	}
}
