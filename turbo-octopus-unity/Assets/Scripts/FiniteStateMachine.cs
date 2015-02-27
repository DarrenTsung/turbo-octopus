using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IStateMachineDelegate {
	void HandleStateChange(string previousId, string newId);
}

public class StateTransition : MonoBehaviour {
	public State state;
	public float weight;

	StateTransition (State state, float weight) {
		this.state = state;
		this.weight = weight;
	}
}

public class State : MonoBehaviour {
	private List<StateTransition> transitions;
	private float cumulativeWeight;
	private string id;
	private float minTime, maxTime;

	void Start () {
		transitions = new List<StateTransition>();
		cumulativeWeight = 0.0f;
		minTime = 0.0f;
		maxTime = 1.0f;
		id = "DefaultId";
	}

	public void SetId(string id) {
		this.id = id;
	}

	public string GetId() {
		return id;
	}

	public void SetMinMaxTime(float min, float max) {
		minTime = min;
		maxTime = max;
	}

	public void AddTransition (StateTransition transition) {
		transitions.Add (transition);
		cumulativeWeight += transition.weight;
	}

	public State AdvanceState () {
		if (transitions.Count <= 0) {
			Debug.LogError("AdvanceState() called when there are no states to transition to");
			throw new UnityException();
		}

		float chosen = Random.value * cumulativeWeight;
		foreach (StateTransition transition in transitions) {
			chosen -= transition.weight;
			if (chosen <= 0) {
				return transition.state;
			}
		}
		return transitions[0].state;
	}

	public float GenerateTime () {
		return Random.Range(minTime, maxTime);
	}
}

public class FiniteStateMachine : MonoBehaviour {
	private float stateTimer;
	private State currentState;
	public delegate void StateChangeAction(string previousStateId, string newStateId);
	private event StateChangeAction OnStateChange;

	void Start () {
		stateTimer = 0.0f;
	}
	
	void Update () {
		stateTimer -= Time.deltaTime;
		if (stateTimer <= 0) {
			State nextState = currentState.AdvanceState ();
			OnStateChange(currentState.GetId (), nextState.GetId ());
			stateTimer = currentState.GenerateTime ();
		}
	}

	void AddStateChangeAction (StateChangeAction action) {
		OnStateChange += action;
	}
}
