using UnityEngine;
using System.Collections;
using System.Globalization;

public class PrefabManager : Singleton<PrefabManager> {

	protected PrefabManager () {}

	public Object Bullet, BulletPuff, BulletExplosion;
	public Object Slime; 

	public Object ParticleText, AmmoSprite;
	public Object SmallGold;

	public Object SecondaryDialog;

	void Awake () {
		BulletPuff = Resources.Load("Prefabs/SpecialEffects/BulletPuff");
		BulletExplosion = Resources.Load ("Prefabs/SpecialEffects/BulletExplosion");
		Bullet = Resources.Load ("Prefabs/Objects/Bullet");
		Slime = Resources.Load ("Prefabs/Objects/Slime");
		ParticleText = Resources.Load ("Prefabs/SPX/ParticleText");
		AmmoSprite = Resources.Load ("Prefabs/UI/AmmoSprite");
		SmallGold = Resources.Load ("Prefabs/SPX/SmallGold");
		SecondaryDialog = Resources.Load ("Prefabs/UI/SecondaryDialog");
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
