using UnityEngine;
using System.Collections;

public static class GameUtils {

	private static GameObject topObject;
	public const int FOREGROUND_LAYER = 0;
	public const int COLLIDER_LAYER = 1;
	public const int BACKGROUND_LAYER = 2;

	public const int CORRIDOR_SIZE = 3;

	// sprite ids for the tilemap sprite collection 
	public const int DOOR_SPRITE_ID = 14;
	public const int BACKGROUND_SPRITE_ID = 15;

	// sprite ids for the UI sprite collection
	public const int UI_AMMO_FILLED_ID = 0;
	public const int UI_AMMO_EMPTY_ID = 1;

	public static LayerMask tileLayers, playerLayer;
	public static int UILayer;
	public static Vector3 PIXEL_SIZE;


	static GameUtils () {
		tileLayers = LayerMask.GetMask("Tile");
		playerLayer = LayerMask.GetMask("Player");
		UILayer = LayerMask.NameToLayer("UI");
		topObject = GameObject.Find("Objects");
	}

	public static GameObject GetEnemyControllerGameObject(GameObject obj) {
		Transform currentTransform = obj.transform;
		while (!currentTransform.gameObject.GetComponent<EnemyController>()) {
			// if we can't find it then return null
			if (currentTransform.parent == null) {
				return null;
			}
			currentTransform = currentTransform.parent;
		}
		return currentTransform.gameObject;
	}

	public static float ManhattanDistance(Vector2 a, Vector2 b) {
		return Mathf.Abs(b.x - a.x) + Mathf.Abs(b.y - a.y);
	}

	public static Direction RandomHorizontalDirection() {
		return Random.value < 0.5f ? Direction.Left : Direction.Right;
	}

	public static float PixelsToMeter() {
		return 10.0f;
	}
}

public enum Direction {
	Left, Right, Up, Down
};

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
