using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShakeInfo {
	public float timeLeft;
	public float shakeValue;

	public ShakeInfo (float value, float time) {
		timeLeft = time;
		shakeValue = value;
	}
}

public class CameraController : MonoBehaviour {
	public Transform transformFollowing;

	private List<ShakeInfo> shakeArray;

	private Vector3 basePosition;

	private Vector3 PIXEL_SIZE;

	void Start () {
		shakeArray = new List<ShakeInfo>();
		PIXEL_SIZE = Camera.main.ScreenToWorldPoint(new Vector3(1, 1, 0)) - Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
		Debug.Log ("Pixel size: " + PIXEL_SIZE);
	}
	
	void Update () {
		Vector3 rayToTransform = transformFollowing.position - basePosition;
		basePosition += rayToTransform / 2.0f;

		// clear temperal effects on the camera position
		transform.position = new Vector3(basePosition.x,
		                                 basePosition.y,
		                                 -10.0f);

		//Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 0));
		//transform.position += mousePosition - transformFollowing.position;

		if (shakeArray.Count > 0) {
			float totalShake = 0;
			for (int i = 0; i < shakeArray.Count; i++) {
				ShakeInfo s = shakeArray[i];
				s.timeLeft -= Time.deltaTime;
				if (s.timeLeft < 0) {
					shakeArray.Remove (s);
				} else {
					totalShake += s.shakeValue;
				}
			}

			float cameraShakeX = Random.Range(-totalShake / 2.0f, totalShake / 2.0f);
			float cameraShakeY = Random.Range(-totalShake / 2.0f, totalShake / 2.0f);

			Vector3 shakeVector = new Vector3(cameraShakeX, cameraShakeY, 0.0f);

			transform.position += shakeVector;
		}

		SnapPositionToNearestPixel();
	}

	protected void SnapPositionToNearestPixel() {
		Vector3 newPosition = transform.position;
		newPosition.x = transform.position.x - (transform.position.x % PIXEL_SIZE.x);
		newPosition.y = transform.position.y - (transform.position.y % PIXEL_SIZE.y);
		transform.position = newPosition;
	}

	public void CameraShake (float value, float time) {
		ShakeInfo s = new ShakeInfo (value, time);
		shakeArray.Add (s);
	}

}
