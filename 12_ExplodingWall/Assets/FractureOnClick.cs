using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent (typeof(Collider))]
public class FractureOnClick : MonoBehaviour
{
	public float debrisGap = .1f;
	// gap between pieces of debris
	public MeshFilter debrisPrefab;

	public float explosionPower = 100;
	public float explosionUpwardModifier = 5;

	public float debrisExpirationTime = 5;

	/// <summary>
	/// Object that contains all fractured pieces
	/// </summary>
	public Transform debrisParent;

	new Collider collider;

	Vector3 debrisSize;

	void Awake ()
	{
		debrisSize = Vector3.Scale (2 * debrisPrefab.sharedMesh.bounds.extents, debrisPrefab.transform.lossyScale);
	}

	Collider Coll { 
		get {
			if (collider == null) {
				collider = GetComponent<Collider> ();
			}
			return collider;
		}
	}

	public int ComputeFractureCount ()
	{
		var coll = Coll;
		var ext = coll.bounds.extents;

		debrisSize = Vector3.Scale (2 * debrisPrefab.sharedMesh.bounds.extents, debrisPrefab.transform.lossyScale);

		// TODO: inaccurate
		return Mathf.CeilToInt (ext.x * ext.y * ext.z * 6 / (debrisSize.x * debrisSize.y * debrisSize.z));
	}

	void OnMouseDown ()
	{
		var sourcePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		BreakApart (sourcePos);
	}

	void BreakApart (Vector3 sourcePos)
	{
		var coll = Coll;
		var ext = coll.bounds.extents;

		var r = Mathf.Max (Vector3.Distance (sourcePos, coll.bounds.min), Vector3.Distance (sourcePos, coll.bounds.max));

		var dx = Mathf.CeilToInt (2 * ext.x / (debrisSize.x + debrisGap));
		var dy = Mathf.CeilToInt (2 * ext.y / (debrisSize.y + debrisGap));
		var dz = Mathf.CeilToInt (2 * ext.z / (debrisSize.z + debrisGap));

		// min 
		var p0 = coll.bounds.min;
		Vector3 p;

		//var mat = new Material(debrisPrefab.GetComponent<MeshRenderer> ().sharedMaterial);
		var mat = GetComponent<MeshRenderer>().material;

		if (debrisParent == null) {
			debrisParent = new GameObject ("Debris (" + gameObject.name + ")").transform;
			if (debrisExpirationTime > 0) {
				Destroy (debrisParent.gameObject, debrisExpirationTime);
			}
		}

		for (var k = 0; k < dz; ++k) {
			p.z = p0.z + k * (debrisSize.z + debrisGap);
			for (var j = 0; j < dy; ++j) {
				p.y = p0.y + j * (debrisSize.y + debrisGap);
				for (var i = 0; i < dx; ++i) {
					p.x = p0.x + i * (debrisSize.x + debrisGap);

					if (coll.bounds.Contains (p)) {
						// create new piece of debris
						var piece = Instantiate (debrisPrefab, p, transform.rotation, debrisParent);
						piece.GetComponent<MeshRenderer> ().material = mat;

						var body = piece.GetComponent<Rigidbody> ();
						if (body) {
							body.AddExplosionForce (explosionPower, sourcePos, r, explosionUpwardModifier, ForceMode.Impulse);
						}


						if (debrisExpirationTime != 0) {
							// animate + destroy debris after some time
							Destroy (piece, debrisExpirationTime);

							var anim = piece.GetComponent<Animator> ();

							if (anim) {
								anim.speed = 1 / debrisExpirationTime;
								//anim.SetFloat("FadeTime", debrisExpirationTime);
								anim.SetTrigger ("Fade");
							}

							if (piece.GetComponent<FadeOut> ()) {
								piece.GetComponent<FadeOut> ().StartFade (debrisExpirationTime);
							}
						}
					}
				}
			}
		}

		// destroy original object
		Destroy (gameObject);
	}
}


#if UNITY_EDITOR
[CustomEditor (typeof(FractureOnClick))]
public class FractureOnClickEditor : Editor
{
	void OnEnable ()
	{
	}

	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();
		var t = (FractureOnClick)target;
		GUILayout.Label ("FractureCount: " + t.ComputeFractureCount ());
	}
}
#endif