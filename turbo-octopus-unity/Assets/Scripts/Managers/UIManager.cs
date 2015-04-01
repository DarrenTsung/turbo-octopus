using UnityEngine;
using System.Collections;

public class UIManager : Singleton<UIManager> {

	protected UIManager () {}

	public GameObject BottomLeftAnchor, TopLeftAnchor;

	public void Awake() {
		BottomLeftAnchor = transform.Find ("BottomLeftAnchor").gameObject;
		TopLeftAnchor = transform.Find ("TopLeftAnchor").gameObject;
	}
}
