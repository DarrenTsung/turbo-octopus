using UnityEngine;
using System.Collections;

public class PlayerManager : Singleton<PlayerManager> {

	protected PlayerManager () {}

	public const float magnetDistance = 3.0f;

	public float health, maxHealth;
	public int ammo, maxAmmo;
	public int gold;

	void Awake () {
		RegisterForEvents();
		gold = 0;
	}

	protected void RegisterForEvents() {
		EventManager.GoldAcquired += HandleGoldAcquired;
	}

	protected void HandleGoldAcquired(int gold) {
		this.gold += gold;
	}
}
