using UnityEngine;
using System.Collections;

public class AutoDestroyParticleSystem : MonoBehaviour {
	void Update () {
		if (particleSystem && !particleSystem.IsAlive()) {
			Destroy(gameObject);
		} else if (!particleSystem) {
			Debug.LogWarning("AutoDestroyParticleSystem attached to an object that has no particle system!");
		}
	}
}
