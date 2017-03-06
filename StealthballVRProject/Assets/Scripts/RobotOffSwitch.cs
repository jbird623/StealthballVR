using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotOffSwitch : MonoBehaviour {

    [SerializeField]
    private Quaternion onPosition;
    [SerializeField]
    private Quaternion offPosition;
    [SerializeField]
    private float lerpTime;

    private bool flipped;

    void Awake () {
        flipped = false;
    }

	void OnCollisionEnter () {
        if (!flipped) {
            TurnOff();
            flipped = true;
        }
    }

    void TurnOff () {
        StartCoroutine(LerpToOffPosition());
    }

    IEnumerator LerpToOffPosition () {
        float t = 0f;
        while (t < lerpTime) {
            transform.rotation = Quaternion.Lerp(onPosition, offPosition, t / lerpTime);
            t += Time.deltaTime;
            yield return null;
        }
        yield break;
    }

}
