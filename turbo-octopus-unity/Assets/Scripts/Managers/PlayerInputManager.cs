using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerInputManager : Singleton<PlayerInputManager> {

	protected PlayerInputManager () {}

	private PlayerController playerController;

	private KeyCode action0 = KeyCode.Q, action1 = KeyCode.E, action2 = KeyCode.F;

	public void SetPlayerController (PlayerController controller) {
		playerController = controller;
	}

	protected void Update () {
		if (playerController) {
			// axis movement
			Vector2 axisVector = new Vector2(Input.GetAxis ("Horizontal"), Input.GetAxis ("Vertical"));
			playerController.handleAxisVector(axisVector);
 
			// mouse movement
			Vector2 mousePosition = new Vector2 (Input.mousePosition.x, Input.mousePosition.y);
			playerController.handleMousePosition(mousePosition);

			// actions
			if (Input.GetKeyDown(action0)) {
				playerController.handleActionDown(0);
			} 
			if (Input.GetKeyDown(action1)) {
				playerController.handleActionDown(1);
			}
			if (Input.GetKeyDown(action2)) {
				playerController.handleActionDown(2);
			}
		}
	}
}
