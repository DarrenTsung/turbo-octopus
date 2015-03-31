using UnityEngine;
using System.Collections;

public class SecondaryDialogController : DialogController {

	protected tk2dSlicedSprite backgroundSprite;

	protected override void Start () {
		base.Start ();
		backgroundSprite = transform.Find ("BackgroundSprite").GetComponent<tk2dSlicedSprite> ();

		float textX = GameUtils.ConvertPixelsToMeters(4.15f);
		float textY = GameUtils.ConvertPixelsToMeters(backgroundSprite.dimensions.y) - textX;
		textMesh.gameObject.transform.localPosition = new Vector2(textX, textY);
	}
}
