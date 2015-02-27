using UnityEngine;
using System.Collections;

public class SluggyController : EnemyController {

	private float moveForce = 600.0f;
	private float velocity = 1.0f;
	private Animator animator;

	public override void OnHit (GameObject obj) {
		BulletController bulletController = obj.GetComponent<BulletController> ();
		if (bulletController) {
			health -= bulletController.GetDamage();
			if (health <= 0.0f) {
				Destroy (gameObject);
			}
		}
		Destroy (obj);
	}

	// Use this for initialization
	void Start () {
		health = 30.0f;
		animator = GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {
		animator.SetFloat("Velocity", Mathf.Abs(velocity));
	}

	public void MoveForward () {
		rigidbody2D.AddForce(Vector2.right * velocity * moveForce);
	}
}
