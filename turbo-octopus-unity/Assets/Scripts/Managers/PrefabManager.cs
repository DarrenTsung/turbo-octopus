using UnityEngine;
using System.Collections;

public class PrefabManager : Singleton<PrefabManager> {

	protected PrefabManager () {}

	public Object bulletPuff, bulletExplosion;

	void Awake () {
		bulletPuff = Resources.Load("Prefabs/BulletPuff");
		bulletExplosion = Resources.Load ("Prefabs/BulletExplosion");
	}
}
