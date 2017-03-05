using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpyBallAnchor : MonoBehaviour {

	public enum Hand {
        none,
        left,
        right,
    }

    [SerializeField]
    private Hand _hand;
    public Hand hand {
        get {
            return _hand;
        }
    }

    public Vector3 position {
        get {
            return transform.position;
        }
    }

}
