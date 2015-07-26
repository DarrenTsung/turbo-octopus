using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {
	protected FiniteStateMachine stateMachine;
	protected int health, baseHealth;
	protected bool dying;
	protected Animator animator;
	protected Rigidbody2D myRigidbody; 

	public virtual void OnHit (GameObject obj, Vector2 hitPoint, Vector3 hitForce) {
		DamageController damageController = obj.GetComponent<DamageController> ();
		if (damageController) {
			DamageModel damageModel = damageController.ComputeDamage();
			OnDamage(obj, hitPoint, hitForce, damageModel);
		}
	}

	public virtual void OnDamage (GameObject obj, Vector2 hitPoint, Vector3 hitForce, DamageModel damageModel) {
		health -= damageModel.computedDamage;

		EventManager.CallDamageDealt(damageModel, gameObject, obj, hitPoint);
	}

	protected virtual void SetUpStateMachine() {
		// nothing in base class
	}

	protected virtual void Awake () {
		health = 100;
		baseHealth = 100;
		animator = GetComponent<Animator> ();
		myRigidbody = GetComponent<Rigidbody2D> ();

		stateMachine = gameObject.AddComponent<FiniteStateMachine>() as FiniteStateMachine;
		stateMachine.AddStateChangeAction(HandleStateChange);
		SetUpStateMachine();
	}
	
	protected virtual void Update () {
		// nothing in base class
	}

	protected virtual void HandleStateChange(string previousStateId, string nextStateId) {
		// nothing in base class
	}

	public virtual void Die () {
		Destroy(gameObject);
	}

	public virtual float GetHealth() {
		return health;
	}

	public virtual bool IsDying() {
		return dying;
	}

	protected virtual void SpawnAtSelf(Object obj, int count) {
		for (int i=0; i<count; i++) {
			GameObject cloneObj = Instantiate(obj, transform.position, Quaternion.identity) as GameObject;
			Rigidbody2D cloneRigidbody = cloneObj.GetComponent<Rigidbody2D> ();
			if (cloneRigidbody) {
				float itemForce = 150.0f;
				cloneRigidbody.AddForce(new Vector2 (Random.Range(-itemForce, itemForce), Random.Range (0.0f, itemForce * 1.5f)));
			}
		}
	}
}
