using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TimerManager : Singleton<TimerManager> {

	protected TimerManager () {}

	protected Dictionary<string, float> timers;

	public void addTimerForKey (string key) {
		addTimerForKey(key, 0.0f);
	}

	public void addTimerForKey (string key, float time) {
		timers.Add(key, time);
	}

	public bool timerDoneForKey (string key) {
		if (!timers.ContainsKey(key)) {
			Debug.LogWarning("In TimerManager: timeDoneForKey called, but key doesn't exist in TimerManager!");
			return false;
		}

		float timeLeft = timers[key];
		if (timeLeft > 0.0f) {
			return false;
		} else {
			return true;
		}
	}

	public void resetTimerForKey (string key, float time) {
		if (!timers.ContainsKey(key)) {
			Debug.LogWarning("In TimerManager: resetTimerForKey called, but key doesn't exist in TimerManager!");
			return;
		}

		timers[key] = time;
	}

	protected void Awake () {
		timers = new Dictionary<string, float> ();
	}

	protected void Update () {
		List<string> keys = new List<string>(timers.Keys);
		foreach (string k in keys) {
			timers[k] -= Time.deltaTime;
		}
	}
}
