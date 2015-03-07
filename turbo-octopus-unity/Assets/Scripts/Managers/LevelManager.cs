using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CorridorDigger {
	protected Vector2 centerPosition;
	protected Orientation orientation;
	protected CorridorDigger targetDigger;

	public CorridorDigger (Vector2 doorPosition, Orientation orientation) {
		this.orientation = orientation;
		this.centerPosition = doorPosition;
		switch (this.orientation) {
			case Orientation.Down:
				this.centerPosition = DownNode();
				break;
			case Orientation.Left:
				this.centerPosition = LeftNode();
				break;
			case Orientation.Right:
				this.centerPosition = RightNode();
				break;
			case Orientation.Up:
				this.centerPosition = UpNode();
				break;
		}
	}

	public void SetTargetDigger(CorridorDigger targetDigger) {
		this.targetDigger = targetDigger;
	}

	protected Vector2 LeftNode() {
		return new Vector2(centerPosition.x - (GameUtils.CORRIDOR_SIZE / 2.0f), centerPosition.y);
	}

	protected Vector2 RightNode() {
		return new Vector2(centerPosition.x + (GameUtils.CORRIDOR_SIZE / 2.0f) + 1.0f, centerPosition.y);
	}

	protected Vector2 UpNode() {
		return new Vector2(centerPosition.x, centerPosition.y + (GameUtils.CORRIDOR_SIZE / 2.0f) + 1.0f);
	}

	protected Vector2 DownNode() {
		return new Vector2(centerPosition.x, centerPosition.y - (GameUtils.CORRIDOR_SIZE / 2.0f));
	}

	protected Vector2 BottomLeftNode() {
		return new Vector2(centerPosition.x - (GameUtils.CORRIDOR_SIZE / 2.0f),
		                   centerPosition.y - (GameUtils.CORRIDOR_SIZE / 2.0f));
	}

	public void AdvanceOneStep() {
		Vector2 movement = new Vector2(0, 0);

		TurnIfNeeded();

		switch (this.orientation) {
			case Orientation.Down:
				movement = new Vector2(0, -1);
				break;
			case Orientation.Left:
				movement = new Vector2(-1, 0);
				break;
			case Orientation.Right:
				movement = new Vector2(1, 0);
				break;
			case Orientation.Up:
				movement = new Vector2(0, 1);
				break;
		}
		centerPosition += movement;
	}

	protected void TurnIfNeeded() {
		// vertically aligned
		if (targetDigger.centerPosition.x == centerPosition.x) {
			if (orientation == Orientation.Left || orientation == Orientation.Right) {
				orientation = (targetDigger.centerPosition.y > centerPosition.y) ? Orientation.Up : Orientation.Down;
			}
		}

		// horizontally aligned
		if (targetDigger.centerPosition.y == centerPosition.y) {
			if (orientation == Orientation.Up || orientation == Orientation.Down) {
				orientation = (targetDigger.centerPosition.x > centerPosition.x) ? Orientation.Right : Orientation.Left;
			}
		}

		// if we're heading the wrong way horizontally, change direction 
		if ((orientation == Orientation.Right && targetDigger.centerPosition.x < centerPosition.x)
		    || (orientation == Orientation.Left && targetDigger.centerPosition.x > centerPosition.x)) {
				orientation = (targetDigger.centerPosition.y > centerPosition.y) ? Orientation.Up : Orientation.Down;
		}

		// if we're heading the wrong way vertically, change direction
		if ((orientation == Orientation.Up && targetDigger.centerPosition.y < centerPosition.y)
		    || (orientation == Orientation.Down && targetDigger.centerPosition.y > centerPosition.y)) {
			orientation = (targetDigger.centerPosition.x > centerPosition.x) ? Orientation.Right : Orientation.Left;
		}
	}

	public bool Finished() {
		return targetDigger.centerPosition == centerPosition;
	}

	public void PaintBackgroundOntoTilemap(tk2dTileMap tilemap) {
		Vector2 bottomLeftPosition = BottomLeftNode();
		for (int x=0; x<GameUtils.CORRIDOR_SIZE; x++) {
			for (int y=0; y<GameUtils.CORRIDOR_SIZE; y++) {
				Vector2 paintPosition = new Vector2(bottomLeftPosition.x + x, bottomLeftPosition.y + y);

				// Mathf.Repeat is the same as fmod
				if (Mathf.Repeat(paintPosition.x, 1.0f) != 0.0f || Mathf.Repeat(paintPosition.y, 1.0f) != 0.0f) {
					Debug.LogWarning("paintPosition.x and paintPosition.y are not integers! Things are wrong.");
				}

				tilemap.SetTile((int)paintPosition.x, (int)paintPosition.y, GameUtils.BACKGROUND_LAYER, GameUtils.BACKGROUND_SPRITE_ID);
			}
		}
	}
}

public class CorridorModel {
	public RoomModel room1, room2;
	public float minDistance;
	public DoorModel door1, door2;

	public CorridorModel(RoomModel room1, RoomModel room2, float minDistance, DoorModel door1, DoorModel door2) {
		this.room1 = room1;
		this.room2 = room2;
		this.minDistance = minDistance;
		this.door1 = door1;
		this.door2 = door2;
	}

	public void PaintOntoTilemap(GameObject tilemapObject) {
		tk2dTileMap tilemap = tilemapObject.GetComponent<tk2dTileMap>();

		Vector2 door1Position = room1.instancePosition + door1.center;
		Vector2 door2Position = room2.instancePosition + door2.center;

		CorridorDigger c1, c2;
		c1 = new CorridorDigger(door1Position, door1.orientation);
		c2 = new CorridorDigger(door2Position, door2.orientation);

		c1.SetTargetDigger(c2);
		c2.SetTargetDigger(c1);

		// paint initial positions onto tilemap
		c1.PaintBackgroundOntoTilemap(tilemap);
		c2.PaintBackgroundOntoTilemap(tilemap);

		while (true) {
			c1.AdvanceOneStep();
			c1.PaintBackgroundOntoTilemap(tilemap);
			if (c1.Finished()) {
				break;
			}

			c2.AdvanceOneStep();
			c2.PaintBackgroundOntoTilemap(tilemap);
			if (c2.Finished()) {
				break;
			}
		}
	}
}

public enum Orientation {Right, Left, Up, Down};

public class DoorModel {
	public Orientation orientation;
	public int length;
	public Vector2 center;

	public DoorModel(Orientation orientation, int length, Vector2 center) {
		this.orientation = orientation;
		this.length = length;
		this.center = center;
	}
}

public class RoomModel {
	public GameObject room;
	public tk2dTileMap tilemapReference;
	public Vector2 instancePosition;

	public string id;

	protected List<DoorModel> doors;

	public RoomModel(Object roomObject) {
		room = roomObject as GameObject;
		tilemapReference = room.GetComponent<tk2dTileMap> ();
		instancePosition = room.transform.position;
	}

	public void ExtractDoors() {
		doors = new List<DoorModel>();

		int currentDoorLength = 0;

		// horizontal scan for doors
		int startX = -1;
		for (int y = 0; y < tilemapReference.height; y += tilemapReference.height - 1) {
			for (int x = 0; x < tilemapReference.width; x++) {
				// if we found a door tile
				if (tilemapReference.GetTile(x, y, GameUtils.COLLIDER_LAYER) == GameUtils.DOOR_SPRITE_ID) {
					// if startX is uninitialized, then we're starting a new door
					if (startX == -1) {
						startX = x;
					}
					currentDoorLength++;
				} else {
					// if startX is initialized, then we need to finish the creation of the door
					if (startX != -1) {
						int lastX = x - 1;
						Orientation orientation = (y == 0) ? Orientation.Down : Orientation.Up;
						DoorModel newDoor = new DoorModel(orientation, 
						                                  currentDoorLength, 
						                                  new Vector2((currentDoorLength / 2.0f) + startX, y));
						doors.Add(newDoor);
						currentDoorLength = 0;
						startX = -1;
					}
				}
			}
		}

		// vertical scan for doors
		int startY = -1;
		for (int x = 0; x < tilemapReference.width; x += tilemapReference.width - 1) {
			for (int y = 0; y < tilemapReference.height; y++) {
				// if we found a door tile
				if (tilemapReference.GetTile(x, y, GameUtils.COLLIDER_LAYER) == GameUtils.DOOR_SPRITE_ID) {
					// if startY is uninitialized, then we're starting a new door
					if (startY == -1) {
						startY = y;
					}
					currentDoorLength++;
				} else {
					// if startX is initialized, then we need to finish the creation of the door
					if (startY != -1) {
						int lastY = y - 1;
						Orientation orientation = (x == 0) ? Orientation.Left : Orientation.Right;
						DoorModel newDoor = new DoorModel(orientation, 
						                                  currentDoorLength, 
						                                  new Vector2(x, (currentDoorLength / 2.0f) + startY));
						doors.Add(newDoor);
						currentDoorLength = 0;
						startY = -1;
					}
				}
			}
		}

		if (doors.Count == 0) {
			Debug.LogWarning("Extract door - failed to extract any doors!");
		}
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
		return RoomModel.WorldPositionForVector(instancePosition);
	}

	public void SetWorldPosition(Vector2 position) {
		instancePosition = RoomModel.VectorForWorldPosition(new Vector3(position.x, position.y));
		RefreshId();
	}

	public void RefreshId() {
		id = room.name + "_" + instancePosition.x + "_" + instancePosition.y;
	}

	public RoomModel Clone(GameObject room, Vector2 position) {
		RoomModel clonedRoomModel = new RoomModel(room);
		clonedRoomModel.instancePosition = position;
		clonedRoomModel.RefreshId();
		return clonedRoomModel;
	}

	public bool OverlapsWith(RoomModel otherModel) {
		Vector2 wp = WorldPosition() - new Vector2(GameUtils.CORRIDOR_SIZE, GameUtils.CORRIDOR_SIZE); 
		Vector2 otherWp = otherModel.WorldPosition() - new Vector2(GameUtils.CORRIDOR_SIZE, GameUtils.CORRIDOR_SIZE);

		float w = tilemapReference.width + 2.0f * GameUtils.CORRIDOR_SIZE;
		float h = tilemapReference.height + 2.0f * GameUtils.CORRIDOR_SIZE;

		float otherW = otherModel.tilemapReference.width + 2.0f * GameUtils.CORRIDOR_SIZE;
		float otherH = otherModel.tilemapReference.height + 2.0f * GameUtils.CORRIDOR_SIZE;

		Rect myRect = new Rect(wp.x, wp.y, w, h);
		Rect otherRect = new Rect(otherWp.x, otherWp.y, otherW, otherH);

		return myRect.Overlaps(otherRect);
	}

	public void PaintOntoTilemap(GameObject tilemapObject) {
		Vector3 otherWorldPosition = RoomModel.WorldPositionForVector(tilemapObject.transform.position);
		Vector2 offset = WorldPosition() - new Vector2(otherWorldPosition.x, otherWorldPosition.y);

		if (offset.x < 0 || offset.y < 0) {
			Debug.LogWarning("Attempted to paint onto non-valid area of tilemap! Aborting.");
			return;
		}

		// Mathf.Repeat is the same as fmod
		if (Mathf.Repeat(offset.x, 1.0f) != 0.0f || Mathf.Repeat(offset.y, 1.0f) != 0.0f) {
			Debug.LogWarning("Offset.x and offset.y are not integers! Things are wrong.");
		}

		tk2dTileMap otherTilemap = tilemapObject.GetComponent<tk2dTileMap> ();
		tk2dTileMap myTilemap = tilemapReference;

		tk2dRuntime.TileMap.Layer[] layers = myTilemap.Layers;

		for (int layer=0; layer<layers.Length; layer++) {
			for (int x=0; x<myTilemap.width; x++) {
				for (int y=0; y<myTilemap.height; y++) {
					int convertedX = x + (int)offset.x;
					int convertedY = y + (int)offset.y;

					if (convertedX >= otherTilemap.width || convertedY >= otherTilemap.height) {
						Debug.LogWarning("Attempted to paint past end of valid of tilemap!");
						continue;
					}

					otherTilemap.SetTile(convertedX, convertedY, layer, myTilemap.GetTile(x, y, layer));
				}
			}
		}
	}

	public Tuple<DoorModel, DoorModel> MinimumDistanceToOtherRoom(RoomModel otherRoom) {
		float minimumDistance = float.MaxValue;
		DoorModel myMinDoor = null, otherMinDoor = null;

		if (doors.Count == 0) {
			Debug.LogWarning("Warning: MinimumDistanceToOtherRoom - room " + room.name + " has no doors, will return an empty tuple");
		}
		if (otherRoom.doors.Count == 0) {
			Debug.LogWarning("Warning: MinimumDistanceToOtherRoom - otherRoom has no doors, will return an empty tuple");
		}

		for (int i=0; i<doors.Count; i++) {
			DoorModel myDoor = doors[i];
			for (int j=0; j<otherRoom.doors.Count; j++) {
				DoorModel otherDoor = otherRoom.doors[j];

				float distance = GameUtils.ManhattanDistance(myDoor.center + instancePosition, otherDoor.center + otherRoom.instancePosition);
				if (distance < minimumDistance) {
					minimumDistance = distance;
					myMinDoor = myDoor;
					otherMinDoor = otherDoor;
				}
			}
		}

		return new Tuple<DoorModel, DoorModel>(myMinDoor, otherMinDoor);
	}
}

public class LevelManager : Singleton<LevelManager> {
	protected LevelManager () {}

	protected List<RoomModel> rooms;
	protected List<RoomModel> clonedRooms;

	protected GameObject player;
	protected GameObject levelTilemap;

	protected int roomCount = 5;
	protected float width = 100.0f, height = 100.0f;
	protected const int MAX_GENERATION_TRIES = 10;

	void Awake() {
		rooms = new List<RoomModel>();
		clonedRooms = new List<RoomModel>();

		player = GameObject.Find("Objects/Player");
		levelTilemap = GameObject.Find("Objects/LevelTilemap");

		Object[] roomObjects = Resources.LoadAll("Prefabs/Rooms");
		PopulateRoomModels(roomObjects);
		GenerateRooms();
		GenerateCorridors();
		// rebuild the tilemap
		levelTilemap.GetComponent<tk2dTileMap>().Build();
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

			// randomly generate room (up to MAX_GENERATION_TRIES times) until we find a position that doesn't collide
			int randomX = 0, randomY = 0;
			RoomModel clonedModel = null;
			bool foundNonCollidingRoom = false;

			for (int currentTry = 0; currentTry < MAX_GENERATION_TRIES; currentTry++) {
				randomX = Random.Range (0, (int)width);
				randomY = Random.Range (0, (int)height);
				clonedModel = randomModel.Clone(randomModel.room, new Vector2(randomX, randomY));
				if (!CollidingWithOtherRooms(clonedModel)) {
					foundNonCollidingRoom = true;
					break;
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

			clonedModel.ExtractDoors();
			clonedRooms.Add (clonedModel);
			// paint the cloned room into the level tilemap
			clonedModel.PaintOntoTilemap(levelTilemap);
		}

		// spawn the player in the middle of the chosen room
		Vector2 worldPosition = minRoom.WorldPosition();
		player.transform.position = new Vector3(worldPosition.x + minRoom.tilemapReference.width/2.0f,
		                                        worldPosition.y + minRoom.tilemapReference.height/2.0f,
		                                        player.transform.position.z);
	}

	protected void GenerateCorridors() {
		List<CorridorModel> edges = new List<CorridorModel>();

		// generate the minimum distance (between all the doors) between every room
		for (int i=0; i<clonedRooms.Count; i++) {
			RoomModel room = clonedRooms[i];
			for (int j=i+1; j<clonedRooms.Count; j++) {
				RoomModel otherRoom = clonedRooms[j];

				if (string.Compare(otherRoom.id, room.id) == 0) {
					continue;
				}

				Tuple<DoorModel, DoorModel> doorTuple = room.MinimumDistanceToOtherRoom(otherRoom);
				float minDistance = GameUtils.ManhattanDistance(doorTuple.First.center + room.instancePosition,
				                                                doorTuple.Second.center + otherRoom.instancePosition);
				CorridorModel corridorModel = new CorridorModel(room, otherRoom, minDistance, doorTuple.First, doorTuple.Second);
				edges.Add(corridorModel);
			}
		}

		edges.Sort(CompareByMinDistance);

		List<CorridorModel> chosenEdges = ConstructMinimumSpanningTree(edges);

		// remove chosen edges from edges
		foreach (CorridorModel model in chosenEdges) {
			edges.Remove(model);
		}

		// go through redundant vertices and randomly choose to keep a few to create some cycles

		// paint the corridors onto the tilemap
		foreach (CorridorModel model in chosenEdges) {
			model.PaintOntoTilemap(levelTilemap);
		}
	}

	private List<CorridorModel> ConstructMinimumSpanningTree(List<CorridorModel> edges) {
		List<CorridorModel> chosenEdges = new List<CorridorModel>();

		// construct a minimum spanning tree (kruskals)
		int currentTreeCount = 0;
		Dictionary<RoomModel, int> treeMap = new Dictionary<RoomModel, int>();
		for (int a=0; a<edges.Count; a++) {
			CorridorModel corridorModel = edges[a];

			bool addToChosenEdges = false;
			Debug.Log ("Room1: " + corridorModel.room1.id);
			Debug.Log ("Room2: " + corridorModel.room2.id);
			bool contains1 = treeMap.ContainsKey(corridorModel.room1);
			bool contains2 = treeMap.ContainsKey(corridorModel.room2);

			Debug.Log ("Contains: (" + contains1 + ", " + contains2 + ")");
			if (contains1) {
				Debug.Log ("Tree1: " + treeMap[corridorModel.room1]);
			} 
			if (contains2) {
				Debug.Log ("Tree2: " + treeMap[corridorModel.room2]);
			}


			// if both rooms are in a tree
			if (contains1 && contains2) {
				// if they are different trees, convert one tree into the other
				if (treeMap[corridorModel.room1] != treeMap[corridorModel.room2]) {
					List<RoomModel> nodesToConvert = new List<RoomModel>();

					foreach (KeyValuePair<RoomModel, int> entry in treeMap) {
						if (entry.Value == treeMap[corridorModel.room2]) {
							nodesToConvert.Add(entry.Key);
						}
					}

					for (int i=0; i<nodesToConvert.Count; i++) {
						treeMap[nodesToConvert[i]] = treeMap[corridorModel.room1];
					}

					addToChosenEdges = true;
				}
			} else if (contains1 && !contains2) {
				// convert the node to the tree it's joining
				treeMap[corridorModel.room2] = treeMap[corridorModel.room1];
				addToChosenEdges = true;
			} else if (!contains1 && contains2) {
				// convert the node to the tree it's joining
				treeMap[corridorModel.room1] = treeMap[corridorModel.room2];
				addToChosenEdges = true;
			} else {
				// if neither rooms are in a tree, create a new tree
				treeMap[corridorModel.room1] = currentTreeCount;
				treeMap[corridorModel.room2] = currentTreeCount;
				currentTreeCount++;
				addToChosenEdges = true;
			}

			if (addToChosenEdges) {
				chosenEdges.Add(corridorModel);

				// we're finished when the number of edges == number of vertices - 1
				if (chosenEdges.Count == clonedRooms.Count - 1) {
					break;
				}
			}
		}

		return chosenEdges;
	}

	private static int CompareByMinDistance(CorridorModel a, CorridorModel b) {
		return a.minDistance.CompareTo(b.minDistance);
	}
}
