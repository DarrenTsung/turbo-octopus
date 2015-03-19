using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SlashController : MonoBehaviour {

	protected BoxCollider2D boxCollider;

	protected HashSet<int> objectsBeingHit;

	void Awake () {
		boxCollider = GetComponent<BoxCollider2D> ();
		objectsBeingHit = new HashSet<int>();
	}

	protected void OnTriggerEnter2D(Collider2D collider) {
		GameObject otherObject = collider.gameObject;

		int instanceId = otherObject.GetInstanceID();

		// make sure we aren't hitting this object already in the trigger lifetime
		if (objectsBeingHit.Contains(instanceId)) {
			return;
		} else {
			objectsBeingHit.Add(instanceId);
		}

		EnemyController enemyController = otherObject.GetComponent<EnemyController> ();
		if (enemyController) {
			Vector2 approximatedCollisionPoint = otherObject.transform.position + new Vector3(Random.value * 0.5f, Random.value * 0.5f, 0.0f);
			enemyController.OnHit(gameObject, approximatedCollisionPoint);
		}

		boxCollider.enabled = false;
	}

	public void EndSlash() {
		objectsBeingHit.Clear();
	}
}
