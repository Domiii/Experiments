using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class FadeOut : MonoBehaviour {
	float timeLeft;
	float fadeRate;

	// Update is called once per frame
	public void StartFade (float fadeTime = 1) {
		timeLeft = fadeTime;
		fadeRate = 1 / fadeTime;
	}

	void Update() {
		if (timeLeft > 0) {
			timeLeft -= Time.deltaTime;
			//var a = Mathf.Sqrt(timeLeft * fadeRate);
			var a = timeLeft * fadeRate;
			if (a < 0) {
				a = 0.001f;
			}
			//print (a);
			var c = GetComponent<MeshRenderer> ().material.color;
			c.a = a;
			GetComponent<MeshRenderer> ().material.color = c;

			if (timeLeft <= 0) {
				// time's up!
				Destroy (gameObject);
			}
		}
	}

}
