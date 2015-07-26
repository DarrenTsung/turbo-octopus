using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TimerManager : Singleton<TimerManager> {

	protected TimerManager () {}

	protected List<Timer> timers, timersToAdd;

	public Timer MakeTimer() {
		Timer t = new Timer();
		timersToAdd.Add(t);
		return t;
	}

	protected void Awake () {
		timers = new List<Timer> ();
		timersToAdd = new List<Timer> ();
	}

	protected void Update () {
		timers.AddRange(timersToAdd);
		timersToAdd.Clear();
		foreach (Timer t in timers) {
			t.Update(Time.deltaTime);
		}
	}
}
