using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerInputManager : Singleton<PlayerInputManager> {

	protected PlayerInputManager () {}

	private PlayerController playerController;

	private KeyCode action0 = KeyCode.Alpha1, action1 = KeyCode.Alpha2, action2 = KeyCode.F;
	private KeyCode leftRoll = KeyCode.Q, rightRoll = KeyCode.E;
	private KeyCode slashKey = KeyCode.Mouse0;
	private KeyCode fireKey = KeyCode.R;
	private KeyCode jumpKey = KeyCode.Space;

	public bool mouseInputEnabled, movementInputEnabled, rollingEnabled, slashingEnabled, actionsEnabled, shootingEnabled, jumpingEnabled;

	public void SetPlayerController (PlayerController controller) {
		playerController = controller;
		EnablePlayerInput();
	}

	public void DisablePlayerInput() {
		mouseInputEnabled = false;
		movementInputEnabled = false;
		rollingEnabled = false;
		slashingEnabled = false;
		actionsEnabled = false;
		shootingEnabled = false;
		jumpingEnabled = false;
	}

	public void EnablePlayerInput() {
		mouseInputEnabled = true;
		movementInputEnabled = true;
		rollingEnabled = true;
		slashingEnabled = true;
		actionsEnabled = true;
		shootingEnabled = true;
		jumpingEnabled = true;
	}

	public static Vector2 MousePosition() {
		return new Vector2 (Input.mousePosition.x, Input.mousePosition.y);
	}

	public static Vector3 MouseWorldPosition() {
		return Camera.main.ScreenToWorldPoint(new Vector3 (Input.mousePosition.x, Input.mousePosition.y));
	}

	protected void LateUpdate () {
		if (playerController) {
			if (movementInputEnabled) {
				// axis movement
				Vector2 axisVector = new Vector2(Input.GetAxis ("Horizontal"), Input.GetAxis ("Vertical"));
				playerController.handleAxisVector (axisVector);
			}
 
			if (mouseInputEnabled) {
				// mouse movement
				playerController.handleMousePosition ();
			}

			if (actionsEnabled) {
				// actions
				if (Input.GetKeyDown(action0)) {
					playerController.handleActionPressed (0);
				} 
				if (Input.GetKeyDown(action1)) {
					playerController.handleActionPressed (1);
				}
				if (Input.GetKeyDown(action2)) {
					playerController.handleActionPressed (2);
					SecondaryDialogManager.Instance.SpawnDialog("This is a test! This is a test!");
				}
			}

			if (rollingEnabled) {
				if (Input.GetKeyDown (leftRoll)) {
					playerController.handleLeftRollPressed ();
				}
				if (Input.GetKeyDown (rightRoll)) {
					playerController.handleRightRollPressed ();
				}
			}

			if (slashingEnabled) {
				if (Input.GetKeyDown (slashKey)) {
					playerController.handleSlashPressed ();
				}
			}

			if (shootingEnabled) {
				// repeating checks
				if (Input.GetKey (fireKey)) {
					playerController.handleFireButtonDown ();
				}
			}

			if (jumpingEnabled) {
				if (Input.GetKeyDown (jumpKey)) {
					playerController.handleJumpButtonDown ();
				}
			}

			// if (Input.GetKey(KeyCode.F)) {
			// 	Debug.Break();
			// }
		}
	}
}
