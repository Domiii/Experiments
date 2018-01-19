using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JustFly : MonoBehaviour {
	public float speed = 10;
	public float rotationVariance = 10;

	Vector3 velocity;

	
	void Update () {
		var dir = Random.onUnitSphere;
		//var rot = Quaternion.RotateTowards (transform.rotation, Quaternion.LookRotation(dir, transform.up), rotationVariance * Time.deltaTime);
		var rot = Quaternion.RotateTowards (transform.rotation, transform.rotation, rotationVariance * Time.deltaTime);
		transform.forward = rot * transform.forward;
		velocity = transform.forward * speed;

		transform.position += transform.forward * speed * Time.deltaTime;
	}
}
