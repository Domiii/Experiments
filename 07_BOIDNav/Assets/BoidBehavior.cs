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
using UnityEngine.AI;
using System.Collections;

/// <summary>
/// Original source: https://github.com/keijiro/Boids
/// Changed based on: http://harry.me/blog/2011/02/17/neat-algorithms-flocking/
/// </summary>
[RequireComponent (typeof(NavMeshAgent))]
public class BoidBehavior : MonoBehaviour
{
	// Reference to the controller.
	public BoidController controller;

	// Options for animation playback.
	public float animationSpeedVariation = 0.2f;

	public Vector3 velocity, a;

	// Random seed.
	float noiseOffset;

	NavMeshAgent navMeshAgent;

	// Caluculates the separation vector with a target.
	Vector3 GetSeparationVector (Transform target)
	{
		var diff = transform.position - target.transform.position;
		var dist = diff.magnitude;
		if (dist < controller.neighborDist/2) {
			// self
			return Vector3.zero;
		}
		var scaler = Mathf.Clamp01 (1.0f - dist / controller.neighborDist);
		return diff * (scaler / dist);
	}

	void Start ()
	{
		noiseOffset = Random.value * 10.0f;

		var animator = GetComponent<Animator> ();
		if (animator)
			animator.speed = Random.Range (-1.0f, 1.0f) * animationSpeedVariation + 1.0f;

		var v = Random.insideUnitCircle * controller.maxSpeed / 2;
		velocity = new Vector3 (v.x, 0, v.y);

		navMeshAgent = GetComponent < NavMeshAgent > ();
	}

	void LimitV (ref Vector3 v, float max)
	{
		if (v.sqrMagnitude > max * max) {
			v.Normalize ();
			v *= max;
		}
	}

	void Update ()
	{
		navMeshAgent.speed = controller.maxSpeed;
		navMeshAgent.destination = controller.target.position;
		var dt = Time.deltaTime;

		var currentPosition = transform.position;
		var currentRotation = transform.rotation;

		// Current velocity randomized with noise.
		//var noise = Mathf.PerlinNoise (Time.time, noiseOffset) * 2.0f - 1.0f;
		//var velocity = controller.speed * (1.0f + noise * controller.velocityVariation);

		// Initializes the vectors.
		var separation = Vector3.zero;
		var alignment = transform.forward;
		var cohesion = currentPosition;

		// Looks up nearby boids.
		var nearbyBoids = Physics.OverlapSphere (currentPosition, controller.neighborDist, controller.searchLayer);

		// Accumulates the vectors.
		foreach (var boid in nearbyBoids) {
			if (boid.gameObject == gameObject)
				continue;
			var t = boid.transform;
			separation += GetSeparationVector (t);
			alignment += t.forward;
			cohesion += t.position;
		}

		var nearbyCount = nearbyBoids.Length;

		var totalWeight = (nearbyCount);
		if (totalWeight > 0) {
			var avg = 1.0f / totalWeight;
			alignment *= avg;
			cohesion *= avg;
		}

		cohesion = (cohesion - currentPosition);
		var targetDist = cohesion.magnitude;
		if (targetDist > 0) {
			cohesion.Normalize ();

			if (targetDist < controller.cohesionDamping) {
				cohesion *= controller.maxSpeed * targetDist / controller.cohesionDamping;
			} else {
				cohesion *= controller.maxSpeed;
			}

			// don't get too close
			var dp0 = velocity * dt;
			if (cohesion.sqrMagnitude > dp0.sqrMagnitude) { 
				cohesion = cohesion - dp0;
			} else {
				cohesion.Set (0, 0, 0);
			}
		}

		separation *= controller.separationWeight;
		alignment *= controller.alignmentWeight;
		cohesion *= controller.cohesionWeight;

		// Calculates a rotation from the vectors.
		//        var direction = separation + alignment + cohesion;
		//        var rotation = Quaternion.FromToRotation(Vector3.forward, direction.normalized);
		//
		//        // Applys the rotation with interpolation.
		//        if (rotation != currentRotation)
		//        {
		//            var ip = Mathf.Exp(-controller.rotationCoeff * Time.deltaTime);
		//            transform.rotation = Quaternion.Slerp(rotation, currentRotation, ip);
		//        }

		// compute acceleration
		a = separation + alignment + cohesion;
		a += (controller.target.position - transform.position).normalized * controller.targetPull;

		if (a.y != 0) {
			// project onto x/z plane, keep magnitude
			var aLen = a.magnitude;
			a.y = 0;
			a *= aLen;
		}
		LimitV (ref a, controller.maxForce);

		// compute velocity
		velocity += a * dt;
		LimitV (ref velocity, controller.maxSpeed);

		// move position
		if (velocity.magnitude >= controller.minSpeed) {
 			var dp = velocity * dt;
//			transform.position = currentPosition + dp;
			var targetPos = currentPosition + dp;
			NavMeshHit hit;
			if (NavMesh.SamplePosition (targetPos, out hit, 2, NavMesh.AllAreas)) {
				//navMeshAgent.destination = hit.position;
				transform.position = currentPosition + dp;
			}
			//transform.forward = velocity.normalized;

		} else {
			// to prevent stable-state oscillation caused by discretization, don't move if force is not strong enough
			//transform.forward = (transform.forward*.9f +  alignment*.1f).normalized;
		}
	}
}
