using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	protected const float TORSO_BEND_ANGLE_THRESHOLD = 80.0f;

	protected const string SPECIFIC_ANIMATION_TRIGGER_KEY = "SpecificAnimationTriggerKey";
	protected const int NO_ANIMATION_TRIGGER = 0;
	protected const int FORWARD_ROLL_ANIMATION_TRIGGER = 1;
	protected const int BACKWARD_ROLL_ANIMATION_TRIGGER = 2;

	protected float playerSpeed = 6.5f;
	protected float jumpForce = 550.0f;
	protected bool facingRight = true;

	protected float rollForce = 500.0f;

	protected Animator animator;

	protected GameObject head, torso, leftArm, rightArm, legs, gun;
	protected GameObject gunReferencePointRight, gunReferencePointLeft;

	protected GameObject groundCheck;
	protected LayerMask groundLayers;
	protected bool grounded;

	private GunController gunController;

	protected bool playerInputAllowed;

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

		groundLayers = LayerMask.GetMask("Ground");
		groundCheck = transform.Find ("GroundCheck").gameObject;

		playerInputAllowed = true;
	}

	protected void Update () {
		grounded = Physics2D.OverlapCircle(groundCheck.transform.position, 0.1f, groundLayers);
		animator.SetBool("grounded", grounded);
	}

	protected void flip () {
		facingRight = !facingRight;
		Vector3 scale = transform.localScale;
		scale.x *= -1.0f;
		transform.localScale = scale;
	}

	public void handleAxisVector (Vector2 axisVector) {
		float horizontalSpeed = axisVector.x * playerSpeed;
		animator.SetFloat("horizontalSpeed", horizontalSpeed);

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

	public void handleLeftRollPressed () {
		if (facingRight) {
			animator.SetInteger(SPECIFIC_ANIMATION_TRIGGER_KEY, BACKWARD_ROLL_ANIMATION_TRIGGER);
		} else {
			animator.SetInteger(SPECIFIC_ANIMATION_TRIGGER_KEY, FORWARD_ROLL_ANIMATION_TRIGGER);
		}
		rigidbody2D.AddForce(-Vector2.right * rollForce);

		PlayerInputManager.Instance.disablePlayerInput();
	}

	public void handleRightRollPressed () {
		if (facingRight) {
			animator.SetInteger(SPECIFIC_ANIMATION_TRIGGER_KEY, FORWARD_ROLL_ANIMATION_TRIGGER);
		} else {
			animator.SetInteger(SPECIFIC_ANIMATION_TRIGGER_KEY, BACKWARD_ROLL_ANIMATION_TRIGGER);
		}
		rigidbody2D.AddForce(Vector2.right * rollForce);

		PlayerInputManager.Instance.disablePlayerInput();
	}

	public void clearAnimationTriggerKeyAndAllowPlayerInput () {
		animator.SetInteger(SPECIFIC_ANIMATION_TRIGGER_KEY, NO_ANIMATION_TRIGGER);
		PlayerInputManager.Instance.enablePlayerInput();
	}

	public void handleFireButtonDown () {
		gunController.FireBulletIfPossible();
	}

	public void handleJumpButtonDown () {
		if (grounded) {
			rigidbody2D.AddForce(Vector2.up * jumpForce);
			grounded = false;
		}
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
			return gun.GetComponent<GunController>().GetRecoilAngle();
		}
		return 0.0f;
	}
}
