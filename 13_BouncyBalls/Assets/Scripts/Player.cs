using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	public float speed = 100;
	public float jumpStrength = 100;
	public float compressionStrength = 1000;
	public Platform platform;
	public int groundColliders;
	public string collisionLayer = "Environment";

	float upDown, leftRight;

	internal int layerMask;

	// Use this for initialization
	void Start () {
		layerMask = LayerMask.NameToLayer(collisionLayer);
	}

	void Update() {
		leftRight = Input.GetAxisRaw ("Horizontal");
		upDown = Input.GetAxisRaw ("Vertical");
		if (upDown > 0 && !Input.GetKeyDown(KeyCode.W) && !Input.GetKeyDown(KeyCode.UpArrow)) {
			// only jump when button is pressed (else, can too easily accelerate along walls)
			upDown = 0;
		}
		//var body = GetComponent<Rigidbody2D> ();
		//var v = body.velocity;
	}

	void FixedUpdate () {
		var body = GetComponent<Rigidbody2D> ();
		var v = body.velocity;

		v.x = leftRight * speed * Time.fixedDeltaTime;


		if (upDown > 0 && groundColliders > 0) {
			v.y = jumpStrength;
			upDown = 0;
		}

		body.velocity = v;

		if (upDown < 0) {
			
			for (var i = 0; i < transform.childCount; ++i) {
				var child = transform.GetChild (i);
				var childBody = child.GetComponent<Rigidbody2D> ();
				if (childBody) {
					var dir = child.transform.localPosition.y > 0 ? Vector2.down : Vector2.up;
					childBody.AddForce (dir * compressionStrength * Time.fixedDeltaTime, ForceMode2D.Force);
				}
			}
//			body.AddForce (Vector2.down * compressionStrength, ForceMode2D.Impulse);
//			body.AddForce (Vector2.up * compressionStrength, ForceMode2D.Impulse);
		}

//		if (jumping) {
//			var childForce = jumpStrength/transform.childCount;
//
//			for (var i = 0; i < transform.childCount; ++i) {
//				var child = transform.GetChild (i);
//				body.AddForceAtPosition (Vector2.up * childForce, child.position, ForceMode2D.Impulse);
//			}
//			jumping = false;
//		}

//		if (v.x != 0) {
//			var dir = Mathf.Sign (v.x);
//			transform.rotation = Quaternion.Euler (0, 90 - dir * 90, 0);
//		}
	}
}
