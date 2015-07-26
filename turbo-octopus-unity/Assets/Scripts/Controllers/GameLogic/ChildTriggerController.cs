using UnityEngine;
using System.Collections;

public class ChildTriggerController : MonoBehaviour {
	public delegate void OnTriggerEnter2DAction(Collider2D other, GameObject child);
	public event OnTriggerEnter2DAction OnTriggerEnter2DEvent;

	public delegate void OnTriggerStay2DAction(Collider2D other, GameObject child);
	public event OnTriggerStay2DAction OnTriggerStay2DEvent;

	public delegate void OnTriggerExit2DAction(Collider2D other, GameObject child);
	public event OnTriggerExit2DAction OnTriggerExit2DEvent;

	void OnTriggerEnter2D(Collider2D other) {
		if (OnTriggerEnter2DEvent != null) {
			OnTriggerEnter2DEvent(other, gameObject); 
		}
	}

	void OnTriggerStay2D(Collider2D other) {
		if (OnTriggerStay2DEvent != null) {
			OnTriggerStay2DEvent(other, gameObject); 
		}
	}

	void OnTriggerExit2D(Collider2D other) {
		if (OnTriggerExit2DEvent != null) {
			OnTriggerExit2DEvent(other, gameObject); 
		}
	}
}
