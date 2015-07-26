using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyManager : Singleton<EnemyManager> {
	protected EnemyManager () {}

	public List<GameObject> activeBats;

	void Awake() {
		activeBats = new List<GameObject>();
	}

	public void RegisterActiveBat(GameObject bat) {
		activeBats.Add (bat);
	}
}
