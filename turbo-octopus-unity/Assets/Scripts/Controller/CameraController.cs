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

	void Start () {
		shakeArray = new List<ShakeInfo>();
	}
	
	void Update () {
		Vector3 rayToTransform = transformFollowing.position - basePosition;
		basePosition += rayToTransform / 5.0f;

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
	}

	public void CameraShake (float value, float time) {
		ShakeInfo s = new ShakeInfo (value, time);
		shakeArray.Add (s);
	}

}
