using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BloodParticleController : MonoBehaviour {

	protected List<GameObject> clippedSprites;
	protected Dictionary<int, Rect> overlaps;
	protected bool dirtyOverlap;

	protected GameObject clippedSprite;

	protected void Awake() {
		clippedSprites = new List<GameObject>();
		overlaps = new Dictionary<int, Rect>();
		dirtyOverlap = false;
	}

	protected void Start() {
		clippedSprite = PrefabManager.Instance.SlimeBloodClippedSprite as GameObject;
	}

	protected void Update() {
		if (dirtyOverlap) {
			RecomputeClippedSprites();
		}
	}

	protected void OnTriggerEnter2D(Collider2D other) {
		HandleTriggerEnterOrStay(other);
	}

	protected void OnTriggerStay2D(Collider2D other) {
		HandleTriggerEnterOrStay(other);
	}

	protected void OnTriggerExit2D(Collider2D other) {
		Destroy (gameObject);
	}

	protected void HandleTriggerEnterOrStay(Collider2D other) {
		GameObject otherObject = other.gameObject;

		int instanceId = otherObject.GetInstanceID();

		// make sure we aren't hitting this object already in the trigger lifetime
		if (overlaps.ContainsKey(instanceId)) {
			return;
		} 

		Bounds myBound = GetComponent<BoxCollider2D>().bounds;
		Bounds otherBound = other.bounds;
		/*
		Debug.Log ("My bounds: " + myBound);
		Debug.Log ("my transform: " + transform.position);
		Debug.Log ("Other bounds: " + otherBound);
		Debug.Log ("Other transform: " + other.transform.position);
		*/

		float overlapMinX = Mathf.Max(myBound.center.x - myBound.extents.x, otherBound.center.x - otherBound.extents.x);
		float overlapMaxX = Mathf.Min(myBound.center.x + myBound.extents.x, otherBound.center.x + otherBound.extents.x);
		float overlapMinY = Mathf.Max(myBound.center.y - myBound.extents.y, otherBound.center.y - otherBound.extents.y);
		float overlapMaxY = Mathf.Min(myBound.center.y + myBound.extents.y, otherBound.center.y + otherBound.extents.y);

		float width = myBound.extents.x * 2.0f;
		float height = myBound.extents.y * 2.0f;

		float leftPercentagePoint = (overlapMinX - (myBound.center.x - myBound.extents.x)) / width;
		// tk2d clipped sprite uses left, bottom
		float bottomPercentagePoint = (overlapMinY - (myBound.center.y - myBound.extents.y)) / height;
		float widthPercentage = (overlapMaxX - overlapMinX) / width;
		float heightPercentage = (overlapMaxY - overlapMinY) / height;

		Rect overlap = new Rect(RoundToNearestDecimalPoint(leftPercentagePoint), 
		                        RoundToNearestDecimalPoint(bottomPercentagePoint), 
		                        RoundToNearestDecimalPoint(widthPercentage), 
		                        RoundToNearestDecimalPoint(heightPercentage));

		if (overlap.width >= 0.3f && overlap.height >= 0.3f) {
			overlaps.Add(instanceId, overlap);
			dirtyOverlap = true;
			GetComponent<Rigidbody2D>().Sleep();
		}
	}

	protected float RoundToNearestDecimalPoint(float n) {
		return n;
	}

	protected void RecomputeClippedSprites() {
		// turn off regular sprite and box collider
		GetComponent<tk2dSprite>().enabled = false;
		GetComponent<MeshRenderer>().enabled = false;
		GetComponent<BoxCollider2D>().enabled = false;

		// destory all previous clipped sprites 
		foreach (GameObject obj in clippedSprites) {
			Destroy(obj);
		}
		clippedSprites.Clear();

		// create a clipped sprite for each overlap (no more than 4)
		foreach(KeyValuePair<int, Rect> entry in overlaps) {
			if (clippedSprites.Count > 4) {
				break;
			}

			GameObject obj = Instantiate(clippedSprite, transform.position, Quaternion.identity) as GameObject;
			obj.transform.parent = transform;
			obj.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
			tk2dClippedSprite clipped = obj.GetComponent<tk2dClippedSprite>();
			clipped.ClipRect = entry.Value;
			clippedSprites.Add (obj);
		}

		dirtyOverlap = false;
	}
}
