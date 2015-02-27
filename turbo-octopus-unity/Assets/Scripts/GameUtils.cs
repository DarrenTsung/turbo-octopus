using UnityEngine;
using System.Collections;

public static class GameUtils {

	private static GameObject topObject;

	static GameUtils () {
		topObject = GameObject.Find("Objects");
	}

	public static GameObject GetTopLevelObject (GameObject obj) {
		Transform currentTransform = obj.transform;
		while (currentTransform.parent.gameObject != topObject) {
			currentTransform = currentTransform.parent;
		}
		return currentTransform.gameObject;
	}

}
