using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {
	protected FiniteStateMachine stateMachine;
	protected float health;
	protected Animator animator;
	protected new Rigidbody2D rigidbody2D;

	public virtual void OnHit (GameObject obj, Vector2 hitPoint) {
	}

	protected virtual void SetUpStateMachine() {
		// nothing in base class
	}

	protected virtual void Awake () {
		health = 10.0f;
		animator = GetComponent<Animator> ();
		rigidbody2D = GetComponent<Rigidbody2D> ();

		stateMachine = gameObject.AddComponent<FiniteStateMachine>() as FiniteStateMachine;
		stateMachine.AddStateChangeAction(HandleStateChange);
		SetUpStateMachine();
	}
	
	protected virtual void Update () {
	
	}

	protected virtual void HandleStateChange(string previousStateId, string nextStateId) {
		// nothing in base class
	}

	public virtual void DestroySelf () {
		Destroy(gameObject);
	}
}
