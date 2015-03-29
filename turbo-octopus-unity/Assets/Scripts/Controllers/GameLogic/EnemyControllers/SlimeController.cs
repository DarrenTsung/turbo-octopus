using UnityEngine;
using System.Collections;

public class SlimeController : EnemyController {

	protected const string IDLE_STATE_ID = "SlimeIdleStateId";
	protected const string WALK_STATE_ID = "SlimeWalkStateId";
	protected const string PREJUMP_STATE_ID = "SlimePreJumpStateId";
	protected const string JUMPING_STATE_ID = "SlimeJumpingStateId";

	protected Transform groundCheck, leftCheck, rightCheck;
	protected bool grounded, leftTouching, rightTouching;

	protected Direction direction;

	protected float speed, jumpForce;

	public override void OnHit (GameObject obj, Vector2 hitPoint) {
		DamageController damageController = obj.GetComponent<DamageController> ();
		if (!damageController) {
			return;
		}

		DamageModel damageModel = damageController.ComputeDamage();

		health -= damageModel.computedDamage;

		EventManager.CallDamageDealt(damageModel, gameObject, obj, hitPoint);
	}

	protected override void SetUpStateMachine() {
		stateMachine.AddState(IDLE_STATE_ID, 1.0f, 3.0f);
		stateMachine.AddState(WALK_STATE_ID, 2.0f, 5.0f);
		// negative min/max time means that the state will not advance without an outside trigger
		stateMachine.AddState(PREJUMP_STATE_ID, -999.0f, -999.0f);
		stateMachine.AddState(JUMPING_STATE_ID, -999.0f, -999.0f);

		stateMachine.AddTransition(IDLE_STATE_ID, PREJUMP_STATE_ID, 0.3f);
		stateMachine.AddTransition(IDLE_STATE_ID, WALK_STATE_ID, 0.7f);

		stateMachine.AddTransition(WALK_STATE_ID, IDLE_STATE_ID, 0.4f);
		stateMachine.AddTransition(WALK_STATE_ID, PREJUMP_STATE_ID, 0.6f);

		stateMachine.AddTransition(PREJUMP_STATE_ID, JUMPING_STATE_ID, 1.0f);

		stateMachine.AddTransition(JUMPING_STATE_ID, IDLE_STATE_ID, 0.9f);
		stateMachine.AddTransition(JUMPING_STATE_ID, WALK_STATE_ID, 0.1f);

		stateMachine.SetStartState(IDLE_STATE_ID);
	}

	protected override void Awake () {
		base.Awake();
		groundCheck = transform.Find ("GroundCheck");
		leftCheck = transform.Find ("LeftCheck");
		rightCheck = transform.Find ("RightCheck");
		direction = Direction.Right;
		jumpForce = 600.0f;
		speed = 4.0f;
		health = 50.0f;
	}
	
	protected override void Update () {
		dying = health <= 0.0f;
		animator.SetBool("Dying", dying);

		Collider2D[] emptyArray = new Collider2D[1];
		int groundedHit = Physics2D.OverlapCircleNonAlloc(groundCheck.position, 0.2f, emptyArray, GameUtils.tileLayers);
		grounded = groundedHit != 0 ? true : false;
		animator.SetBool("Grounded", grounded);

		int leftHit = Physics2D.OverlapCircleNonAlloc(leftCheck.position, 0.2f, emptyArray, GameUtils.tileLayers);
		leftTouching = leftHit != 0 ? true : false;

		int rightHit = Physics2D.OverlapCircleNonAlloc(rightCheck.position, 0.2f, emptyArray, GameUtils.tileLayers);
		rightTouching = rightHit != 0 ? true : false;

		if (stateMachine.CurrentStateId().Equals(WALK_STATE_ID)) {
			switch (direction) {
				case Direction.Left:
					rigidbody2D.velocity = new Vector2(-speed, rigidbody2D.velocity.y);
					break;
				case Direction.Right:
					rigidbody2D.velocity = new Vector2(speed, rigidbody2D.velocity.y);
					break;
				default:
					Debug.LogWarning("SlimeUpdate - direction is not valid: " + direction);
					break;
			}
		}
		animator.SetFloat("AbsoluteHorizontalSpeed", Mathf.Abs(rigidbody2D.velocity.x));
		animator.SetFloat("AbsoluteVerticalSpeed", Mathf.Abs(rigidbody2D.velocity.y));

		// if we're not in jumping state and we're not grounded, transition to jumping
		if (!stateMachine.CurrentStateId().Equals(JUMPING_STATE_ID) && !grounded) {
			stateMachine.TransitionToState(JUMPING_STATE_ID);
		}

		// if we're in jumping state and we're grounded, transition out of jumping state
		if (stateMachine.CurrentStateId().Equals(JUMPING_STATE_ID) && grounded) {
			stateMachine.AdvanceCurrentState();
		}

		if (stateMachine.CurrentStateId().Equals(PREJUMP_STATE_ID)) {
			// if we're about to jump left and there's tiles to the left of us, jump right
			if (direction == Direction.Left && leftTouching) {
				direction = Direction.Right;
			} else if (direction == Direction.Right && rightTouching) {
				direction = Direction.Left;
			}

			animator.SetBool("PreparingToJump", true);
		} else { 
			animator.SetBool("PreparingToJump", false);
		}
	}

	protected override void HandleStateChange(string previousStateId, string nextStateId) {
		// if the next state is walking
		if (nextStateId.Equals(WALK_STATE_ID)) {
			// and the previous state was walking
			if (previousStateId.Equals(WALK_STATE_ID)) {
				switch (direction) {
					case Direction.Left:
						direction = Direction.Right;
						break;
					case Direction.Right:
						direction = Direction.Left;
						break;
					default:
						direction = GameUtils.RandomHorizontalDirection();
						break;
				}
			} else {
				// if we're touching a wall to the left, go right and vice versa
				if (leftTouching) {
					direction = Direction.Right;
				} else if (rightTouching) {
					direction = Direction.Left;
				} else {
					direction = GameUtils.RandomHorizontalDirection();
				}
			}
		}
	}

	public void Jump() {
		if (!stateMachine.CurrentStateId().Equals(PREJUMP_STATE_ID)) {
			Debug.LogWarning("Jump - called when current state isn't prejumping");
		}

		switch (direction) {
			case Direction.Left:
				rigidbody2D.AddForce((new Vector2(-1.0f, 1.5f)).normalized * jumpForce);
				break;
			case Direction.Right:
				rigidbody2D.AddForce((new Vector2(1.0f, 1.5f)).normalized * jumpForce);
				break;
			default:
				Debug.LogWarning("Jump - direction improperly set: " + direction);
				break;
		}
	}

	public override void Die ()
	{
		base.Die ();
		// spawn gold at self
		SpawnAtSelf(PrefabManager.Instance.SmallGold, 3);
	}

	public override bool IsDying ()
	{
		return base.IsDying () && grounded;
	}
}
