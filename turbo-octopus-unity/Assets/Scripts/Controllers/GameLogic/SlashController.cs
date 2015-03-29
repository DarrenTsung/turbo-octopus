using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SlashController : MonoBehaviour {

	protected BoxCollider2D boxCollider;

	protected HashSet<int> objectsBeingHit;
	protected PlayerController parentController;
	protected DamageController damageController;
	protected int comboLevel;

	void Awake () {
		boxCollider = GetComponent<BoxCollider2D> ();
		damageController = GetComponent<DamageController> ();
		objectsBeingHit = new HashSet<int>();
	}

	public void SetParentController(PlayerController controller) {
		parentController = controller;
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

			Rigidbody2D enemyRigidbody = otherObject.GetComponent<Rigidbody2D> ();
			if (enemyRigidbody && !enemyController.IsDying()) {
				Vector3 direction = boxCollider.transform.right;
				if (!parentController.isFacingRight()) {
					direction.x *= -1;
				}
				enemyRigidbody.velocity = new Vector2(0.0f, 0.0f);
				enemyRigidbody.AddForce(direction * 300.0f);
			}
		}

		boxCollider.enabled = false;
	}

	public void SetComboLevel(int comboLevel) {
		this.comboLevel = comboLevel;
		damageController.SetComboMultiplier(comboLevel);
	}

	public void EndSlash() {
		objectsBeingHit.Clear();
	}

	public bool HitAnything() {
		return objectsBeingHit.Count > 0;
	}
}
