using UnityEngine;
using System.Collections;

public class ParticleTextManager : Singleton<ParticleTextManager> {

	protected ParticleTextManager() {}

	void Awake () {
		RegisterForEvents();
	}

	protected void RegisterForEvents() {
		EventManager.DamageDealt += HandleDamageDealt;
		EventManager.GoldAcquired += HandleGoldAcquired;
	}

	protected void HandleGoldAcquired(int gold) {
		Vector3 playerPosition = GameUtils.PlayerInstance().transform.position;
		Vector3 textPosition = new Vector3(playerPosition.x + Random.Range(0.0f, 0.5f), 
		                                   playerPosition.y + Random.Range(0.0f, 0.9f));
		GameObject goldText = Instantiate(PrefabManager.Instance.ParticleText, 
		                                    new Vector3(textPosition.x, textPosition.y), 
		                                    Quaternion.identity) as GameObject;
		ParticleTextController controller = goldText.GetComponent<ParticleTextController>();
		// yellow color
		controller.SetColor(new Color(1.0f, 0.95f, 0.05f, 1.0f));
		controller.SetNumberToDisplay(gold);
		controller.AddPrefixToDisplay("+");
		goldText.transform.parent = transform;
	}

	private void HandleDamageDealt(DamageModel damageModel, GameObject objectBeingDamaged, GameObject damageSource, Vector2 hitPoint) {
		GameObject damageText = Instantiate(PrefabManager.Instance.ParticleText, 
		                                    new Vector3(hitPoint.x, hitPoint.y), 
		                                    Quaternion.identity) as GameObject;
		ParticleTextController controller = damageText.GetComponent<ParticleTextController>();
		controller.SetNumberToDisplay(damageModel.computedDamage);
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
