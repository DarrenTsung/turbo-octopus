using UnityEngine;
using System.Collections;
using System.Globalization;

public class PrefabManager : Singleton<PrefabManager> {

	protected PrefabManager () {}

	public Object Bullet, BulletPuff, BulletExplosion;
	public Object Slime, SlimeBlood, SlimeBloodClippedSprite; 

	public Object VineHinge, VineSegment;

	public Object ParticleText, AmmoSprite;
	public Object SmallGold;

	public Object SecondaryDialog;

	void Awake () {
		Bullet = Resources.Load ("Prefabs/Objects/Bullet");
		Slime = Resources.Load ("Prefabs/Objects/Slime");
		VineHinge = Resources.Load ("Prefabs/Objects/VineHinge");
		VineSegment = Resources.Load ("Prefabs/Objects/VineSegment");

		BulletPuff = Resources.Load("Prefabs/SpecialEffects/BulletPuff");
		BulletExplosion = Resources.Load ("Prefabs/SpecialEffects/BulletExplosion");
		ParticleText = Resources.Load ("Prefabs/SPX/ParticleText");
		SmallGold = Resources.Load ("Prefabs/SPX/SmallGold");
		SlimeBloodClippedSprite = Resources.Load ("Prefabs/SPX/SlimeBloodClippedSprite");
		SlimeBlood = Resources.Load ("Prefabs/SPX/SlimeBlood");

		SecondaryDialog = Resources.Load ("Prefabs/UI/SecondaryDialog");
		AmmoSprite = Resources.Load ("Prefabs/UI/AmmoSprite");
	}

	public static GameObject PrefabForName(string name) {
		Object obj;

		CompareInfo myComp = CultureInfo.InvariantCulture.CompareInfo;

		Debug.Log ("name: " + name);
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
