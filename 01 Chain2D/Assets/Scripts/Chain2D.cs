using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chain2D : MonoBehaviour {
	static readonly Bounds emptyBounds = new Bounds ();

	public int linkCount = 10;
	public bool enableCollisions = true;
	public float linkDist = .1f;
	public Rigidbody2D hookPrefab;
	public Rigidbody2D linkPrefab;
	public Rigidbody2D weightPrefab;

	Bounds GetBounds(Rigidbody2D go) {
		var collider = go.GetComponent<Collider2D> ();
		if (collider) {
			return collider.bounds;
		}
		return emptyBounds;
	}

	C GetOrCreate<C>(GameObject go) 
		where C : Component
	{
		var joint = go.GetComponent<C> ();
		if (!joint) {
			joint = go.AddComponent<C> ();
		}

		return joint;
	}

	void ConnectToJoint(HingeJoint2D joint, Rigidbody2D connected) {
		joint.connectedBody = connected;
		joint.autoConfigureConnectedAnchor = true;
		joint.enableCollision = enableCollisions;

		var anchorY = 1/(2*joint.transform.lossyScale.y) * linkDist + .5f;
		joint.anchor = new Vector2 (0, anchorY);
	}

	void ConnectToJoint(DistanceJoint2D joint, Rigidbody2D connected) {
		joint.connectedBody = connected;
		joint.autoConfigureDistance = true;
		joint.autoConfigureConnectedAnchor = true;
		joint.enableCollision = enableCollisions;
		joint.maxDistanceOnly = true;

		var anchorY = 1/(2*joint.transform.lossyScale.y) * linkDist + .5f;
		joint.anchor = new Vector2 (0, anchorY);
	}

	Rigidbody2D Connect(Rigidbody2D prefab, Rigidbody2D previous, ref Vector2 pos) {
		var link = Instantiate<Rigidbody2D>(prefab, transform);
		//print (GetBounds (link).extents.y +" + " + GetBounds (previous).extents.y +" + " + linkDist);
		pos.y -= GetBounds(link).extents.y + GetBounds(previous).extents.y + linkDist;
		link.transform.SetPositionAndRotation (pos, prefab.transform.localRotation);
		link.transform.localScale = prefab.transform.localScale;
		var joint1 = GetOrCreate<HingeJoint2D> (link.gameObject);
		ConnectToJoint (joint1, previous);

		var joint2 = GetOrCreate<DistanceJoint2D> (link.gameObject);
		ConnectToJoint (joint2, previous);
		return link;
	}

	public void Clear() {
		#if UNITY_EDITOR
		UnityEditor.Undo.RecordObject (gameObject, "Cleared chain");
		#endif

		for (var i = transform.childCount-1; i >= 0; --i) {
			DestroyImmediate (transform.GetChild(i).gameObject);
		}

		#if UNITY_EDITOR
		UnityEditor.Undo.RegisterCompleteObjectUndo(gameObject, "Cleared chain");

		UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
		#endif
	}

	public void Build() {
		Clear ();

		var pos = new Vector2(transform.position.x, transform.position.y);
		var hook = Instantiate<Rigidbody2D> (hookPrefab, pos, hookPrefab.transform.localRotation, transform);
		Rigidbody2D previous = hook;

		for (var i = 0; i < linkCount; ++i) {
			previous = Connect(linkPrefab, previous, ref pos);
		}

		Connect(weightPrefab, previous, ref pos);

		#if UNITY_EDITOR
		UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
		#endif
	}
}
