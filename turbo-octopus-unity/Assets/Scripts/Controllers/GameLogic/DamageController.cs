using UnityEngine;
using System.Collections;

public class DamageModel {
	public bool didCrit;
	public int computedDamage;
}

public class DamageController : MonoBehaviour {

	protected int damageMin, damageMax;
	protected float critChance, critMultiplier;
	protected int comboMultiplier;

	void Awake() {
		comboMultiplier = 1;
	}

	public int maxBaseDamage() {
		return damageMax;
	}

	public int minBaseDamage() {
		return damageMin;
	}

	// critChance = 0.0 -- 1.0
	public void SetUp(int damageMin, int damageMax, float critChance, float critMultiplier) {
		this.damageMax = damageMax;
		this.damageMin = damageMin;
		this.critChance = critChance;
		this.critMultiplier = critMultiplier;
	}

	public void SetComboMultiplier(int comboMultiplier) {
		if (comboMultiplier <= 0) {
			Debug.LogWarning ("SetComboMultiplier - combo multiplier <= 0: " + comboMultiplier);
			comboMultiplier = 1;
		}

		this.comboMultiplier = comboMultiplier;
	}


	public DamageModel ComputeDamage() {
		DamageModel damageModel = new DamageModel();

		bool didCrit = Random.value <= critChance;
		int computedDamage = Random.Range(damageMin, damageMax) * comboMultiplier;
		if (didCrit) {
			computedDamage = (int)(computedDamage * critMultiplier);
		}

		damageModel.didCrit = didCrit;
		damageModel.computedDamage = computedDamage;

		return damageModel;
	}
}
