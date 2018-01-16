using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformCreator : MonoBehaviour {
	public float minRadius = 30;
	public float maxRadius = 40;
	public float minHeight = -5;
	public float maxHeight = 5;
	public GameObject platformPrefab;
	public Transform platformGroup;


	public void SpawnPlatform() {
		var pos = transform.position + Vector3.forward * Random.Range(minRadius, maxRadius);
		pos.y += Random.Range (minHeight, maxHeight);

		var platform = Instantiate (platformPrefab, pos, Quaternion.identity, platformGroup);
	}

	// Use this for initialization
	void Awake () {
		//SpawnPlatform ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}


}
