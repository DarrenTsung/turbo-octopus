using UnityEngine;
using System.Collections;

public class ParticleVelocityScript : MonoBehaviour {

	protected Vector2 velocity;

	public void SetVelocity(Vector2 velocity) {
		this.velocity = velocity;
	}

	// Update is called once per frame
	void Update () {
		ParticleSystem.Particle[] particles = new ParticleSystem.Particle[GetComponent<ParticleSystem>().particleCount];
		GetComponent<ParticleSystem>().GetParticles(particles);

		for (int i=0; i<particles.Length; i++) {
			particles[i].velocity = this.velocity;
		}
		GetComponent<ParticleSystem>().SetParticles(particles, particles.Length);
	}
}
