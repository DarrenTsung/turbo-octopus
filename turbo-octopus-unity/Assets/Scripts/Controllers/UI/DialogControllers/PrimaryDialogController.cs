using UnityEngine;
using System.Collections;

public class PrimaryDialogController : DialogController {

	protected GameObject backgroundObject;
	protected tk2dSlicedSprite backgroundSprite;
	protected tk2dCameraAnchor myAnchor;

	protected override void Start ()
	{
		base.Start ();
		myAnchor = transform.parent.gameObject.GetComponent<tk2dCameraAnchor> ();
		backgroundObject = transform.Find ("Background").gameObject;
		backgroundSprite = backgroundObject.GetComponent<tk2dSlicedSprite> ();

		tk2dCamera UICamera = tk2dCamera.CameraForLayer(GameUtils.UILayer);
		float cameraZoom = UICamera.ZoomFactor;
		float cameraWidth = UICamera.nativeResolutionWidth / cameraZoom;
		float cameraHeight = UICamera.nativeResolutionHeight / cameraZoom;

		// dimensions are in pixels
		backgroundSprite.dimensions = new Vector2(cameraWidth * 0.8f, cameraHeight * 0.25f);
		myAnchor.AnchorPoint = tk2dBaseSprite.Anchor.LowerLeft;
		myAnchor.AnchorOffsetPixels = new Vector2(cameraWidth * 0.1f, cameraHeight * 0.1f);

		// bake 1:1
		backgroundSprite.MakePixelPerfect();
	}

}
