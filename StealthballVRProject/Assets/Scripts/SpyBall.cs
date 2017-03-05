using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpyBall : MonoBehaviour {

    private Collider ballCollider;
    private Rigidbody ballRigidBody;

    private bool isHeld;

    private Vector3 lastPosition;
    [SerializeField]
    private float throwVelocityMultiplier = 5f;

    void Awake () {
        ballCollider = GetComponent<Collider>();
        ballRigidBody = GetComponent<Rigidbody>();
    }

    public void PickUp (Transform holdAnchor) {
        ballRigidBody.isKinematic = true;
        transform.SetParent(holdAnchor);
        transform.localPosition = Vector3.zero;
        ballRigidBody.velocity = Vector3.zero;
        isHeld = true;
    }

    public void Drop () {
        ballRigidBody.isKinematic = false;
        transform.SetParent(null);
        isHeld = false;
        ballRigidBody.velocity = GetThrowVelocity();
    }

    void LateUpdate () {
        lastPosition = transform.position;
    }

    Vector3 GetThrowVelocity () {
        return (transform.position - lastPosition) * throwVelocityMultiplier;
    }

}
