using UnityEngine;
using System.Collections;

public class UIAnchorRelativePosition : MonoBehaviour {

	public tk2dCameraAnchor anchor;
	public Vector2 relativePosition;
	protected Vector2 computedPosition;

	protected void Start() {
		anchor = GetComponent<tk2dCameraAnchor> ();
		RecomputePosition();
	}

	public void RecomputePosition() {
		tk2dCamera UICamera = tk2dCamera.CameraForLayer(GameUtils.UILayer); 
		float cameraZoom = UICamera.ZoomFactor;
		float cameraWidth = UICamera.nativeResolutionWidth / cameraZoom;
		float cameraHeight = UICamera.nativeResolutionHeight / cameraZoom;

		computedPosition = new Vector2(cameraWidth * relativePosition.x, cameraHeight * relativePosition.y);
		Debug.Log ("Computed position: " + computedPosition);
		anchor.AnchorOffsetPixels = computedPosition;
	}
}
