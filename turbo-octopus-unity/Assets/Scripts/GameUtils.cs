using UnityEngine;
using System.Collections;

public static class GameUtils {

	private static GameObject topObject;
	public const int FOREGROUND_LAYER = 0;
	public const int COLLIDER_LAYER = 1;
	public const int BACKGROUND_LAYER = 2;

	public const int CORRIDOR_SIZE = 3;

	// sprite id is found in the sprite collection for the tilemap
	public const int DOOR_SPRITE_ID = 14;
	public const int BACKGROUND_SPRITE_ID = 15;

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

	public static float ManhattanDistance(Vector2 a, Vector2 b) {
		return Mathf.Abs(b.x - a.x) + Mathf.Abs(b.y - a.y);
	}
}

public class Tuple<T1, T2>
{
	public T1 First { get; private set; }
	public T2 Second { get; private set; }
	internal Tuple(T1 first, T2 second)
	{
		First = first;
		Second = second;
	}
}

public static class Tuple
{
	public static Tuple<T1, T2> New<T1, T2>(T1 first, T2 second)
	{
		var tuple = new Tuple<T1, T2>(first, second);
		return tuple;
	}
}
