using UnityEngine;
using System.Collections;

public class GoldController : MonoBehaviour {

	public int baseValue;

	protected void Update() {
		Collider2D[] emptyArray = new Collider2D[1];
		int hitPlayer = Physics2D.OverlapCircleNonAlloc(transform.position, 0.3f, emptyArray, GameUtils.playerLayer);
		if (hitPlayer > 0) {
			PlayerManager.Instance.gold += baseValue;
			Destroy(gameObject);
		}
	}
}
