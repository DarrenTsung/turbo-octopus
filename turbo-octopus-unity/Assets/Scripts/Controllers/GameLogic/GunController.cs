using UnityEngine;
using System.Collections;

public class GunController : MonoBehaviour {
	private Transform bulletExitPoint;
	private Transform bulletReferencePoint;

	protected const string FIRING_BULLET_COOLDOWN_KEY = "bulletCooldownKey";
	protected const float GUN_ANGLE_RECOVERY_SPEED = 7.0f;
	protected float GUN_FORCE_RECOIL = 150.0f;

	protected float bulletFireCooldown = 0.10f;

	protected float cameraShakeValue = 0.03f;
	protected float cameraShakeTime = 0.1f;

	protected float bulletSpread = 0.03f;

	protected float magazineSize = 5.0f;
	protected float reloadTime = 1.0f;

	protected float tracerPercentage = 1.0f;

	private CameraController cameraController;

	protected MuzzleFlashController muzzleFlashController;

	// recoil goes from 0.0 to 1.0
	protected float recoil = 0.0f;
	protected float angleRecoil = 5.0f;
	protected Vector3 positionRecoil = new Vector3(-0.5f, 0.0f, 0.0f);

	protected Vector3 basePosition;

	protected PlayerController playerController;

	protected Timer readyToFireTimer;

	void Start () {
		bulletExitPoint = transform.Find ("BulletExitPoint");
		bulletReferencePoint = transform.Find ("BulletReferencePoint");

		muzzleFlashController = transform.Find ("MuzzleFlash").gameObject.GetComponent<MuzzleFlashController> ();

		cameraController = Camera.main.GetComponent<CameraController> ();

		readyToFireTimer = TimerManager.Instance.MakeTimer();
		readyToFireTimer.SetTime(bulletFireCooldown);

		basePosition = transform.localPosition;
	}

	public void SetPlayerController(PlayerController pc) {
		playerController = pc;
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
		if (readyToFireTimer.IsFinished()) {
			FireBullet();
			readyToFireTimer.SetTime(bulletFireCooldown);
		}
	}

	protected void FireBullet () {
		Vector3 exitPoint = bulletExitPoint.position;
		exitPoint.x += Random.Range(-bulletSpread, bulletSpread);
		exitPoint.y += Random.Range(-bulletSpread, bulletSpread);

		Vector3 bulletRay = exitPoint - bulletReferencePoint.position;
		bulletRay.z = 0.0f;

		GameObject bulletClone = Instantiate (PrefabManager.Instance.Bullet, bulletExitPoint.position, Quaternion.identity) as GameObject;
		bulletClone.transform.parent = PrefabManager.Instance.transform;

		BulletController bulletController = bulletClone.GetComponent<BulletController> ();
		bulletController.SetDirection (bulletRay);

		DamageController damageController = bulletClone.GetComponent<DamageController> ();
		damageController.SetUp(2, 4, 0.0f, 1.0f);

		if (Random.value > tracerPercentage) {
			bulletController.SetTracerBullet(false);
		} else {
			bulletController.SetTracerBullet(true);
		}

		cameraController.CameraShake (cameraShakeValue, cameraShakeTime);
		recoil = 1.0f;
		muzzleFlashController.MuzzleFlash(0.03f);

		if (!playerController) {
			Debug.LogError("Gun firing, but no player controller to influence!");
		}
		playerController.addForce(-bulletRay.normalized * GUN_FORCE_RECOIL);
	}

	public float GetRecoilAngle () {
		return recoil * angleRecoil;
	}
}
