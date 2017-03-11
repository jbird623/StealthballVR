using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotPatrol : MonoBehaviour {

    public enum MovementMode {
        patroling,
        chasing,
        searching,
    }

    [SerializeField]
    private MovementMode movementMode;
    [SerializeField]
    private bool movementEnabled = true;
    [SerializeField]
    private bool visionEnabled = true;

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
        if (movementEnabled) {
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
        movementEnabled = false;
        visionEnabled = false;
    }

}
