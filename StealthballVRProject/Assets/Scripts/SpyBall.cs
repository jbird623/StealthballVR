using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpyBall : MonoBehaviour {

    private Collider ballCollider;
    private Rigidbody ballRigidBody;

    [SerializeField]
    private Light indicatorLight;
    [SerializeField]
    private Color movingColor;
    [SerializeField]
    private Color stoppedColor;
    [SerializeField]
    private Color heldColor;

    [SerializeField]
    private float _ballRadius = 0.1f;
    public float ballRadius {
        get {
            return _ballRadius;
        }
    }

    public bool isHeld { get; private set; }
    public bool isStopped { get; private set; }

    private Vector3 lastPosition;
    [SerializeField]
    private float throwVelocityMultiplier = 5f;

    void Awake () {
        ballCollider = GetComponent<Collider>();
        ballRigidBody = GetComponent<Rigidbody>();
        if (indicatorLight == null) {
            Debug.LogError("SpyBall: Indicator light reference not set!");
        }
        indicatorLight.color = movingColor;
    }

    public void PickUp (Transform holdAnchor) {
        ballRigidBody.isKinematic = true;
        transform.SetParent(holdAnchor);
        transform.localPosition = Vector3.zero;
        ballRigidBody.velocity = Vector3.zero;
        isHeld = true;
        indicatorLight.color = heldColor;
    }

    public void Drop () {
        ballRigidBody.isKinematic = false;
        transform.SetParent(null);
        isHeld = false;
        ballRigidBody.velocity = GetThrowVelocity();
        isStopped = false;
        indicatorLight.color = movingColor;
    }

    void LateUpdate () {
        if (lastPosition == transform.position && ballRigidBody.velocity == Vector3.zero) {
            isStopped = true;
            indicatorLight.color = stoppedColor;
        }
        lastPosition = transform.position;
    }

    Vector3 GetThrowVelocity () {
        return (transform.position - lastPosition) * throwVelocityMultiplier;
    }

}
