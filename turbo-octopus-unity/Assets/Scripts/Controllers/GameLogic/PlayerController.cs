using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	protected const float TORSO_BEND_ANGLE_THRESHOLD = 80.0f;

	protected const float PLAYER_WIDTH = 0.4f;

	protected const string SPECIFIC_ANIMATION_TRIGGER_KEY = "SpecificAnimationTriggerKey";
	protected const int NO_ANIMATION_TRIGGER = 0;
	protected const int FORWARD_ROLL_ANIMATION_TRIGGER = 1;
	protected const int BACKWARD_ROLL_ANIMATION_TRIGGER = 2;
	protected const int SLASH_1_ANIMATION_TRIGGER = 3;
	protected const int SLASH_2_ANIMATION_TRIGGER = 4;
	protected const int SLASH_3_ANIMATION_TRIGGER = 5;

	protected const float MIN_WALK_PUFF_TIMER = 0.3f;
	protected const float MAX_WALK_PUFF_TIMER = 0.8f;
	protected Timer walkPuffTimer;

	protected const float IN_AIR_FORCE_MULTIPLIER = 0.3f;

	protected Object walkPuff;
	protected Object jumpPuff;

	protected float playerSpeed = 8.4f;
	protected float jumpForce = 800.0f;
	protected bool facingRight = true;

	protected bool doubleJumpCharge = true;

	protected Vector2 playerInputVelocity;

	protected float rollForce = 650.0f;

	protected Animator animator;

	protected GameObject body;
	protected GameObject head, torso, leftArm, rightArm, legs, gun;
	protected GameObject gunReferencePointRight, gunReferencePointLeft;
	protected GameObject slash, slashCollider;
	protected SlashController slashController;
	protected FreezingController freezingController;
	protected int slashCombo;
	protected bool slashPressed;

	protected GameObject groundCheck, leftCheck, rightCheck;
	protected bool grounded, leftTouching, rightTouching;

	protected int leftBoostFrames = 0, rightBoostFrames = 0;

	protected GunController gunController;
	new protected Rigidbody2D rigidbody;

	protected bool playerInputAllowed;
	protected float torsoAnimationAngle;

	protected bool rollingEnabled, jumpingEnabled, shootingEnabled, slashingEnabled;

	protected void Start () {
		rollingEnabled = true;
		jumpingEnabled = true;
		shootingEnabled = true;
		slashingEnabled = true;

		animator = GetComponent<Animator> ();
		rigidbody = GetComponent<Rigidbody2D> ();
		freezingController = GetComponent<FreezingController> ();

		PlayerInputManager.Instance.SetPlayerController(this);

		body = transform.Find ("Body").gameObject;
		torso = body.transform.Find ("Torso").gameObject;
		legs = body.transform.Find ("Legs").gameObject;
		head = torso.transform.Find ("Head").gameObject;
		leftArm = torso.transform.Find ("LeftArm").gameObject;
		rightArm = torso.transform.Find ("RightArm").gameObject;
		slash = torso.transform.Find ("Slash").gameObject;
		slashCollider = torso.transform.Find ("SlashCollider").gameObject;
		slashController = slashCollider.GetComponent<SlashController>();
		slashController.SetParentController(this);
		slashCollider.GetComponent<DamageController>().SetUp(5, 10, 0.3f, 1.3f);
		slashCombo = 1;

		gun = torso.transform.Find ("Gun").gameObject;
		gunController = gun.GetComponent<GunController> ();
		gunController.SetPlayerController(this);

		gunReferencePointRight = gun.transform.Find ("ReferencePointRight").gameObject;
		gunReferencePointLeft = gun.transform.Find ("ReferencePointLeft").gameObject;

		groundCheck = transform.Find ("Checks/GroundCheck").gameObject;
		leftCheck = transform.Find ("Checks/LeftCheck").gameObject;
		rightCheck = transform.Find ("Checks/RightCheck").gameObject;

		walkPuffTimer = TimerManager.Instance.MakeTimer();
		walkPuffTimer.SetTime(Random.Range(MIN_WALK_PUFF_TIMER, MAX_WALK_PUFF_TIMER));
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
		int groundedHit = Physics2D.OverlapCircleNonAlloc(groundCheck.transform.position, 0.2f, emptyArray, GameUtils.tileLayers);
		grounded = groundedHit != 0 ? true : false;
		animator.SetBool("grounded", grounded);

		bool previouslyLeftTouching = leftTouching;
		bool previouslyRightTouching = rightTouching;

		int leftTouchingHit = Physics2D.OverlapCircleNonAlloc(leftCheck.transform.position, 0.25f, emptyArray, GameUtils.tileLayers);
		leftTouching = leftTouchingHit != 0 ? true : false;
		int rightTouchingHit = Physics2D.OverlapCircleNonAlloc(rightCheck.transform.position, 0.25f, emptyArray, GameUtils.tileLayers);
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

	public bool isFacingRight() {
		return facingRight;
	}

	public void handleAxisVector (Vector2 axisVector) {
		// if our rigidbody is current kinematic, don't move through player input
		if (rigidbody.isKinematic) {
			return;
		}

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
			if (rigidbody.velocity.x != 0.0 && walkPuffTimer.IsFinished()) {
				Instantiate(walkPuff, groundCheck.transform.position, Quaternion.identity);
				walkPuffTimer.SetTime(Random.Range(MIN_WALK_PUFF_TIMER, MAX_WALK_PUFF_TIMER));
			}
		}
	}

	public void handleMousePosition () {
		Vector3 mouseWorldPosition = PlayerInputManager.MouseWorldPosition();

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

	public bool canRoll() {
		bool touchingWall = leftTouching || rightTouching;
		return rollingEnabled && !touchingWall;
	}

	public void handleLeftRollPressed () {
		if (!canRoll()) {
			return;
		}

		if (facingRight) {
			animator.SetInteger(SPECIFIC_ANIMATION_TRIGGER_KEY, BACKWARD_ROLL_ANIMATION_TRIGGER);
		} else {
			animator.SetInteger(SPECIFIC_ANIMATION_TRIGGER_KEY, FORWARD_ROLL_ANIMATION_TRIGGER);
		}

		if (grounded) {
			Instantiate(jumpPuff, groundCheck.transform.position, Quaternion.identity);
		}
		addForce(-Vector2.right * rollForce);

		PlayerInputManager.Instance.DisablePlayerInput();
	}

	public void handleRightRollPressed () {
		if (!canRoll()) {
			return;
		}

		if (facingRight) {
			animator.SetInteger(SPECIFIC_ANIMATION_TRIGGER_KEY, FORWARD_ROLL_ANIMATION_TRIGGER);
		} else {
			animator.SetInteger(SPECIFIC_ANIMATION_TRIGGER_KEY, BACKWARD_ROLL_ANIMATION_TRIGGER);
		}

		if (grounded) {
			Instantiate(jumpPuff, groundCheck.transform.position, Quaternion.identity);
		}
		addForce(Vector2.right * rollForce);

		PlayerInputManager.Instance.DisablePlayerInput();
	}

	public void clearAnimationTriggerKeyAndAllowPlayerInput () {
		animator.SetInteger(SPECIFIC_ANIMATION_TRIGGER_KEY, NO_ANIMATION_TRIGGER);
		torsoAnimationAngle = 0;
		PlayerInputManager.Instance.EnablePlayerInput();
	}

	public void handleSlashPressed () {
		slashPressed = true;
		int currentTriggerKey = animator.GetInteger(SPECIFIC_ANIMATION_TRIGGER_KEY);
		if (currentTriggerKey == SLASH_1_ANIMATION_TRIGGER
		    || currentTriggerKey == SLASH_2_ANIMATION_TRIGGER
		    || currentTriggerKey == SLASH_3_ANIMATION_TRIGGER) {
			// do nothing for now
		} else {
			slashPressed = false;
			slashController.SetComboLevel(slashCombo);
			animator.SetInteger(SPECIFIC_ANIMATION_TRIGGER_KEY, SLASH_1_ANIMATION_TRIGGER);
			PlayerInputManager.Instance.DisablePlayerInput();
			PlayerInputManager.Instance.movementInputEnabled = true;
			PlayerInputManager.Instance.jumpingEnabled = true;
			PlayerInputManager.Instance.slashingEnabled = true;

			// set the torso pointing to the mouse position during the animation
			Vector2 mouseWorldPosition = PlayerInputManager.MouseWorldPosition();
			float angleToMouse = angleToPosition(torso, mouseWorldPosition);
			torsoAnimationAngle = angleToMouse;
		}
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
					rigidbody.velocity = new Vector2(rigidbody.velocity.x, 0.0f);
					rigidbody.AddForce(Vector2.up * jumpForce);
					Instantiate(jumpPuff, groundCheck.transform.position, Quaternion.identity);
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

	public void addForce(Vector2 force) {
		if (grounded) {
			rigidbody.AddForce(force);
		} else {
			rigidbody.AddForce(force * IN_AIR_FORCE_MULTIPLIER);
		}
	}

	public void slashUpwardForce() {
		rigidbody.AddForce(Vector2.up * 200.0f);
	}

	public void EndSlash() {
		// if the user has pressed slash again while slashing, move to next part of the combo if possible
		if (slashPressed) {
			int currentTriggerKey = animator.GetInteger(SPECIFIC_ANIMATION_TRIGGER_KEY);
			if (currentTriggerKey != SLASH_2_ANIMATION_TRIGGER) {
				animator.SetInteger(SPECIFIC_ANIMATION_TRIGGER_KEY, currentTriggerKey+1);
				slashPressed = false;
			} else {
				clearAnimationTriggerKeyAndAllowPlayerInput();
			}
		} else {
			clearAnimationTriggerKeyAndAllowPlayerInput();
		}

		bool hitAnything = slashController.HitAnything();
		if (hitAnything) {
			slashCombo++;
		} else {
			slashCombo = 1;
		}

		slashController.EndSlash();
	}
}
