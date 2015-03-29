using UnityEngine;
using System.Collections;

public class GoldCounterController : MonoBehaviour {

	protected tk2dTextMesh textMesh;

	int cachedGold;

	protected void Start () {

		textMesh = GetComponent<tk2dTextMesh> ();
		cachedGold = PlayerManager.Instance.gold;
		textMesh.text = cachedGold.ToString(); 
	}

	protected void Update() {
		if (cachedGold != PlayerManager.Instance.gold) {
			// update gold count and string
			cachedGold = PlayerManager.Instance.gold;
			textMesh.text = cachedGold.ToString();
		}
	}
}
