using UnityEngine;
using System.Collections;

public class GunController : MonoBehaviour {
	private Transform bulletExitPoint;
	private Transform bulletReferencePoint;

	protected const string FIRING_BULLET_COOLDOWN_KEY = "bulletCooldownKey";
	protected float bulletFireCooldown = 0.05f;

	protected float recoilAmount = 5.0f;

	protected float cameraShakeValue = 0.03f;
	protected float cameraShakeTime = 0.1f;

	protected Object bullet; 

	protected float magazineSize = 5.0f;
	protected float reloadTime = 1.0f;

	protected float tracerPercentage = 0.3f;

	private CameraController cameraController;

	void Start () {
		bulletExitPoint = transform.Find ("BulletExitPoint");
		bulletReferencePoint = transform.Find ("BulletReferencePoint");

		bullet = Resources.Load ("Prefabs/Bullet");

		cameraController = Camera.main.GetComponent<CameraController> ();

		TimerManager.Instance.addTimerForKey (FIRING_BULLET_COOLDOWN_KEY);
	}

	public void FireBulletIfPossible () {
		if (TimerManager.Instance.timerDoneForKey (FIRING_BULLET_COOLDOWN_KEY)) {
			FireBullet();
			TimerManager.Instance.resetTimerForKey (FIRING_BULLET_COOLDOWN_KEY, bulletFireCooldown);
		}
	}

	protected void FireBullet () {
		Vector3 bulletRay = bulletExitPoint.position - bulletReferencePoint.position;
		bulletRay.z = 0.0f;

		GameObject bulletClone = Instantiate (bullet, bulletExitPoint.position, Quaternion.identity) as GameObject;
		BulletController bulletController = bulletClone.GetComponent<BulletController> ();
		bulletController.SetDirection (bulletRay);
		if (Random.value > tracerPercentage) {
			bulletController.SetTracerBullet(false);
		} else {
			bulletController.SetTracerBullet(true);
		}

		cameraController.CameraShake (cameraShakeValue, cameraShakeTime);
	}
}
