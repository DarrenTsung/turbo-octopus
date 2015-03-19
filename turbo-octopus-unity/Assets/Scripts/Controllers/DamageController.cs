using UnityEngine;
using System.Collections;

public class DamageModel {
	public bool didCrit;
	public int computedDamage;
}

public class DamageController : MonoBehaviour {

	public int computedDamage;
	public bool didCrit;

	protected int damageMin, damageMax;
	protected float critChance, critMultiplier;
	// critChance = 0.0 -- 1.0
	public void SetUp(int damageMin, int damageMax, float critChance, float critMultiplier) {
		this.damageMax = damageMax;
		this.damageMin = damageMin;
		this.critChance = critChance;
		this.critMultiplier = critMultiplier;
	}


	public DamageModel ComputeDamage() {
		DamageModel damageModel = new DamageModel();

		bool didCrit = Random.value <= critChance;
		int computedDamage = Random.Range(damageMin, damageMax);
		if (didCrit) {
			computedDamage = (int)(computedDamage * critMultiplier);
		}

		damageModel.didCrit = didCrit;
		damageModel.computedDamage = computedDamage;

		return damageModel;
	}
}
