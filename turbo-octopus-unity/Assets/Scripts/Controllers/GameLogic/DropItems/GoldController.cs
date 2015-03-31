using UnityEngine;
using System.Collections;

public class GoldController : ItemDropBase {

	public int baseValue;

	protected override void Update() {
		base.Update();

	}

	protected override void HandleHitPlayer() {
		PlayerManager.Instance.gold += baseValue;
	}
}
