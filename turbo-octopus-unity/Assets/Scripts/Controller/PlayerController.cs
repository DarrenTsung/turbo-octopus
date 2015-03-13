using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	protected const float TORSO_BEND_ANGLE_THRESHOLD = 80.0f;

	protected const float PLAYER_WIDTH = 0.4f;

	protected const string SPECIFIC_ANIMATION_TRIGGER_KEY = "SpecificAnimationTriggerKey";
	protected const int NO_ANIMATION_TRIGGER = 0;
	protected const int FORWARD_ROLL_ANIMATION_TRIGGER = 1;
	protected const int BACKWARD_ROLL_ANIMATION_TRIGGER = 2;
	protected const int SLASH_ANIMATION_TRIGGER = 3;

	protected const float MIN_WALK_PUFF_TIMER = 0.3f;
	protected const float MAX_WALK_PUFF_TIMER = 0.8f;
	protected const string WALK_PUFF_TIMER_KEY = "WalkPuffTimerKey";

	protected Object walkPuff;
	protected Object jumpPuff;

	protected float playerSpeed = 8.4f;
	protected float jumpForce = 800.0f;
	protected bool facingRight = true;

	protected bool doubleJumpCharge = true;

	protected Vector2 playerInputVelocity;

	protected float rollForce = 500.0f;

	protected Animator animator;

	protected GameObject body;
	protected GameObject head, torso, leftArm, rightArm, legs, gun;
	protected GameObject gunReferencePointRight, gunReferencePointLeft;

	protected GameObject groundCheck, leftCheck, rightCheck;
	protected bool grounded, leftTouching, rightTouching;
	protected LayerMask tileLayers;

	protected int leftBoostFrames = 0, rightBoostFrames = 0;

	private GunController gunController;
	new private Rigidbody2D rigidbody;

	protected bool playerInputAllowed;
	protected float torsoAnimationAngle;

	protected void Start () {
		animator = GetComponent<Animator> ();
		rigidbody = GetComponent<Rigidbody2D> ();

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
		groundCheck = transform.Find ("Checks/GroundCheck").gameObject;
		leftCheck = transform.Find ("Checks/LeftCheck").gameObject;
		rightCheck = transform.Find ("Checks/RightCheck").gameObject;

		TimerManager.Instance.addTimerForKey(WALK_PUFF_TIMER_KEY, Random.Range(MIN_WALK_PUFF_TIMER, MAX_WALK_PUFF_TIMER));
		walkPuff = Resources.Load ("Prefabs/SpecialEffects/WalkPuff");
		jumpPuff = Resources.Load ("Prefabs/SpecialEffects/JumpPuff");

		playerInputAllowed = true;
	}

	protected void Update () {
	}

	protected void LateUpdate () {
		Vector3 newTorsoEulerAngles = torso.transform.eulerAngles;
		newTorsoEulerAngles.z += torsoAnimationAngle;
		torso.transform.eulerAngles = newTorsoEulerAngles;

		Collider2D[] emptyArray = new Collider2D[1];
		int groundedHit = Physics2D.OverlapCircleNonAlloc(groundCheck.transform.position, 0.2f, emptyArray, tileLayers);
		grounded = groundedHit != 0 ? true : false;
		animator.SetBool("grounded", grounded);

		bool previouslyLeftTouching = leftTouching;
		bool previouslyRightTouching = rightTouching;

		int leftTouchingHit = Physics2D.OverlapCircleNonAlloc(leftCheck.transform.position, 0.25f, emptyArray, tileLayers);
		leftTouching = leftTouchingHit != 0 ? true : false;
		int rightTouchingHit = Physics2D.OverlapCircleNonAlloc(rightCheck.transform.position, 0.25f, emptyArray, tileLayers);
		rightTouching = rightTouchingHit != 0 ? true : false;

		animator.SetBool("leftTouching", leftTouching);
		animator.SetBool("rightTouching", rightTouching);

		bool startedLeftTouching = (!previouslyLeftTouching && (previouslyLeftTouching == !leftTouching));
		bool startedRightTouching = (!previouslyRightTouching && (previouslyRightTouching == !rightTouching));
		if (startedLeftTouching || startedRightTouching) {
			torso.transform.eulerAngles = new Vector3(0, 0, 0);
			head.transform.eulerAngles = new Vector3(0, 0, 0);
			legs.transform.eulerAngles = new Vector3(0, 0, 0);
			clearAnimationTriggerKeyAndAllowPlayerInput();
		}
		if (startedLeftTouching) {
			if (startedLeftTouching && !facingRight) {
				flip ();
			}
		} else if (startedRightTouching) {
			if (startedRightTouching && facingRight) {
				flip ();
			}
		}
	}

	protected void flip () {
		facingRight = !facingRight;
		Vector3 scale = body.transform.localScale;
		scale.x *= -1.0f;
		body.transform.localScale = scale;
	}

	public void handleAxisVector (Vector2 axisVector) {
		// the player input creates a desired velocity -> the velocity we should go towards
		float desiredSpeed = axisVector.x;

		float minSpeed = leftTouching ? -0.4f : -1.0f;
		float maxSpeed = rightTouching ? 0.4f : 1.0f;

		desiredSpeed = Mathf.Clamp(desiredSpeed, minSpeed, maxSpeed);
		desiredSpeed *= playerSpeed;

		float actualSpeed = rigidbody.velocity.x;
		float updatedSpeed;

		if (grounded) {
			updatedSpeed = desiredSpeed;
		} else {
			updatedSpeed = actualSpeed + ((desiredSpeed - actualSpeed) * Time.deltaTime);
		}

		rigidbody.velocity = new Vector2(updatedSpeed, rigidbody.velocity.y);

		animator.SetFloat ("horizontalSpeed", rigidbody.velocity.x);

		if (grounded) {
			if (rigidbody.velocity.x != 0.0 && TimerManager.Instance.timerDoneForKey(WALK_PUFF_TIMER_KEY)) {
				Instantiate(walkPuff, groundCheck.transform.position, Quaternion.identity);
				TimerManager.Instance.resetTimerForKey(WALK_PUFF_TIMER_KEY, Random.Range(MIN_WALK_PUFF_TIMER, MAX_WALK_PUFF_TIMER));
			}
		}
	}

	public void handleMousePosition () {
		Vector3 mouseWorldPosition = PlayerInputManager.mouseWorldPosition();

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

		if (grounded) {
			Instantiate(jumpPuff, groundCheck.transform.position, Quaternion.identity);
			rigidbody.AddForce(-Vector2.right * rollForce);
		} else {
			rigidbody.AddForce(-Vector2.right * rollForce * 2.0f / 3.0f);
		}

		PlayerInputManager.Instance.disablePlayerInput();
	}

	public void handleRightRollPressed () {
		if (facingRight) {
			animator.SetInteger(SPECIFIC_ANIMATION_TRIGGER_KEY, FORWARD_ROLL_ANIMATION_TRIGGER);
		} else {
			animator.SetInteger(SPECIFIC_ANIMATION_TRIGGER_KEY, BACKWARD_ROLL_ANIMATION_TRIGGER);
		}

		if (grounded) {
			Instantiate(jumpPuff, groundCheck.transform.position, Quaternion.identity);
			rigidbody.AddForce(Vector2.right * rollForce);
		} else {
			rigidbody.AddForce(Vector2.right * rollForce * 2.0f / 3.0f);
		}

		PlayerInputManager.Instance.disablePlayerInput();
	}

	public void clearAnimationTriggerKeyAndAllowPlayerInput () {
		animator.SetInteger(SPECIFIC_ANIMATION_TRIGGER_KEY, NO_ANIMATION_TRIGGER);
		torsoAnimationAngle = 0;
		PlayerInputManager.Instance.enablePlayerInput();
	}

	public void handleSlashPressed () {
		animator.SetInteger(SPECIFIC_ANIMATION_TRIGGER_KEY, SLASH_ANIMATION_TRIGGER);
		PlayerInputManager.Instance.disablePlayerInput();

		// set the torso pointing to the mouse position during the animation
		Vector2 mouseWorldPosition = PlayerInputManager.mouseWorldPosition();
		float angleToMouse = angleToPosition(torso, mouseWorldPosition);
		torsoAnimationAngle = angleToMouse;
		Debug.Log ("Torso animation angle is: " + torsoAnimationAngle);
	}

	public void handleFireButtonDown () {
		gunController.FireBulletIfPossible();
	}

	public void handleJumpButtonDown () {
		if (grounded) {
			rigidbody.AddForce(Vector2.up * jumpForce);
			grounded = false;
			Instantiate(jumpPuff, groundCheck.transform.position, Quaternion.identity);
			doubleJumpCharge = true;
		} else {
			if (leftTouching) {
				rigidbody.velocity = new Vector2(0.0f, 0.0f);
				rigidbody.AddForce((new Vector2(1.0f, 2.0f)).normalized * jumpForce * 1.3f);
				Instantiate(jumpPuff, leftCheck.transform.position, Quaternion.Euler(new Vector3(0, 0, 90)));
				doubleJumpCharge = true;
			} else if (rightTouching) {
				rigidbody.velocity = new Vector2(0.0f, 0.0f);
				rigidbody.AddForce((new Vector2(-1.0f, 2.0f)).normalized * jumpForce * 1.3f);
				Instantiate(jumpPuff, rightCheck.transform.position, Quaternion.Euler(new Vector3(0, 0, 90)));
				doubleJumpCharge = true;
			} else {
				// double jump if not touching wall or ground
				if (doubleJumpCharge) {
					doubleJumpCharge = false;
					Vector2 jumpVector = Vector2.up;
					jumpVector.x = 0.3f * Input.GetAxis ("Horizontal");
					rigidbody.AddForce(jumpVector.normalized * jumpForce * 0.6f);
				}
			}
		}
	}

	protected void pointObjectToPosition (GameObject obj, Vector3 position) {
		float angleInDegrees = angleToPosition(obj, position);
		obj.transform.eulerAngles = new Vector3 (0.0f, 0.0f, angleInDegrees);
	}

	protected float angleToPosition (GameObject obj, Vector3 position) {
		Vector3 rayToPosition = position - obj.transform.position;
		if (!facingRight) {
			rayToPosition.x *= -1.0f;
		}
		float angleInDegrees = Mathf.Atan2 (rayToPosition.y, rayToPosition.x) * 180.0f / Mathf.PI;
		angleInDegrees += objectSpecificAngleAdditions (obj);
		angleInDegrees = objectSpecificAngleRestrictions (obj, angleInDegrees);
		return angleInDegrees;
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
