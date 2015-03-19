using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StateTransition {
	public State state;
	public float weight;

	public StateTransition (State state, float weight) {
		this.state = state;
		this.weight = weight;
	}
}

public class State {
	private List<StateTransition> transitions;
	private float cumulativeWeight;
	private string id;
	private float minTime, maxTime;
	private bool isFinite;

	public State(string id) {
		transitions = new List<StateTransition>();
		cumulativeWeight = 0.0f;
		minTime = 0.0f;
		maxTime = 1.0f;
		this.id = id;
		isFinite = true;
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

		if (min < 0.0f && max < 0.0f) {
			isFinite = false;
		}
	}

	public bool CanTransitionWithTime() {
		return isFinite;
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

/*
 * Example usage:
 * 		FinateStateMachine enemyStateMachine = new FinateStateMachine();
 * 
 * 		enemyStateMachine.AddState("idleState", 1.0f, 2.0f);  		// idle state lasts for 1.0 - 2.0 seconds 
 * 		enemyStateMachine.AddState("walkState", 3.0f, 5.0f);
 * 		
 * 		enemyStateMachine.AddTransition("idleState", "walkState", 1.0f);
 * 		enemyStateMachine.AddTransition("walkState", "idleState", 0.5f);
 * 		enemyStateMachine.AddTransition("walkState", "walkState", 0.5f); 		// walking can sometimes transition to walking
 * 		
 * 		enemyStateMachine.AddStateChangeAction(HandleStateChange);				// add HandleStateChange function to handle state changes
 * */
public class FiniteStateMachine : MonoBehaviour {
	private float stateTimer;
	private State currentState;
	public delegate void StateChangeAction(string previousStateId, string newStateId);
	private event StateChangeAction OnStateChange;
	private Dictionary <string, State> stateHash;

	public void AddState(string stateId, float minTime, float maxTime) {
		State newState = new State(stateId);
		newState.SetMinMaxTime(minTime, maxTime);
		stateHash.Add(newState.GetId (), newState);
	}

	public void AddTransition(string stateIdFrom, string stateIdTo, float weight) {
		if (!stateHash.ContainsKey(stateIdFrom) || !stateHash.ContainsKey(stateIdTo)) {
			Debug.LogWarning("AddTransition - invalid state id; stateFrom: " + stateIdFrom + ". stateTo: " + stateIdTo);
			return;
		}

		State stateFrom = stateHash[stateIdFrom];
		State stateTo = stateHash[stateIdTo];

		StateTransition newTransition = new StateTransition(stateTo, weight);
		stateFrom.AddTransition(newTransition);
	}

	public void SetStartState(string startStateId) {
		if (!stateHash.ContainsKey(startStateId)) {
			Debug.LogWarning("SetStartState - invalid start state id: " + startStateId);
		}

		State startState = stateHash[startStateId];

		currentState = startState;
	}

	void Awake () {
		stateTimer = 0.0f;

		stateHash = new Dictionary<string, State> ();
	}
	
	void Update () {
		stateTimer -= Time.deltaTime;
		if (stateTimer <= 0) {
			if (currentState.CanTransitionWithTime()) {
				AdvanceCurrentState();
			} 
		}
	}

	public void AdvanceCurrentState() {
		State nextState = currentState.AdvanceState ();
		TransitionToState(nextState);
	}

	public void TransitionToState(string stateId) {
		if (!stateHash.ContainsKey(stateId)) {
			Debug.LogWarning("TransitionToState - invalid state id: " + stateId);
			return;
		}

		State stateTo = stateHash[stateId];

		TransitionToState(stateTo);
	}

	protected void TransitionToState(State nextState) {
		OnStateChange(currentState.GetId (), nextState.GetId ());
		currentState = nextState;
		stateTimer = currentState.GenerateTime ();
	}

	public string CurrentStateId () {
		return currentState.GetId ();
	}

	public void AddStateChangeAction (StateChangeAction action) {
		OnStateChange += action;
	}
}
