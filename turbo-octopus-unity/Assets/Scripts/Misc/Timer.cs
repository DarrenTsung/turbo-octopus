using UnityEngine;
using System.Collections;

public class Timer {

	private float timeLeft;
	public delegate void HandleTimerFinished();
	public event HandleTimerFinished TimerFinished;

	public Timer () {
		timeLeft = -1.0f;
	}

	public void SetTime(float time) {
		timeLeft = time;
	}
	
	public void Update (float deltaTime) {
		float previousTime = timeLeft;
		timeLeft -= deltaTime;
		if (previousTime > 0.0 && timeLeft <= 0.0f) {
			if (TimerFinished != null) {
				TimerFinished();
			}
		}
	}

	public bool IsFinished() {
		return timeLeft <= 0.0f;
	}
}
