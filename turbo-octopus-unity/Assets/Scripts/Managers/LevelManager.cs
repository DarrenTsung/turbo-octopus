using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomModel {
	protected GameObject room;
	public tk2dTileMap tilemapReference;

	public RoomModel(Object roomObject) {
		room = roomObject as GameObject;
		tilemapReference = room.GetComponent<tk2dTileMap> ();
	}

	public static Vector3 WorldPositionForVector(Vector3 vec) {
		vec.x -= 0.5f;
		vec.y -= 0.5f;
		return vec;
	}

	public static Vector3 VectorForWorldPosition(Vector3 vec) {
		vec.x += 0.5f;
		vec.y += 0.5f;
		return vec;
	}

	public Vector2 WorldPosition() {
		return RoomModel.WorldPositionForVector(room.transform.position);
	}

	public void SetWorldPosition(Vector2 position) {
		room.transform.position = RoomModel.VectorForWorldPosition(new Vector3(position.x, position.y));
	}

	public RoomModel Clone(Vector2 position) {
		Vector3 worldPosition = RoomModel.VectorForWorldPosition(position);
		GameObject objectsRoot = GameObject.Find ("Objects/Rooms");

		GameObject newRoom = MonoBehaviour.Instantiate(room, worldPosition, Quaternion.identity) as GameObject;
		newRoom.transform.parent = objectsRoot.transform;

		return new RoomModel(newRoom);
	}

	public bool OverlapsWith(RoomModel otherModel) {
		Vector2 wp = WorldPosition();
		Vector2 otherWp = otherModel.WorldPosition();

		float w = tilemapReference.width;
		float h = tilemapReference.height;

		float otherW = otherModel.tilemapReference.width;
		float otherH = otherModel.tilemapReference.height;

		Rect myRect = new Rect(wp.x, wp.y, w, h);
		Rect otherRect = new Rect(otherWp.x, otherWp.y, otherW, otherH);

		return myRect.Overlaps(otherRect);
	}

	public void Dealloc() {
		MonoBehaviour.Destroy(room);
	}
}

public class LevelManager : Singleton<LevelManager> {
	protected LevelManager () {}

	protected List<RoomModel> rooms;
	protected List<RoomModel> clonedRooms;

	protected GameObject player;

	protected int roomCount = 20;
	protected float width = 100.0f, height = 100.0f;
	protected const int MAX_GENERATION_TRIES = 10;

	void Awake() {
		rooms = new List<RoomModel>();
		clonedRooms = new List<RoomModel>();

		player = GameObject.Find ("Objects/Player");

		Object[] roomObjects = Resources.LoadAll("Prefabs/Rooms");
		PopulateRoomModels(roomObjects);
		GenerateRooms();
	}

	protected void PopulateRoomModels(Object[] roomObjects) {
		for (int i=0; i<roomObjects.Length; i++) {
			RoomModel model = new RoomModel(roomObjects[i]);
			rooms.Add(model);
		}
	}

	protected bool CollidingWithOtherRooms(RoomModel model) {
		for (int i=0; i<clonedRooms.Count; i++) {
			// if rooms collide, return true
			RoomModel otherRoom = clonedRooms[i];
			if (otherRoom.OverlapsWith(model)) {
				return true;
			}
		}
		return false;
	}

	protected void GenerateRooms() {
		float minDistance = float.MaxValue;
		RoomModel minRoom = null;
		for (int i=0; i<roomCount; i++) {
			RoomModel randomModel = rooms[Random.Range(0, rooms.Count)];

			// randomly generate rooms (up to MAX_GENERATION_TRIES times) until we find a room that doesn't collide
			int randomX = 0, randomY = 0;
			RoomModel clonedModel = null;
			bool foundNonCollidingRoom = false;

			for (int currentTry = 0; currentTry < MAX_GENERATION_TRIES; currentTry++) {
				randomX = Random.Range (0, (int)width);
				randomY = Random.Range (0, (int)height);
				clonedModel = randomModel.Clone(new Vector2(randomX, randomY));
				if (!CollidingWithOtherRooms(clonedModel)) {
					foundNonCollidingRoom = true;
					break;
				} else {
					clonedModel.Dealloc();
				}
			}

			if (!foundNonCollidingRoom) {
				continue;
			}

			float distance = Mathf.Sqrt(Mathf.Pow(randomX, 2.0f) + Mathf.Pow(randomY, 2.0f));
			if (distance < minDistance) {
				minDistance = distance;
				minRoom = clonedModel;
			}

			clonedRooms.Add (clonedModel);
		}

		// spawn the player in the middle of the chosen room
		Vector2 worldPosition = minRoom.WorldPosition();
		player.transform.position = new Vector3(worldPosition.x + minRoom.tilemapReference.width/2.0f,
		                                        worldPosition.y + minRoom.tilemapReference.height/2.0f,
		                                        player.transform.position.z);
	}
}
