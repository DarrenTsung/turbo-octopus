using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	protected float playerSpeed = 6.5f;
	protected bool facingRight = true;

	protected Animator animator;

	protected GameObject head, torso, leftArm, rightArm, legs, gun;
	protected GameObject gunReferencePointRight, gunReferencePointLeft;

	void Start () {
		animator = GetComponent<Animator> ();
		PlayerInputManager.Instance.SetPlayerController(this);

		head = transform.Find ("Torso/Head").gameObject;
		torso = transform.Find ("Torso").gameObject;
		leftArm = transform.Find ("Torso/LeftArm").gameObject;
		rightArm = transform.Find ("Torso/RightArm").gameObject;
		legs = transform.Find ("Legs").gameObject;
		gun = transform.Find ("Torso/Gun").gameObject;

		gunReferencePointRight = gun.transform.Find ("ReferencePointRight").gameObject;
		gunReferencePointLeft = gun.transform.Find ("ReferencePointLeft").gameObject;
	}

	void Update () {
	
	}

	private void flip () {
		facingRight = !facingRight;
		Vector3 scale = transform.localScale;
		scale.x *= -1.0f;
		transform.localScale = scale;
	}

	public void handleAxisVector (Vector2 axisVector) {
		float horizontalSpeed = axisVector.x * playerSpeed;
		animator.SetFloat("horizontalSpeed", Mathf.Abs(horizontalSpeed));

		rigidbody2D.velocity = new Vector2(horizontalSpeed, rigidbody2D.velocity.y);
	}

	public void handleMousePosition (Vector2 mousePosition) {
		Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3 (mousePosition.x, mousePosition.y, 0));

		// flip the player if the angle to the mouse is facing the opposite direction
		Vector3 headRayToMouse = mouseWorldPosition - head.transform.position;
		if ((facingRight && headRayToMouse.x < 0)
		    || (!facingRight && headRayToMouse.x > 0)) {
			flip ();
		}

		pointObjectToPosition(head, mouseWorldPosition);
		pointObjectToPosition(gun, mouseWorldPosition);
		pointObjectToPosition(rightArm, gunReferencePointRight.transform.position);
		pointObjectToPosition(leftArm, gunReferencePointLeft.transform.position);
	}

	public void pointObjectToPosition(GameObject obj, Vector3 position) {
		Vector3 rayToPosition = position - obj.transform.position;
		if (!facingRight) {
			rayToPosition.x *= -1.0f;
		}
		float angleInDegrees = Mathf.Atan2 (rayToPosition.y, rayToPosition.x) * 180.0f / Mathf.PI;
		obj.transform.eulerAngles = new Vector3(0.0f, 0.0f, angleInDegrees);
	}

	public void handleActionDown (int actionNumber) {

	}
}
