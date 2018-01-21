//
// Boids - Flocking behavior simulation.
//
// Copyright (C) 2014 Keijiro Takahashi
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using UnityEngine;
using System.Collections;

public class BoidController : MonoBehaviour
{
	public GameObject boidPrefab;

	public int spawnCount = 10;

	public float spawnRadius = 4.0f;

	//[Range(0.1f, 20.0f)]
	//public float speed = 6.0f;

	//	[Range(0.0f, 0.9f)]
	//	public float velocityVariation = 0.5f;
	//
	//	[Range(0.1f, 20.0f)]
	//	public float rotationCoeff = 4.0f;

	[Range(0.1f, 1000.0f)]
	public float maxSpeed = 20.0f;

	[Range(0.01f, 10.0f)]
	public float minSpeed = .2f;

	[Range(0.1f, 1000.0f)]
	public float maxForce = 20.0f;

	[Range(0.1f, 20.0f)]
	public float neighborDist = 2.0f;

	[Range(-1000, 1000)]
	public float targetPull = 1;

	public float separationWeight = 1.5f;

	public float alignmentWeight = 1;

	public float cohesionWeight = 1;

	/// <summary>
	/// To reduce risk of boids crashing into each other
	/// </summary>
	public float cohesionDamping = 1;

	public Transform target;

	public LayerMask searchLayer;

	void Reset() {
		searchLayer = LayerMask.NameToLayer ("Everything");
	}

	void Start()
	{
		if (target == null) {
			target = transform;
		}
		SpawnFlock ();
	}

	void Update() {
		if (Input.GetKeyDown (KeyCode.Space)) {
			SpawnFlock ();
		}

		//Debug.DrawRay(targetPos, Vector3.up, Color.red, 100.0f);
	}

	void SpawnFlock() {
		for (var i = 0; i < spawnCount; i++) 
			SpawnOne();
	}

	public GameObject SpawnOne()
	{
		return SpawnOne(transform.position + Random.insideUnitSphere * spawnRadius);
	}

	public GameObject SpawnOne(Vector3 position)
	{
		var rotation = Quaternion.Slerp(transform.rotation, Random.rotation, 0.3f);
		var boid = Instantiate(boidPrefab, position, rotation) as GameObject;
		boid.GetComponent<BoidBehavior>().controller = this;
		return boid;
	}
}
