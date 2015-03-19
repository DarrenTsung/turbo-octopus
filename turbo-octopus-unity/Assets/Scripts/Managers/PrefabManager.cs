using UnityEngine;
using System.Collections;
using System.Globalization;

public class PrefabManager : Singleton<PrefabManager> {

	protected PrefabManager () {}

	public Object Bullet, BulletPuff, BulletExplosion;
	public Object Slime; 

	public Object DamageText;

	void Awake () {
		BulletPuff = Resources.Load("Prefabs/SpecialEffects/BulletPuff");
		BulletExplosion = Resources.Load ("Prefabs/SpecialEffects/BulletExplosion");
		Bullet = Resources.Load ("Prefabs/Objects/Bullet");
		Slime = Resources.Load ("Prefabs/Objects/Slime");
		DamageText = Resources.Load ("Prefabs/UI/DamageText");
	}

	public static GameObject PrefabForName(string name) {
		Object obj;

		CompareInfo myComp = CultureInfo.InvariantCulture.CompareInfo;

		if (myComp.IsPrefix(name, ("Slime"))) {
			obj = PrefabManager.Instance.Slime;
		} else if (myComp.IsPrefix(name, "Bullet")) {
			obj = PrefabManager.Instance.Bullet;
		} else {
			Debug.LogWarning("PrefabForName - no prefab found for string: " + name);
			return null;
		}

		return obj as GameObject;
	}
}
