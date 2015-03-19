using UnityEngine;
using System.Collections;

public class DamageTextManager : Singleton<DamageTextManager> {

	protected DamageTextManager() {}

	void Awake () {
		RegisterForEvents();
	}

	protected void RegisterForEvents() {
		EventManager.DamageDealt += HandleDamageDealt;
	}

	private void HandleDamageDealt(DamageModel damageModel, GameObject objectBeingDamaged, GameObject damageSource, Vector2 hitPoint) {
		GameObject damageText = Instantiate(PrefabManager.Instance.DamageText, 
		                                    new Vector3(hitPoint.x, hitPoint.y), 
		                                    Quaternion.identity) as GameObject;
		DamageTextController controller = damageText.GetComponent<DamageTextController>();
		controller.SetDamage(damageModel.computedDamage);
		controller.SetScale(ScaleForDamage(damageModel));
		damageText.transform.parent = transform;
	}

	private float ScaleForDamage(DamageModel damageModel) {
		// start it based off raw damage
		float scaling = 1.0f + (damageModel.computedDamage / 5.0f) * 0.1f;
		scaling = Mathf.Min (1.6f, scaling);
		if (damageModel.didCrit) {
			scaling = 1.6f;
		}

		return scaling;
	}
}
