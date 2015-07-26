using UnityEngine;
using System.Collections;

public class SecondaryDialogController : DialogController {

	tk2dSlicedSprite backgroundSprite;
	protected float cachedIndex;

	protected const float FADE_ALPHA = 0.3f;

	protected override void Awake () {
		base.Awake ();
		cachedIndex = 0.0f;
		backgroundSprite = transform.Find ("BackgroundSprite").GetComponent<tk2dSlicedSprite> ();

		float textX = GameUtils.ConvertPixelsToMeters(4.15f);
		float textY = GameUtils.ConvertPixelsToMeters(backgroundSprite.dimensions.y) - textX;
		textMesh.gameObject.transform.localPosition = new Vector2(textX, textY);
	}

	public float PercentHeight() {
		return transform.lossyScale.y;
	}

	public float ScaledHeight() {
		float rawHeight = GameUtils.ConvertPixelsToMeters(backgroundSprite.dimensions.y);
		return rawHeight * transform.lossyScale.y;
	}

	public void SetIndex(float index) {
		if (cachedIndex != index) {
			SetAlpha(Mathf.Max(FADE_ALPHA, 1.0f - (index / FADE_ALPHA)));
			cachedIndex = index;
		}
	}

	protected override void LateUpdate () {
		base.LateUpdate ();
	}

	protected virtual void SetAlpha(float alpha) {
		Color oldColor = backgroundSprite.color;
		backgroundSprite.color = new Color(oldColor.r, oldColor.g, oldColor.b, alpha);
		oldColor = textMesh.color;
		textMesh.color = new Color(oldColor.r, oldColor.g, oldColor.b, alpha);
	}
}
