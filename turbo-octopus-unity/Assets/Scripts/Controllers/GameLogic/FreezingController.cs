using UnityEngine;
using System.Collections;

public class FreezingController : MonoBehaviour {
	protected Timer disabledTimer;
	protected Rigidbody2D rigidbodyReference;

	protected Vector2 storedVelocity;

	public void FreezeForTime(float time) {
		disabledTimer.SetTime(time);
		storedVelocity = rigidbodyReference.velocity;
		Debug.Log ("Stored velocity: " + storedVelocity);
		rigidbodyReference.isKinematic = true;
	}

	protected void HandleDisabledTimerFinish() {
		if (rigidbodyReference) {
			rigidbodyReference.isKinematic = false;
			rigidbodyReference.velocity = storedVelocity;
			Debug.Log ("Restoring stored velocity: " + storedVelocity);
		}
	}
	
	protected void Start () {
		disabledTimer = TimerManager.Instance.MakeTimer();
		disabledTimer.TimerFinished += HandleDisabledTimerFinish;
		rigidbodyReference = GetComponent<Rigidbody2D>();
		rigidbodyReference.isKinematic = false;
	}
}
