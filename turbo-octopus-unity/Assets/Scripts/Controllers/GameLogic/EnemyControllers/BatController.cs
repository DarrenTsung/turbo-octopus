using UnityEngine;
using System.Collections;

public class BatController : EnemyController {
	protected const string SLEEPING_STATE_ID = "BatSleepingStateId";
	protected const string DROPPING_STATE_ID = "BatDroppingStateId";
	protected const string FLYING_TO_PLAYER_STATE_ID = "BatFlyingToPlayerStateId";
	protected const string STUNNED_STATE_ID = "BatStunnedStateId";
	protected const string DEAD_STATE_ID = "BatDeadStateId";

	// swarming constants
	protected const float NEIGHBORHOOD_RANGE = 4.0f;
	protected const float SEPERATION_RANGE_MIN = 1.5f;
	protected const float SEPERATION_RANGE_MAX = 3.5f;
	protected const float CENTER_OF_SWARM_WEIGHT = 0.5f;
	protected const float INDIVIDUAL_SEPERATION_WEIGHT = 0.6f;
	protected const float TARGET_WEIGHT = 2.0f;
	protected float mySeperationRange;

	protected Transform groundCheck, leftCheck, rightCheck;
	protected bool grounded, leftTouching, rightTouching;

	protected GameObject awakeChildTrigger;

	protected bool dead;
	protected bool goingToAwaken;

	protected float speed;

	protected GameObject target;

	protected override void Awake () {
		base.Awake();

		mySeperationRange = Random.Range (SEPERATION_RANGE_MIN, SEPERATION_RANGE_MAX);

		groundCheck = transform.Find ("GroundCheck");
		leftCheck = transform.Find ("LeftCheck");
		rightCheck = transform.Find ("RightCheck");
		speed = 6.0f;
		health = 10;
		baseHealth = health;
		dead = false;
		goingToAwaken = false;

		awakeChildTrigger = transform.Find ("AwakeTrigger").gameObject;
		ChildTriggerController childTriggerController = awakeChildTrigger.GetComponent<ChildTriggerController>();
		childTriggerController.OnTriggerEnter2DEvent += OnChildTriggerEnter2DEvent;

		target = GameUtils.PlayerInstance();

		animator.SetBool("Asleep", true);
		myRigidbody.gravityScale = 0;
	}

	void OnChildTriggerEnter2DEvent(Collider2D other, GameObject child) {
		if (child == awakeChildTrigger) {
			if (!goingToAwaken) {
				// we should wake up if the player comes into our radius
				Invoke ("Awaken", Random.Range (0.0f, 1.0f));
				goingToAwaken = true;
			}
		}
	}

	public override void OnHit (GameObject obj, Vector2 hitPoint, Vector3 hitForce) {
		if (dead) {
			return;
		}

		base.OnHit(obj, hitPoint, hitForce);

		if (stateMachine.CurrentStateId().Equals(SLEEPING_STATE_ID)) {
			Awaken ();
			return;
		} else if (stateMachine.CurrentStateId().Equals(DROPPING_STATE_ID)) {
			return;
		}

		myRigidbody.AddForce(hitForce);
		if (!stateMachine.CurrentStateId().Equals(STUNNED_STATE_ID)) {
			stateMachine.TransitionToState(STUNNED_STATE_ID);
		}
	}

	public override void OnDamage (GameObject obj, Vector2 hitPoint, Vector3 hitForce, DamageModel damageModel) {
		base.OnDamage(obj, hitPoint, hitForce, damageModel);
	}

	protected override void SetUpStateMachine() {
		// negative min/max time means that the state will not advance without an outside trigger
		stateMachine.AddState(SLEEPING_STATE_ID, -999.0f, -999.0f);
		stateMachine.AddState(DROPPING_STATE_ID, -999.0f, -999.0f);
		stateMachine.AddState(FLYING_TO_PLAYER_STATE_ID, -999.0f, -999.0f);
		stateMachine.AddState(DEAD_STATE_ID, -999.0f, -999.0f);

		// a bat will attempt to get out of stunned state every X to Y seconds
		stateMachine.AddState(STUNNED_STATE_ID, 1.0f, 2.0f);

		stateMachine.AddTransition(SLEEPING_STATE_ID, DROPPING_STATE_ID, 1.0f);

		stateMachine.AddTransition(DROPPING_STATE_ID, FLYING_TO_PLAYER_STATE_ID, 1.0f);

		stateMachine.AddTransition(FLYING_TO_PLAYER_STATE_ID, STUNNED_STATE_ID, 1.0f);

		stateMachine.AddTransition(STUNNED_STATE_ID, FLYING_TO_PLAYER_STATE_ID, 0.8f);
		stateMachine.AddTransition(STUNNED_STATE_ID, STUNNED_STATE_ID, 0.2f);

		stateMachine.SetStartState(SLEEPING_STATE_ID);
	}

	protected override void HandleStateChange(string previousStateId, string nextStateId) {
		if (previousStateId.Equals(STUNNED_STATE_ID) && nextStateId.Equals(STUNNED_STATE_ID)) {
			Debug.Log ("Bat - Stunned to stunned state change, something is wrong");
			return;
		}

		if (nextStateId.Equals(STUNNED_STATE_ID)) {
			animator.SetBool("Stunned", true);
		}
		if (previousStateId.Equals(STUNNED_STATE_ID)) {
			animator.SetBool("Stunned", false);
		}
	}

	protected void Awaken() {
		if (stateMachine.CurrentStateId().Equals(SLEEPING_STATE_ID)) {
			animator.SetBool("Asleep", false);
		} else {
			Debug.LogWarning("Bat - awaken called when not sleeping! Current state: " + stateMachine.CurrentStateId());
		}
	}

	public void AwakenAnimationFinished() {
		stateMachine.TransitionToState(DROPPING_STATE_ID);
		// start applying gravity on the bat
		myRigidbody.gravityScale = 1;
	}

	public void DroppingAnimationFinished() {
		stateMachine.TransitionToState(FLYING_TO_PLAYER_STATE_ID);
		animator.SetBool("FinishedDropAnimation", true);
		EnemyManager.Instance.RegisterActiveBat(gameObject);
	}


	protected override void Update() {
		if (dead) {
			return;
		}

		if (!dying && !stateMachine.CurrentStateId().Equals(DEAD_STATE_ID)) {
			dying = health <= 0.0f;
			if (dying) {
				stateMachine.TransitionToState(DEAD_STATE_ID);
				animator.SetBool ("Stunned", true);
			}
		}

		// if we're dying, but havn't hit the ground yet, we're not "dead"
		// if we hit the ground and we're not dead yet, die
		if (dying && !dead && grounded) {
			Die ();
		}

		Collider2D[] emptyArray = new Collider2D[1];
		int groundedHit = Physics2D.OverlapCircleNonAlloc(groundCheck.position, 0.2f, emptyArray, GameUtils.tileLayers);
		grounded = groundedHit != 0 ? true : false;
		animator.SetBool("Grounded", grounded);

		if (stateMachine.CurrentStateId().Equals(FLYING_TO_PLAYER_STATE_ID)) {
			Vector3 direction = Vector3.zero;

			// go towards the target
			Vector3 offset = new Vector3(Random.Range (-mySeperationRange, mySeperationRange), 
			                             Random.Range (-mySeperationRange, mySeperationRange), 
			                             0.0f);
			Vector3 targetVector = (target.transform.position + offset) - transform.position;
			direction += targetVector.normalized * TARGET_WEIGHT;

			foreach (GameObject otherBat in EnemyManager.Instance.activeBats) {
				Transform otherBatTransform = otherBat.transform;
				Vector3 otherBatTargetVector = otherBatTransform.position - transform.position;

				float distanceToOtherBat = Vector3.Magnitude(otherBatTargetVector);
				if (distanceToOtherBat <= NEIGHBORHOOD_RANGE) {
					// closer the other bat is, the more we want to go in the other direction
					float seperationWeight = Mathf.Clamp ((mySeperationRange - distanceToOtherBat) / mySeperationRange, 0.0f, 1.0f) * INDIVIDUAL_SEPERATION_WEIGHT;
					direction += seperationWeight * -otherBatTargetVector;
				}
			}

			myRigidbody.velocity = direction.normalized * speed;
		}
	}

	public override void Die ()
	{
		dead = true;
		SpawnAtSelf(PrefabManager.Instance.SmallGold, Random.Range (1, 3));
	}
}
