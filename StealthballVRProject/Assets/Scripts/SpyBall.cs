using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpyBall : MonoBehaviour {

    private Collider ballCollider;
    private Rigidbody ballRigidBody;
    private Renderer ballRenderer;

    [SerializeField]
    private int throwArcSize = 3;

    [SerializeField]
    private Light indicatorLight;
    [SerializeField]
    private Color movingColor;
    [SerializeField]
    private Texture movingTexture;
    [SerializeField]
    private Color validColor;
    [SerializeField]
    private Texture stoppedTexture;
    [SerializeField]
    private Color invalidColor;
    [SerializeField]
    private Texture invalidTexture;

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
    public bool validThrow { get; private set; }

    private Vector3 lastPosition;

    private class ThrowArcStep {
        public ThrowArcStep (Vector3 pos, bool v) {
            position = pos;
            valid = v;
        }
        public Vector3 position;
        public bool valid;
    }

    private List<ThrowArcStep> throwArc;
    [SerializeField]
    private float throwVelocityMultiplier = 5f;

    void Awake () {
        validThrow = true;
        ballCollider = GetComponent<Collider>();
        ballRigidBody = GetComponent<Rigidbody>();
        ballRenderer = GetComponentInChildren<Renderer>();
        if (indicatorLight == null) {
            Debug.LogError("SpyBall: Indicator light reference not set!");
        }
        ChangeColor(movingColor);
        throwArc = new List<ThrowArcStep>();
    }

    public void PickUp (Transform holdAnchor) {
        if (isHeld) {
            return;
        }
        ballRigidBody.isKinematic = true;
        ballCollider.isTrigger = true;
        transform.SetParent(holdAnchor);
        transform.localPosition = Vector3.zero;
        ballRigidBody.velocity = Vector3.zero;
        throwArc = new List<ThrowArcStep>();
        isHeld = true;
        lastPosition = transform.position;
        ChangeColor(validColor);
    }

    public void Drop () {
        if (!CheckThrowValidity()) {
            return;
        }
        ballRigidBody.isKinematic = false;
        ballCollider.isTrigger = false;
        transform.SetParent(null);
        isHeld = false;
        ballRigidBody.velocity = GetThrowVelocity();
        throwArc = new List<ThrowArcStep>();
        isStopped = false;
        ChangeColor(movingColor);
    }

    void Update () {
        if (isHeld) {
            if (CheckThrowValidity()) {
                ChangeColor(validColor);
            }
            else {
                ChangeColor(invalidColor);
            }
        }
    }

    void LateUpdate () {
        if (lastPosition == transform.position && ballRigidBody.velocity == Vector3.zero && !isHeld) {
            isStopped = true;
            if (CheckClearance()) {
                ChangeColor(validColor);
                validPosition = true;
            }
            else {
                ChangeColor(invalidColor);
                validPosition = false;
            }
        }
        if (isHeld) {
            AddThrowArcStep();
        }
        lastPosition = transform.position;
    }

    void AddThrowArcStep () {
        if (throwArc.Count >= throwArcSize) {
            throwArc.RemoveAt(0);
        }
        throwArc.Add(new ThrowArcStep(transform.position - lastPosition, validThrow && TeleportToSpyBall.singleton.CheckPlayerPositionValidity()));
    }

    Vector3 GetThrowVelocity () {
        return ( (transform.position - lastPosition) + GetAverageOfThrowArc() ) * throwVelocityMultiplier * 0.5f;
    }

    Vector3 GetAverageOfThrowArc () {
        Vector3 combinedArc = Vector3.zero;
        foreach (ThrowArcStep step in throwArc) {
            combinedArc += step.position;
        }
        return combinedArc / throwArc.Count;
    }

    bool CheckThrowValidity () {
        foreach (ThrowArcStep step in throwArc) {
            if (!step.valid) {
                return false;
            }
        }
        if (!TeleportToSpyBall.singleton.CheckPlayerPositionValidity()) {
            return false;
        }
        if (WallHackHandler.singleton.active) {
            return false;
        }
        return validThrow;
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
        if (c == movingColor) {
            ballRenderer.material.SetTexture("_MainTex", movingTexture);
        }
        else if (c == validColor) {
            ballRenderer.material.SetTexture("_MainTex", stoppedTexture);
        }
        else if (c == invalidColor) {
            ballRenderer.material.SetTexture("_MainTex", invalidTexture);
        }
    }

    void OnTriggerEnter () {
        if (isHeld) {
            validThrow = false;
            ChangeColor(invalidColor);
        }
    }

    void OnTriggerStay () {
        if (isHeld) {
            validThrow = false;
            ChangeColor(invalidColor);
        }
    }

    void OnTriggerExit () {
        if (isHeld) {
            validThrow = true;
        }
    }

}
