using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour {

	public float jumpStrengthMin = 1;
	public float jumpStrengthMax = 50;
	public float jumpAngle = 45;
	public float deathDepth = -100;
	public float jumpStrengthFrequency = 1;
	public int score = -1;
	public Text scoreText;

	float deathDepthDelta;
	internal float jumpStrength = 0;
	int collisionCount = 0;
	float startTime;


	bool hasJumpingIntent;
	Rigidbody body;
	PlatformCreator platformCreator;

	void Awake() {
		body = GetComponent<Rigidbody> ();
		platformCreator = GetComponent<PlatformCreator> ();
		deathDepthDelta = deathDepth;
	}

	public bool CanJump {
		get { 
			return collisionCount > 0;
		}
	}

	public float RealJumpStrength {
		get {
			return jumpStrengthMin + (jumpStrengthMax - jumpStrengthMin) * jumpStrength;
		}
	}

	public void Jump() {
		hasJumpingIntent = false;
		var rot = Quaternion.AngleAxis (-jumpAngle, transform.right);
		var direction = rot * transform.forward;

		var force = direction * RealJumpStrength;

		body.AddForce (force, ForceMode.VelocityChange);
	}

	// Update is called once per frame
	void Update () {
		if (!hasJumpingIntent) {
			if (Input.GetKeyDown (KeyCode.Space)) {
				startTime = Time.time;
			}
			if (Input.GetKey (KeyCode.Space)) {
				// keep changing jumpStrength
				jumpStrength = 1 - (Mathf.Cos ((Time.time - startTime) * Mathf.PI * jumpStrengthFrequency) + 1) / 2;	// between 0 and 1
			}
			if (Input.GetKeyUp (KeyCode.Space)) {
				hasJumpingIntent = true;
			}
		}

		if (Input.GetKey (KeyCode.R) || transform.position.y < deathDepth) {
			// reset scene
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}

		scoreText.text = score.ToString();
	}

	void FixedUpdate() {
		if (CanJump && hasJumpingIntent) {
			Jump ();
		}
	}

	void OnCollisionEnter(Collision collision) {
		var platform = collision.gameObject.GetComponent<Platform> ();
		if (!platform) {
			// we only care about platforms
			return;
		}

		++collisionCount;

		if (!platform.reached) {
			if (!hasJumpingIntent) {
				jumpStrength = 0;
			}
			++score;
			platform.reached = true;
			platformCreator.SpawnPlatform ();

			deathDepth = Mathf.Min (deathDepth, platform.transform.position.y + deathDepthDelta);
		}
	}

	void OnCollisionExit(Collision collision) {
		var platform = collision.gameObject.GetComponent<Platform> ();
		if (!platform) {
			// we only care about platforms
			return;
		}

		--collisionCount;
	}
}
