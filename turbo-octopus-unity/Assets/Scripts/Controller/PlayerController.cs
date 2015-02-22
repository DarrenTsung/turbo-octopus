using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	protected const float TORSO_BEND_ANGLE_THRESHOLD = 80.0f;

	protected const string SPECIFIC_ANIMATION_TRIGGER_KEY = "SpecificAnimationTriggerKey";
	protected const int NO_ANIMATION_TRIGGER = 0;
	protected const int FORWARD_ROLL_ANIMATION_TRIGGER = 1;
	protected const int BACKWARD_ROLL_ANIMATION_TRIGGER = 2;

	protected float playerSpeed = 7.4f;
	protected float jumpForce = 650.0f;
	protected bool facingRight = true;

	protected Vector2 playerInputVelocity;

	protected float rollForce = 400.0f;

	protected Animator animator;

	protected GameObject body;
	protected GameObject head, torso, leftArm, rightArm, legs, gun;
	protected GameObject gunReferencePointRight, gunReferencePointLeft;

	protected GameObject groundCheck, leftCheck, rightCheck;
	protected bool grounded, leftTouching, rightTouching;
	protected LayerMask tileLayers;

	private GunController gunController;

	protected bool playerInputAllowed;

	protected void Start () {
		animator = GetComponent<Animator> ();
		PlayerInputManager.Instance.SetPlayerController(this);

		body = transform.Find ("Body").gameObject;
		torso = body.transform.Find ("Torso").gameObject;
		legs = body.transform.Find ("Legs").gameObject;
		head = torso.transform.Find ("Head").gameObject;
		leftArm = torso.transform.Find ("LeftArm").gameObject;
		rightArm = torso.transform.Find ("RightArm").gameObject;
		gun = torso.transform.Find ("Gun").gameObject;
		gunController = gun.GetComponent<GunController> ();

		gunReferencePointRight = gun.transform.Find ("ReferencePointRight").gameObject;
		gunReferencePointLeft = gun.transform.Find ("ReferencePointLeft").gameObject;

		tileLayers = LayerMask.GetMask("Tile");
		Debug.Log("GroundCheck: " + groundCheck);
		groundCheck = transform.Find ("GroundCheck").gameObject;
		Debug.Log("GroundCheck: " + groundCheck);
		leftCheck = transform.Find ("LeftCheck").gameObject;
		rightCheck = transform.Find ("RightCheck").gameObject;

		playerInputAllowed = true;
	}

	protected void Update () {
		grounded = Physics2D.OverlapCircle(groundCheck.transform.position, 0.1f, tileLayers);
		animator.SetBool("grounded", grounded);

		bool previouslyLeftTouching = leftTouching;
		bool previouslyRightTouching = rightTouching;
		leftTouching = Physics2D.OverlapCircle(leftCheck.transform.position, 0.3f, tileLayers);
		animator.SetBool("leftTouching", leftTouching);
		rightTouching = Physics2D.OverlapCircle(rightCheck.transform.position, 0.3f, tileLayers);
		animator.SetBool("rightTouching", rightTouching);

		bool startedLeftTouching = (!previouslyLeftTouching && (previouslyLeftTouching == !leftTouching));
		bool startedRightTouching = (!previouslyRightTouching && (previouslyRightTouching == !rightTouching));
		if (startedLeftTouching || startedRightTouching) {
			float horizontalVelocity = startedLeftTouching ? -playerSpeed : playerSpeed;
			rigidbody2D.velocity = new Vector2(horizontalVelocity, rigidbody2D.velocity.y);

			if (startedLeftTouching && !facingRight) {
				flip ();
			} else if (startedRightTouching && facingRight) {
				flip ();
			}
		}
	}

	protected void LateUpdate () {
	}

	protected void flip () {
		facingRight = !facingRight;
		Vector3 scale = body.transform.localScale;
		scale.x *= -1.0f;
		body.transform.localScale = scale;
	}

	public void handleAxisVector (Vector2 axisVector) {
		float horizontalSpeed = axisVector.x;

		float minSpeed = leftTouching ? -0.4f : -1.0f;
		float maxSpeed = rightTouching ? 0.4f : 1.0f;

		horizontalSpeed = Mathf.Clamp(horizontalSpeed, minSpeed, maxSpeed);
		horizontalSpeed *= playerSpeed;

		animator.SetFloat("horizontalSpeed", horizontalSpeed);

		if (grounded) {
			rigidbody2D.velocity = new Vector2(horizontalSpeed, rigidbody2D.velocity.y);
		}
	}

	public void handleMousePosition (Vector2 mousePosition) {
		Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3 (mousePosition.x, mousePosition.y));

		Vector3 headRayToMouse = mouseWorldPosition - head.transform.position;
		if (!(leftTouching || rightTouching)) {
			// flip the player if the angle to the mouse is facing the opposite direction
			if (((facingRight && headRayToMouse.x < 0)
			    || (!facingRight && headRayToMouse.x > 0))) {
				flip ();
			}
		} 
		if (!facingRight) {
			headRayToMouse.x *= -1.0f;
		}

		float headAngleToMouse = Mathf.Atan2 (headRayToMouse.y, headRayToMouse.x) * 180.0f / Mathf.PI;

		// if the head would theoretically point above 80 degrees, rotate the torso the difference
		// don't rotate torso if touching wall
		if (headAngleToMouse > TORSO_BEND_ANGLE_THRESHOLD 
		    && !(leftTouching || rightTouching)) {
			torso.transform.eulerAngles += new Vector3 (0.0f, 0.0f, headAngleToMouse - TORSO_BEND_ANGLE_THRESHOLD);
		} else {
			torso.transform.eulerAngles += new Vector3 (0.0f, 0.0f, 0.0f);
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
		} else {
			if (leftTouching) {
				rigidbody2D.velocity = new Vector2(0.0f, 0.0f);
				rigidbody2D.AddForce((new Vector2(1.0f, 2.0f)).normalized * jumpForce * 1.3f);
			} else if (rightTouching) {
				rigidbody2D.velocity = new Vector2(0.0f, 0.0f);
				rigidbody2D.AddForce((new Vector2(-1.0f, 2.0f)).normalized * jumpForce * 1.3f);
			}
		}
	}

	protected void pointObjectToPosition (GameObject obj, Vector3 position) {
		Vector3 rayToPosition = position - obj.transform.position;
		if (!facingRight) {
			rayToPosition.x *= -1.0f;
		}
		float angleInDegrees = Mathf.Atan2 (rayToPosition.y, rayToPosition.x) * 180.0f / Mathf.PI;
		angleInDegrees += objectSpecificAngleAdditions (obj);
		angleInDegrees = objectSpecificAngleRestrictions (obj, angleInDegrees);
		obj.transform.eulerAngles = new Vector3 (0.0f, 0.0f, angleInDegrees);
	}

	protected float objectSpecificAngleAdditions (GameObject obj) {
		if (obj == gun) {
			return gun.GetComponent<GunController>().GetRecoilAngle();
		}
		return 0.0f;
	}

	protected float objectSpecificAngleRestrictions (GameObject obj, float angleInDegrees) {
		float min = -180.0f, max = 180.0f;

		if (leftTouching || rightTouching) {
			min = Mathf.Max (min, -90.0f);
			max = Mathf.Min (max, 90.0f);
		}
		if (obj == head && (leftTouching || rightTouching)) {
			max = Mathf.Min (max, 20.0f);
		}
		return Mathf.Clamp (angleInDegrees, min, max);
	}
}
