using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	protected const float TORSO_BEND_ANGLE_THRESHOLD = 80.0f;
	protected const float GUN_ANGLE_RECOVERY_SPEED = 30.0f;

	protected float playerSpeed = 6.5f;
	protected bool facingRight = true;

	protected Animator animator;

	protected GameObject head, torso, leftArm, rightArm, legs, gun;
	protected GameObject gunReferencePointRight, gunReferencePointLeft;

	private GunController gunController;

	protected float gunRecoil = 0.0f;

	protected void Start () {
		animator = GetComponent<Animator> ();
		PlayerInputManager.Instance.SetPlayerController(this);

		head = transform.Find ("Torso/Head").gameObject;
		torso = transform.Find ("Torso").gameObject;
		leftArm = transform.Find ("Torso/LeftArm").gameObject;
		rightArm = transform.Find ("Torso/RightArm").gameObject;
		legs = transform.Find ("Legs").gameObject;
		gun = transform.Find ("Torso/Gun").gameObject;
		gunController = gun.GetComponent<GunController> ();

		gunReferencePointRight = gun.transform.Find ("ReferencePointRight").gameObject;
		gunReferencePointLeft = gun.transform.Find ("ReferencePointLeft").gameObject;
	}

	protected void Update () {
		// update the gun recoil
		if (gunRecoil > 0.01f) {
			gunRecoil -= GUN_ANGLE_RECOVERY_SPEED * Time.deltaTime;
		} else {
			gunRecoil = 0.0f;
		}
	}

	protected void flip () {
		facingRight = !facingRight;
		Vector3 scale = transform.localScale;
		scale.x *= -1.0f;
		transform.localScale = scale;
	}

	public void handleAxisVector (Vector2 axisVector) {
		float horizontalSpeed = axisVector.x * playerSpeed;
		animator.SetFloat("horizontalSpeed", Mathf.Abs(horizontalSpeed));

		rigidbody2D.velocity = new Vector2(horizontalSpeed, rigidbody2D.velocity.y);
	}

	public void handleMousePosition (Vector2 mousePosition) {
		Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3 (mousePosition.x, mousePosition.y));

		// flip the player if the angle to the mouse is facing the opposite direction
		Vector3 headRayToMouse = mouseWorldPosition - head.transform.position;
		if ((facingRight && headRayToMouse.x < 0)
		    || (!facingRight && headRayToMouse.x > 0)) {
			flip ();
		}
		if (!facingRight) {
			headRayToMouse.x *= -1.0f;
		}
		float headAngleToMouse = Mathf.Atan2 (headRayToMouse.y, headRayToMouse.x) * 180.0f / Mathf.PI;

		// if the head would theoretically point above 80 degrees, rotate the torso the difference
		if (headAngleToMouse > TORSO_BEND_ANGLE_THRESHOLD) {
			torso.transform.eulerAngles = new Vector3 (0.0f, 0.0f, headAngleToMouse - TORSO_BEND_ANGLE_THRESHOLD);
		} else {
			torso.transform.eulerAngles = new Vector3 (0.0f, 0.0f, 0.0f);
		}

		pointObjectToPosition (head, mouseWorldPosition);
		pointObjectToPosition (gun, mouseWorldPosition);
		pointObjectToPosition (rightArm, gunReferencePointRight.transform.position);
		pointObjectToPosition (leftArm, gunReferencePointLeft.transform.position);
	}

	public void handleActionPressed (int actionNumber) {

	}

	public void handleFireButtonDown () {
		gunController.FireBulletIfPossible();
	}

	protected void pointObjectToPosition (GameObject obj, Vector3 position) {
		Vector3 rayToPosition = position - obj.transform.position;
		if (!facingRight) {
			rayToPosition.x *= -1.0f;
		}
		float angleInDegrees = Mathf.Atan2 (rayToPosition.y, rayToPosition.x) * 180.0f / Mathf.PI;
		angleInDegrees += objectSpecificAngleAdditions (obj);
		obj.transform.eulerAngles = new Vector3 (0.0f, 0.0f, angleInDegrees);
	}

	protected float objectSpecificAngleAdditions (GameObject obj) {
		if (obj == gun) {
			return gunRecoil;
		}
		return 0.0f;
	}
}
