using UnityEngine;
using System.Collections;

public class AmmoSpriteController : MonoBehaviour {
	public bool filled;
	private tk2dSprite sprite;

	// Use this for initialization
	void Start () {
		filled = true;
		sprite = GetComponent<tk2dSprite>();
		sprite.SetSprite(GameUtils.UI_AMMO_FILLED_ID);
	}

	void Toggle() {
		filled = !filled;
		if (filled) {
			sprite.SetSprite(GameUtils.UI_AMMO_FILLED_ID);
		} else { 
			sprite.SetSprite(GameUtils.UI_AMMO_EMPTY_ID);
		}
	}
}
