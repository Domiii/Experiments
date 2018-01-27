using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotate : MonoBehaviour {
	public float speed = 30;
	public Vector3 axis = Vector3.up;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate (axis, speed * Time.deltaTime);
	}
}
