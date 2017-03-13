using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JBirdEngine.AngleVector;

public class RobotPatrol : MonoBehaviour {

    public enum MovementMode {
        patroling,
        chasing,
        searching,
    }

    [SerializeField]
    private MovementMode movementMode;
    [SerializeField]
    private bool _movementEnabled = true;
    public bool movementEnabled {
        get {
            return _movementEnabled;
        }
    }
    [SerializeField]
    private bool _visionEnabled = true;
    public bool visionEnabled {
        get {
            return _visionEnabled;
        }
    }

    [SerializeField]
    private Vector3 _eyeOffset;
    public Vector3 eyeOffset {
        get {
            return _eyeOffset;
        }
    }
    [SerializeField]
    private AngleVector3 _searchSpread;
    public AngleVector3 searchSpread {
        get {
            return _searchSpread;
        }
    }

    public List<Vector3> route;

    int _routeIndex = 0;
    int routeIndex {
        get {
            return _routeIndex;
        }
        set {
            if (route.Count == 0) {
                _routeIndex = 0;
            }
            else {
                _routeIndex = value % route.Count;
            }
        }
    }

    [SerializeField]
    private Transform _targetTransform;
    public Transform targetTransform {
        get {
            return _targetTransform;
        }
    }

    [SerializeField]
    private float moveSpeed = 1f;
    [SerializeField]
    private float turnSpeed = 100f;

    float timeStep {
        get {
            return Time.fixedDeltaTime;
        }
    }

    Vector3 nextNode {
        get {
            if (route.Count > 0) {
                return route[routeIndex];
            }
            else {
                return transform.position;
            }
        }
    }

    void FixedUpdate () {
        if (_movementEnabled) {
            if (movementMode == MovementMode.patroling) {
                FollowRoute();
            }
        }
    }

    void FollowRoute () {
        // Snap to node if close enough
        if (Vector3.Distance(transform.position, nextNode) < moveSpeed * timeStep) {
            transform.position = nextNode;
            ++routeIndex;
            return;
        }
        // Turn to face next node
        if (Vector3.Angle(transform.forward, nextNode - transform.position) >= turnSpeed * timeStep) {
            transform.forward = Quaternion.AngleAxis(turnSpeed * Mathf.Sign(Vector3.Dot(Vector3.up, Vector3.Cross(transform.forward, nextNode - transform.position))) * timeStep, Vector3.up) * transform.forward;
        }
        // Move towards next node
        else {
            transform.forward = (nextNode - transform.position).normalized;
            transform.position += transform.forward * moveSpeed * timeStep;
        }
    }

    public void TurnOff () {
        _movementEnabled = false;
        _visionEnabled = false;
    }

}
