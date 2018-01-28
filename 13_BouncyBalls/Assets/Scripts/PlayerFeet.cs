using UnityEngine;
using System.Collections;

public class PlayerFeet : MonoBehaviour {
	public Player player;

	void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject != player.gameObject && other.transform.parent.gameObject != player.gameObject) {
			++player.groundColliders;
		}
	}

	void OnTriggerStay2D(Collider2D other) {
		// check if play is on platform
		var triggerPlatform = other.gameObject.GetComponentInParent<Platform> ();
		if (triggerPlatform != null) {
			player.platform = triggerPlatform;
			player.transform.parent = player.platform.transform;
		}
	}

	void OnTriggerExit2D(Collider2D other) {
		//if (other.gameObject.layer == player.layerMask) {
		if (other.gameObject != player.gameObject && other.transform.parent.gameObject != player.gameObject) {
			--player.groundColliders;
		}
		//}

		var triggerPlatform = other.gameObject.GetComponentInParent<Platform> ();
		if (triggerPlatform != null && triggerPlatform == player.platform) {
			// player left platform
			player.platform = null;
			player.transform.parent = null;
		}
	}
}