using UnityEngine;
using System.Collections;

public class MuzzleFlashController : MonoBehaviour {

	protected SpriteRenderer spriteRenderer;

	protected float visibleTime = 0.0f;

	void Start () {
		spriteRenderer = GetComponent<SpriteRenderer> ();
	}

	void Update () {
		visibleTime -= Time.deltaTime;
		if (spriteRenderer.enabled && visibleTime < 0.0f) {
			spriteRenderer.enabled = false;
		}
	}

	public void MuzzleFlash (float time) {
		spriteRenderer.enabled = true;
		visibleTime = time;
	}
}
