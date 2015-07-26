using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class VineController : MonoBehaviour {

	[SerializeField] protected GameObject hinge1, hinge2;
	[SerializeField] protected List<GameObject> segments;
	public int segmentCount;

	[SerializeField] protected GameObject hingeObject, segmentObject;
	[SerializeField] protected Vector2 segmentSize;

	protected void InitializeIfNeeded () {
		if (!hingeObject || !segmentObject) {
			hingeObject = Resources.Load ("Prefabs/Objects/VineHinge") as GameObject;
			segmentObject = Resources.Load ("Prefabs/Objects/VineSegment") as GameObject;
		}

		if (segmentSize == null || segmentSize == Vector2.zero) {
			GameObject tempSegment = Instantiate(segmentObject) as GameObject;
			segmentSize = tempSegment.GetComponent<BoxCollider2D>().size;
			DestroyImmediate(tempSegment);
		}

		if (!hinge1 || !hinge2) {
			hinge1 = Instantiate(hingeObject, transform.position, Quaternion.identity) as GameObject;
			hinge1.name = "Hinge1";
			hinge1.transform.parent = transform;
			// disable hinge 1's joint since it won't be connected to anything
			hinge1.GetComponent<HingeJoint2D>().enabled = false;

			hinge2 = Instantiate(hingeObject, transform.position, Quaternion.identity) as GameObject;
			hinge2.name = "Hinge2";
			hinge2.transform.parent = transform;
		}

		if (segments == null) {
			segments = new List<GameObject>();
		}
	}

	public void UpdateSegmentCount() {
		InitializeIfNeeded();

		// remove all previous segments
		for(int i = segments.Count - 1; i >= 0; i--) {
			DestroyImmediate(segments[i]);
		}
		segments.Clear ();

		if (segmentCount <= 0) {
			return;
		}

		for (int i = 1; i <= segmentCount; i++) {
			GameObject segment = Instantiate(segmentObject, 
			                                 hinge1.transform.position + (new Vector3(segmentSize.x, 0.0f) * i),
			                                 Quaternion.identity) as GameObject;
			segment.name = "Segment" + i;
			segment.transform.parent = transform;

			HingeJoint2D hingeJoint = segment.GetComponent<HingeJoint2D>();
			GameObject connectedGameObject = (i == 1) ? hinge1 : segments[i - 2];
			hingeJoint.connectedBody = connectedGameObject.GetComponent<Rigidbody2D>();
			hingeJoint.anchor = new Vector2(-segmentSize.x / 2.0f, 0.0f);
			if (i > 1) { 
				hingeJoint.connectedAnchor = new Vector2(segmentSize.x / 2.0f, 0.0f);
			}

			segments.Add (segment);
		}

		hinge1.transform.position = hinge1.transform.position + new Vector3(segmentSize.x / 2.0f, 0.0f);
		hinge2.transform.position = hinge1.transform.position + (new Vector3(segmentSize.x, 0.0f) * (segmentCount + 1)) - (1.5f * new Vector3(segmentSize.x, 0.0f));

		HingeJoint2D hingeHingeJoint = hinge2.GetComponent<HingeJoint2D>();
		hingeHingeJoint.connectedBody = segments[segmentCount - 1].GetComponent<Rigidbody2D>();
		hingeHingeJoint.connectedAnchor = new Vector2(segmentSize.x / 2.0f, 0.0f);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
