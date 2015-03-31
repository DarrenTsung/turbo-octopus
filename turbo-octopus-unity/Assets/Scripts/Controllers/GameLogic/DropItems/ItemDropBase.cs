using UnityEngine;
using System.Collections;

public class ItemDropBase : MonoBehaviour {

	protected bool movingToPlayer;
	protected float movingSpeed, acceleration;

	protected Rigidbody2D myRigidbody;

	protected virtual void Start () {
		movingToPlayer = false;
		movingSpeed = 0.0f;
		acceleration = 1.0f;

		myRigidbody = GetComponent<Rigidbody2D> ();
	}
	
	protected virtual void Update () {
		Vector2 playerPosition = GameUtils.PlayerInstance().transform.position;
		Vector2 myPosition = transform.position;
		
		if (Vector2.Distance(playerPosition, myPosition) <= PlayerManager.magnetDistance) {
			movingToPlayer = true;
		}

		if (movingToPlayer) {
			movingSpeed += acceleration * Time.deltaTime;
			Vector2 newPosition = (Vector2)transform.position + (movingSpeed * (playerPosition - myPosition).normalized);
			transform.position = newPosition;
		}

		Collider2D[] emptyArray = new Collider2D[1];
		int hitPlayer = Physics2D.OverlapCircleNonAlloc(transform.position, 0.3f, emptyArray, GameUtils.playerLayer);
		if (hitPlayer > 0) {
			HandleHitPlayer();
			Destroy(gameObject);
		}
	}

	protected virtual void HandleHitPlayer() {
	}
}
