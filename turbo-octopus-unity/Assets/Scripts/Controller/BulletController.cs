using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BulletController : MonoBehaviour {

	protected const float DESPAWN_DISTANCE = 200.0f;
	protected const float BULLET_DRAW_DISTANCE_MIN = 1.7f;
	protected const float BULLET_DRAW_DISTANCE_MAX = 2.3f;

	Camera mainCamera;

	//make sure we aren't in this layer 
	LayerMask layerMask;

	protected Vector2 previousPosition;
	protected Vector2 lastCollision;

	protected LineRenderer lineRenderer;

	protected float initialSpeedMin = 70.0f;
	protected float initialSpeedMax = 90.0f;

	protected float damage = 1.0f;
	protected int ricochetsLeft = 0;

	protected float myDrawDistance;

	protected List<Vector2> collisionPoints;

	protected bool isTracer;

	public float GetDamage() {
		return damage;
	}

	public void SetDirection(Vector2 direction) {
		GetComponent<Rigidbody2D>().velocity = Random.Range(initialSpeedMin, initialSpeedMax) * direction.normalized;

		previousPosition = GetComponent<Rigidbody2D>().position;
	}

	public void SetTracerBullet(bool tracerBullet) {
		isTracer = tracerBullet;
		if (!isTracer) {
			lineRenderer.enabled = false;
		}
	}

	void Awake() { 
		lineRenderer = GetComponent<LineRenderer> ();

		mainCamera = Camera.main;
		myDrawDistance = Random.Range(BULLET_DRAW_DISTANCE_MIN, BULLET_DRAW_DISTANCE_MAX);

		collisionPoints = new List<Vector2> ();
		collisionPoints.Insert(0, GetComponent<Rigidbody2D>().position);
	}

	void Update() {
		CalculateNewPosition();
		DestroyIfOutOfRange();
		if (isTracer) {
			DrawBulletTrail();
		}
	}

	void DestroyIfOutOfRange() {
		Vector2 cameraPosition2D = new Vector2(mainCamera.transform.position.x,
		                                       mainCamera.transform.position.y);

		Vector2 distanceToCamera = cameraPosition2D - GetComponent<Rigidbody2D>().position;
		if (distanceToCamera.magnitude >= DESPAWN_DISTANCE) {
			Destroy(gameObject);
		}
	}

	void CalculateNewPosition() {
		Vector2 movementThisStep = GetComponent<Rigidbody2D>().position - previousPosition;

		//check for obstructions in the path that we've traveled in the last timestep 
		RaycastHit2D hitInfo = Physics2D.Raycast(previousPosition, movementThisStep.normalized, movementThisStep.magnitude);
		if (hitInfo.collider != null && hitInfo.fraction != 0.0f) {
			GameObject obj = GameUtils.GetTopLevelObject(hitInfo.transform.gameObject);
			Rigidbody2D objRigidbody = obj.GetComponent<Rigidbody2D>();
			if (objRigidbody) {
				Vector2 force = GetComponent<Rigidbody2D>().velocity * GetComponent<Rigidbody2D>().mass;
				objRigidbody.AddForce(force);
			}

			EnemyController behaviorController = obj.GetComponent<EnemyController> ();
			if (behaviorController) {
				behaviorController.OnHit(gameObject);
			}
				
			if (ricochetsLeft > 0) {
				ricochetsLeft--;

				// lineRendering
				Vector2 bulletVel = GetComponent<Rigidbody2D>().velocity;
				Vector2 normal = hitInfo.normal.normalized;

				collisionPoints.Insert(0, hitInfo.point);

				Vector2 reflectedVelocity = bulletVel - (2.0f * Vector2.Dot (bulletVel, normal) * normal);
				GetComponent<Rigidbody2D>().velocity = reflectedVelocity;

				float reflectedDistance = (1.0f - hitInfo.fraction) * movementThisStep.magnitude;

				previousPosition = hitInfo.point;
				GetComponent<Rigidbody2D>().position = hitInfo.point + (reflectedDistance * reflectedVelocity.normalized);
				CalculateNewPosition();
			} else {
				Instantiate(PrefabManager.Instance.bulletExplosion, hitInfo.point, Quaternion.identity);
				GameObject bulletPuff = Instantiate(PrefabManager.Instance.bulletPuff, hitInfo.point, Quaternion.identity) as GameObject;
				bulletPuff.GetComponent<ParticleVelocityScript> ().SetVelocity(-GetComponent<Rigidbody2D>().velocity * 0.015f);
				Destroy(gameObject);
			}
		} 

		previousPosition = GetComponent<Rigidbody2D>().position; 
	}


	void DrawBulletTrail() {
		List<Vector3> pointsToDraw = CalculateDrawPoints();
		DrawTrailWithPoints(pointsToDraw);
	}

	List<Vector3> CalculateDrawPoints() {
		List<Vector3> pointsToDraw = new List<Vector3> ();

		pointsToDraw.Add (new Vector3(GetComponent<Rigidbody2D>().position.x, GetComponent<Rigidbody2D>().position.y, -1.0f));

		float distanceLeftToDraw = myDrawDistance;
		Vector2 previousPosition = GetComponent<Rigidbody2D>().position;
		for (int i=0; i<collisionPoints.Count; i++) {
			Vector2 currentPosition = collisionPoints[i];
			float distanceToDraw = (currentPosition - previousPosition).magnitude;
			if (distanceToDraw > distanceLeftToDraw) {
				Vector2 drawPoint = previousPosition + Vector2.ClampMagnitude(currentPosition - previousPosition, distanceLeftToDraw);
				pointsToDraw.Add (new Vector3 (drawPoint.x, drawPoint.y, -1.0f));
				return pointsToDraw;
			} else {
				distanceLeftToDraw -= distanceToDraw;
				pointsToDraw.Add (new Vector3 (currentPosition.x, currentPosition.y, -1.0f));
			}
			previousPosition = currentPosition;
		}
		return pointsToDraw;
	}

	void DrawTrailWithPoints(List<Vector3> pointsToDraw) {
		lineRenderer.SetVertexCount(pointsToDraw.Count);
		for (int i=0; i<pointsToDraw.Count; i++) {
			lineRenderer.SetPosition(i, pointsToDraw[i]);
		}
	}
}
