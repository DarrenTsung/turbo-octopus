using UnityEngine;
using System.Collections;

public class GunController : MonoBehaviour {
	private Transform bulletExitPoint;
	private Transform bulletReferencePoint;

	protected const string FIRING_BULLET_COOLDOWN_KEY = "bulletCooldownKey";
	protected const float GUN_ANGLE_RECOVERY_SPEED = 10.0f;

	protected float bulletFireCooldown = 0.10f;

	protected float cameraShakeValue = 0.03f;
	protected float cameraShakeTime = 0.1f;

	protected Object bullet; 

	protected float bulletSpread = 0.03f;

	protected float magazineSize = 5.0f;
	protected float reloadTime = 1.0f;

	protected float tracerPercentage = 1.0f;

	private CameraController cameraController;

	protected MuzzleFlashController muzzleFlashController;

	// recoil goes from 0.0 to 1.0
	protected float recoil = 0.0f;
	protected float angleRecoil = 10.0f;
	protected Vector3 positionRecoil = new Vector3(-0.1f, 0.0f, 0.0f);

	protected Vector3 basePosition;

	void Start () {
		bulletExitPoint = transform.Find ("BulletExitPoint");
		bulletReferencePoint = transform.Find ("BulletReferencePoint");

		bullet = Resources.Load ("Prefabs/Bullet");

		muzzleFlashController = transform.Find ("MuzzleFlash").gameObject.GetComponent<MuzzleFlashController> ();

		cameraController = Camera.main.GetComponent<CameraController> ();

		TimerManager.Instance.addTimerForKey (FIRING_BULLET_COOLDOWN_KEY);

		basePosition = transform.localPosition;
	}

	void Update () {
		// update the gun recoil
		if (recoil > 0.01f) {
			recoil -= GUN_ANGLE_RECOVERY_SPEED * Time.deltaTime;
		} else {
			recoil = 0.0f;
		}

		transform.localPosition = basePosition + (recoil * positionRecoil);
	}

	public void FireBulletIfPossible () {
		if (TimerManager.Instance.timerDoneForKey (FIRING_BULLET_COOLDOWN_KEY)) {
			FireBullet();
			TimerManager.Instance.resetTimerForKey (FIRING_BULLET_COOLDOWN_KEY, bulletFireCooldown);
		}
	}

	protected void FireBullet () {
		Vector3 exitPoint = bulletExitPoint.position;
		exitPoint.x += Random.Range(-bulletSpread, bulletSpread);
		exitPoint.y += Random.Range(-bulletSpread, bulletSpread);

		Vector3 bulletRay = exitPoint - bulletReferencePoint.position;
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
		recoil = 1.0f;
		muzzleFlashController.MuzzleFlash(0.03f);
	}

	public float GetRecoilAngle () {
		return recoil * angleRecoil;
	}
}
