using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {
	protected float health;

	public virtual void OnHit (GameObject obj) {

	}

	void Start () {
		health = 10.0f;
	}
	
	void Update () {
	
	}
}
