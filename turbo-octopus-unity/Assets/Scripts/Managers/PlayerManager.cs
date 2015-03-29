using UnityEngine;
using System.Collections;

public class PlayerManager : Singleton<PlayerManager> {

	protected PlayerManager () {}

	public float health, maxHealth;
	public int ammo, maxAmmo;
	public int gold;

	void Awake () {
		gold = 0;
	}
}
