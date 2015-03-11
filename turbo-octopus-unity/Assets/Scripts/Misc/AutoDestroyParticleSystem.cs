using UnityEngine;
using System.Collections;

public class AutoDestroyParticleSystem : MonoBehaviour {
	void Update () {
		if (GetComponent<ParticleSystem>() && !GetComponent<ParticleSystem>().IsAlive()) {
			Destroy(gameObject);
		} else if (!GetComponent<ParticleSystem>()) {
			Debug.LogWarning("AutoDestroyParticleSystem attached to an object that has no particle system!");
		}
	}
}
