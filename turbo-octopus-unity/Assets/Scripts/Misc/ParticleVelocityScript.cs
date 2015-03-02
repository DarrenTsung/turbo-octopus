using UnityEngine;
using System.Collections;

public class ParticleVelocityScript : MonoBehaviour {

	protected Vector2 velocity;

	public void SetVelocity(Vector2 velocity) {
		this.velocity = velocity;
	}

	// Update is called once per frame
	void Update () {
		ParticleSystem.Particle[] particles = new ParticleSystem.Particle[particleSystem.particleCount];
		particleSystem.GetParticles(particles);

		for (int i=0; i<particles.Length; i++) {
			particles[i].velocity = this.velocity;
		}
		particleSystem.SetParticles(particles, particles.Length);
	}
}
