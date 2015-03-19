﻿using UnityEngine;
using System.Collections;

public class DamageTextController : MonoBehaviour {

	protected float lifespan, baseLifespan;
	protected tk2dTextMesh textMesh;

	void Awake () {
		lifespan = 1.3f;
		baseLifespan = lifespan;
		GetComponent<Rigidbody2D> ().velocity = Vector2.up * 2.0f;
		textMesh = GetComponent<tk2dTextMesh> ();
	}

	public void SetDamage(int damage) {
		string damageText = damage.ToString();
		textMesh.text = damageText;
	}

	public void SetScale(float scale) {
		textMesh.scale = new Vector3(scale, scale, 1.0f);
	}
	
	void Update () {
		lifespan -= Time.deltaTime;
		Color newColor = textMesh.color;
		newColor.a = lifespan / baseLifespan;
		textMesh.color = newColor;
		if (lifespan <= 0.0f) {
			Destroy(gameObject);
		}
	}
}
