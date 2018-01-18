using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ComponentUtil {
	public static C GetComponentInHierarchy<C>(this Transform t)
		where C : Component
	{
		var c = t.GetComponent<C> ();
		if (!c && t.parent) {
			return t.parent.GetComponentInHierarchy<C> ();
		}
		return c;
	}
}

[RequireComponent(typeof(SpriteRenderer))]
public class Planet : MonoBehaviour {
	/// <summary>
	// Gravity influences objects up to x times the radius above the planet surface
	/// </summary>
	public float gravityHeight = 1;

	/// <summary>
	/// Gravity strength on surface (scales to 0 at end of gravity influence zone)
	/// </summary>
	public float gravityStrength = 1f;

	new SpriteRenderer renderer;
	float radius;
	Collider2D[] colliders = new Collider2D[64];

	void Start () {
		renderer = GetComponent<SpriteRenderer>();
		radius = renderer.bounds.extents.x;
	}

	void Update () {
		var count = Physics2D.OverlapCircleNonAlloc (transform.position, radius * (1 + gravityHeight), colliders);

		for (var i = 0; i < count; ++i) {
			var collider = colliders [i];
			var body = collider.transform.GetComponentInHierarchy<Rigidbody2D> ();
			if (body) {
				Attract (body);
			}
		}
	}

	Vector3 GetRealPosition(Transform t) {
		var player = t.GetComponent<Player> ();
		if (player) {
			return player.feet.position;
		}
		return t.position;
	}

	Planet GetPlanet(Transform t) {
		if (t.parent) {
			var planet = t.parent.GetComponent<Planet> ();
			return planet;
		}
		return null;
	}

	public bool IsOnPlanet(Transform t) {
		return t == transform || GetPlanet(t) != null;
	}

	void Attract(Rigidbody2D body) {
		if (body.gameObject == gameObject)
			return;	// don't attract self or objects already "on planet"

		var targetPos = GetRealPosition (body.transform);
		var dir = transform.position - targetPos;

		// simplistic gravity scales linearly from surface to gravitational horizon
		var dist = dir.magnitude - radius;
		var maxDist = gravityHeight * radius;
		var distanceFactor = (maxDist - dist) / maxDist;

		distanceFactor = distanceFactor * distanceFactor;

		if (distanceFactor < 0) {
			// something went wrong here
			return;
		}


		dir.Normalize ();

		//var coll = body.GetComponent<Collider2D> ();
		//var pos = coll.bounds.center - new Vector3(0, -coll.bounds.extents.y, 0);
		body.AddForceAtPosition(dir * gravityStrength * distanceFactor * Time.deltaTime, targetPos, ForceMode2D.Force);

	}

	void OnTriggerEnter2D(Collider2D other) {
		var body = other.transform.GetComponentInHierarchy<Rigidbody2D> ();
		if (body && GetPlanet(body.transform) != this) {
			body.transform.SetParent (transform, true);
			body.SendMessage ("LandedOnPlanet", this, SendMessageOptions.DontRequireReceiver);
		}
	}

	void OnTriggerExit2D(Collider2D other) {
		var body = other.transform.GetComponentInHierarchy<Rigidbody2D> ();
		if (body && GetPlanet(body.transform) == this) {
			body.transform.SetParent (null, true);
			body.SendMessage ("LeftPlanet", this, SendMessageOptions.DontRequireReceiver);
		}
	}
}
