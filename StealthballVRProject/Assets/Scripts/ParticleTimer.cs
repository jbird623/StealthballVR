using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleTimer : MonoBehaviour {

    [SerializeField]
    private float duration;

	void Awake () {
        StartCoroutine(WaitForDecay());
    }

    IEnumerator WaitForDecay () {
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }

}
