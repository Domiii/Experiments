using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent (typeof(Rigidbody2D))]
public class Player : MonoBehaviour {
	public float speed = 6;
	public float jumpStrength = 20;
	public Transform feet;

//	public Collider2D collider;
//	Bounds bounds;

	float gravity = 0;
	Planet planet;
	Rigidbody2D body;
	bool lookingRight = true;

	Vector2 move;
	bool wantsToJump = false;

	void Start () {
		body = GetComponent<Rigidbody2D> ();
		//bounds = collider.bounds;
	}

	void Update () {
		move.x = Input.GetAxisRaw ("Horizontal");
		move.y = Input.GetAxisRaw ("Vertical");

		if (planet != null) {
			if (Input.GetKeyDown (KeyCode.Space)) {
				wantsToJump = true;
			}
		}
	}

	Vector3 GetBestUp() {
		Vector3 up;
		if (!planet) {
			up = transform.up;
		} else {
			up = transform.position - planet.transform.position;
		}
		up.Normalize ();
		return up;
	}

	void FixedUpdate () {
		if (planet != null) {
			if (move.x != 0) {
				if (lookingRight != (move.x > 0)) {
					// flip forward direction
					lookingRight = move.x > 0;

					//var dir = Mathf.Sign (move.x);
					//transform.rotation = Quaternion.Euler (0, 90 - dir * 90, transform.rotation.eulerAngles.z);
				}
				//move.x = Mathf.Abs (move.x);

				//var dir = transform.right + 1 * transform.up;
				var up = GetBestUp();

				// The cross product of two vectors gives you the vector orthogonal to both
				// https://answers.unity.com/questions/10323/calculating-a-movement-direction-that-is-a-tangent.html
				var dir = Vector3.Cross (up, Vector3.forward);
				dir.Normalize ();

				var delta = (move.x * speed * Time.fixedDeltaTime) * dir;
				//body.MovePosition (transform.position + delta);
				body.velocity = delta;
			} else {
				//body.velocity = Vector2.zero;
			}
			if (wantsToJump) {
				var up = GetBestUp();
				body.AddForce (up * jumpStrength, ForceMode2D.Impulse);
				wantsToJump = false;
			}
		}
	}

	void LandedOnPlanet (Planet planet) {
		this.planet = planet;
		print ("landed");
		body.velocity = Vector2.zero;
		body.angularVelocity = 0;
	}

	void LeftPlanet (Planet planet) {
		print ("left");
		if (this.planet == planet) {
			this.planet = null;	
		}
	}
}
