using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpyBall : MonoBehaviour {

    private Collider ballCollider;
    private Rigidbody ballRigidBody;
    private Renderer ballRenderer;

    [SerializeField]
    private Light indicatorLight;
    [SerializeField]
    private Color movingColor;
    [SerializeField]
    private Color stoppedColor;
    [SerializeField]
    private Color heldColor;
    [SerializeField]
    private Color invalidColor;

    [SerializeField]
    private float _ballRadius = 0.1f;
    public float ballRadius {
        get {
            return _ballRadius;
        }
    }

    [SerializeField]
    private float clearanceCapsuleRadius = 0.4f;
    [SerializeField]
    private float clearanceCapsuleHeight = 1.8f;

    public bool isHeld { get; private set; }
    public bool isStopped { get; private set; }
    public bool validPosition { get; private set; }

    private Vector3 lastPosition;
    [SerializeField]
    private float throwVelocityMultiplier = 5f;

    void Awake () {
        ballCollider = GetComponent<Collider>();
        ballRigidBody = GetComponent<Rigidbody>();
        ballRenderer = GetComponentInChildren<Renderer>();
        if (indicatorLight == null) {
            Debug.LogError("SpyBall: Indicator light reference not set!");
        }
        ChangeColor(movingColor);
    }

    public void PickUp (Transform holdAnchor) {
        ballRigidBody.isKinematic = true;
        transform.SetParent(holdAnchor);
        transform.localPosition = Vector3.zero;
        ballRigidBody.velocity = Vector3.zero;
        isHeld = true;
        ChangeColor(heldColor);
    }

    public void Drop () {
        ballRigidBody.isKinematic = false;
        transform.SetParent(null);
        isHeld = false;
        ballRigidBody.velocity = GetThrowVelocity();
        isStopped = false;
        ChangeColor(movingColor);
    }

    void LateUpdate () {
        if (lastPosition == transform.position && ballRigidBody.velocity == Vector3.zero) {
            isStopped = true;
            if (CheckClearance()) {
                ChangeColor(stoppedColor);
                validPosition = true;
            }
            else {
                ChangeColor(invalidColor);
                validPosition = false;
            }
        }
        lastPosition = transform.position;
    }

    Vector3 GetThrowVelocity () {
        return (transform.position - lastPosition) * throwVelocityMultiplier;
    }

    bool CheckClearance () {
        RaycastHit hit;
        Physics.Raycast(transform.position, Vector3.up, out hit, clearanceCapsuleHeight);
        if (hit.collider != null) {
            return false;
        }
        Physics.SphereCast(transform.position + Vector3.up * clearanceCapsuleRadius, clearanceCapsuleRadius, Vector3.up, out hit, clearanceCapsuleHeight - (2 * clearanceCapsuleRadius));
        if (hit.collider != null) {
            return false;
        }
        return true;
    }

    void ChangeColor (Color c) {
        indicatorLight.color = c;
        ballRenderer.material.SetColor("_OutlineColor", c);
    }

}
