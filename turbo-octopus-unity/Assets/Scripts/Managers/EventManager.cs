using UnityEngine;
using System.Collections;

public class EventManager : Singleton<EventManager> {

	protected EventManager() {}

	// damange is being dealt
	public delegate void HandleDamageDealt(DamageModel damageModel, GameObject objectBeingDamaged, GameObject damageSource, Vector2 hitPoint);
	public static event HandleDamageDealt DamageDealt;

	// gold is getting acquired
	public delegate void HandleGoldAcquired(int gold);
	public static event HandleGoldAcquired GoldAcquired;

	void Awake () {
	
	}

	public static void CallDamageDealt(DamageModel damageModel, GameObject objectBeingDamaged, GameObject damageSource, Vector2 hitPoint) {
		if (DamageDealt != null) {
			DamageDealt(damageModel, objectBeingDamaged, damageSource, hitPoint);
		}
	}

	public static void CallGoldAcquired(int gold) {
		if (GoldAcquired != null) {
			GoldAcquired(gold);
		}
	}
}
