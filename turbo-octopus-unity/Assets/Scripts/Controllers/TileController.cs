using UnityEngine;
using System.Collections;

public class TileController : MonoBehaviour {

	public enum TileType {
		TileTop,
		TileBottom,
		TileLeft,
		TileRight,
		TileMiddle,
		TileTopLeft,
		TileTopRight,
		TileBottomLeft,
		TileBottomRight
	};

	public TileType type;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
