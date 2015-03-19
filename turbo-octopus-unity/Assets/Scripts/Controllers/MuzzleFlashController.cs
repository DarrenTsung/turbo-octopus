using UnityEngine;
using System.Collections;

public class MuzzleFlashController : MonoBehaviour {

	protected SpriteRenderer spriteRenderer;
	protected Light light;

	protected float visibleTime = 0.0f;

	void Start () {
		spriteRenderer = GetComponent<SpriteRenderer> ();
		light = transform.Find ("Light").gameObject.GetComponent<Light> ();
	}

	void Update () {
		visibleTime -= Time.deltaTime;
		if (spriteRenderer.enabled && visibleTime < 0.0f) {
			spriteRenderer.enabled = false;
		}
		if (light.enabled && visibleTime < 0.0f) {
			light.enabled = false;
		}
	}

	public void MuzzleFlash (float time) {
		spriteRenderer.enabled = true;
		light.enabled = true;
		visibleTime = time;
	}
}
