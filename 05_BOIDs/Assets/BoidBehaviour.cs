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

/// <summary>
/// Original source: https://github.com/keijiro/Boids
/// Changed based on: http://harry.me/blog/2011/02/17/neat-algorithms-flocking/
/// </summary>
public class BoidBehaviour: MonoBehaviour {
 // Reference to the controller.
 public BoidController controller;

 // Options for animation playback.
 public float animationSpeedVariation = 0.2 f;

 public Vector3 velocity;

 // Random seed.
 float noiseOffset;

 // Caluculates the separation vector with a target.
 Vector3 GetSeparationVector(Transform target) {
  var diff = transform.position - target.transform.position;
  var dist = diff.magnitude;
  if (dist == 0) {
   // self
   return Vector3.zero;
  }
  var scaler = Mathf.Clamp01(1.0 f - dist / controller.neighborDist);
  return diff * (scaler / dist);
 }

 void Start() {
  noiseOffset = Random.value * 10.0 f;

  var animator = GetComponent < Animator > ();
  if (animator)
   animator.speed = Random.Range(-1.0 f, 1.0 f) * animationSpeedVariation + 1.0 f;

  velocity = Random.insideUnitSphere * controller.maxSpeed / 2;
 }

 void LimitV(ref Vector3 v, float max) {
  if (v.sqrMagnitude > max * max) {
   v.Normalize();
   v *= max;
  }
 }

 void Update() {
  var currentPosition = transform.position;
  var currentRotation = transform.rotation;

  // Current velocity randomized with noise.
  var noise = Mathf.PerlinNoise(Time.time, noiseOffset) * 2.0 f - 1.0 f;
  //var velocity = controller.speed * (1.0f + noise * controller.velocityVariation);

  // Initializes the vectors.
  var separation = Vector3.zero;
  var alignment = transform.forward;
  var cohesion = controller.transform.position * controller.controllerPull;

  // Looks up nearby boids.
  var nearbyBoids = Physics.OverlapSphere(currentPosition, controller.neighborDist, controller.searchLayer);

  // Accumulates the vectors.
  foreach(var boid in nearbyBoids) {
   if (boid.gameObject == gameObject) continue;
   var t = boid.transform;
   separation += GetSeparationVector(t);
   alignment += t.forward;
   cohesion += t.position;
  }

  var totalWeight = nearbyBoids.Length + controller.controllerPull;
  if (totalWeight > float.Epsilon) {
   var avg = 1.0 f / totalWeight;
   alignment *= avg;
   cohesion *= avg;
  }

  cohesion = (cohesion - currentPosition);
  var targetDist = cohesion.magnitude;
  if (targetDist > 0) {
   cohesion.Normalize();

   if (targetDist < controller.cohesionDamping) {
    cohesion *= controller.maxSpeed * targetDist / controller.cohesionDamping;
   } else {
    cohesion *= controller.maxSpeed;
   }
   cohesion = cohesion - velocity;
  }

  LimitV(ref cohesion, controller.maxForce);
  LimitV(ref alignment, controller.maxForce);

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

  // Moves forawrd.
  var dt = Time.deltaTime;
  var a = separation + alignment + cohesion;
  velocity += a * dt;
  LimitV(ref velocity, controller.maxSpeed);

  transform.position = currentPosition + velocity * dt;
  transform.forward = velocity.normalized;
 }
}
