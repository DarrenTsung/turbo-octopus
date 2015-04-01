using UnityEngine;
using System.Collections;

public class SecondaryDialogController : DialogController {

	protected tk2dSlicedSprite backgroundSprite;
	protected int cachedIndex;

	protected override void Start () {
		base.Start ();
		cachedIndex = 0;
		backgroundSprite = transform.Find ("BackgroundSprite").GetComponent<tk2dSlicedSprite> ();

		float textX = GameUtils.ConvertPixelsToMeters(4.15f);
		float textY = GameUtils.ConvertPixelsToMeters(backgroundSprite.dimensions.y) - textX;
		textMesh.gameObject.transform.localPosition = new Vector2(textX, textY);
	}

	public float ScaledHeight() {
		float rawHeight = GameUtils.ConvertPixelsToMeters(backgroundSprite.dimensions.y);
		return rawHeight * transform.lossyScale.y;
	}

	public void SetIndex(int index) {
		if (cachedIndex != index) {
			if (index > 0) {
				SetAlpha(0.5f);
			} else {
				SetAlpha(1.0f);
			}
			cachedIndex = index;
		}
	}

	protected override void LateUpdate ()
	{
		base.LateUpdate ();
	}

	protected virtual void SetAlpha(float alpha) {
		Color oldColor = backgroundSprite.color;
		backgroundSprite.color = new Color(oldColor.r, oldColor.g, oldColor.b, alpha);
		oldColor = textMesh.color;
		textMesh.color = new Color(oldColor.r, oldColor.g, oldColor.b, alpha);
	}
}
