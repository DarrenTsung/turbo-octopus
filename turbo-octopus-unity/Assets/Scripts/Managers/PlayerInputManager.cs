﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerInputManager : Singleton<PlayerInputManager> {

	protected PlayerInputManager () {}

	private PlayerController playerController;

	private KeyCode action0 = KeyCode.Alpha1, action1 = KeyCode.Alpha2, action2 = KeyCode.F;
	private KeyCode leftRoll = KeyCode.Q, rightRoll = KeyCode.E;
	private KeyCode fireKey = KeyCode.Mouse0;
	private KeyCode jumpKey = KeyCode.Space;

	protected bool playerInputAllowed;

	public void SetPlayerController (PlayerController controller) {
		playerController = controller;
		playerInputAllowed = true;
	}

	public void disablePlayerInput() {
		playerInputAllowed = false;
	}

	public void enablePlayerInput() {
		playerInputAllowed = true;
	}

	protected void Update () {
		if (playerController) {
			if (!playerInputAllowed) {
				return;
			}

			// axis movement
			Vector2 axisVector = new Vector2(Input.GetAxis ("Horizontal"), Input.GetAxis ("Vertical"));
			playerController.handleAxisVector (axisVector);
 
			// mouse movement
			Vector2 mousePosition = new Vector2 (Input.mousePosition.x, Input.mousePosition.y);
			playerController.handleMousePosition (mousePosition);

			// actions
			if (Input.GetKeyDown(action0)) {
				playerController.handleActionPressed (0);
			} 
			if (Input.GetKeyDown(action1)) {
				playerController.handleActionPressed (1);
			}
			if (Input.GetKeyDown(action2)) {
				playerController.handleActionPressed (2);
			}

			if (Input.GetKeyDown (leftRoll)) {
				playerController.handleLeftRollPressed ();
			}
			if (Input.GetKeyDown (rightRoll)) {
				playerController.handleRightRollPressed ();
			}

			// repeating checks
			if (Input.GetKey (fireKey)) {
				playerController.handleFireButtonDown ();
			}
			if (Input.GetKeyDown (jumpKey)) {
				playerController.handleJumpButtonDown ();
			}

		}
	}
}
