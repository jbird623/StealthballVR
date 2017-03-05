using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportToSpyBall : MonoBehaviour {

    [SerializeField]
    private SpyBall spyBall;
    [SerializeField]
    private Transform head;
    [SerializeField]
    private bool backToHandOnTeleport;
    [SerializeField]
    private Transform teleportParticlePrefab;

    public static TeleportToSpyBall singleton;

    [SerializeField]
    private LayerMask ballExclusionMask;

    void Awake () {
        JBirdEngine.Singleton.ManageSingleton(this, ref singleton);
    }

    public void Teleport (SpyBallAnchor ballAnchor) {
        if (spyBall == null || head == null) {
            Debug.LogError("TeleportToSpyBall: References not set!");
            return;
        }
        if (spyBall.isHeld || !spyBall.isStopped || !spyBall.validPosition) {
            return;
        }
        Vector3 distToHead = head.position - transform.position;
        Vector3 distToPlayer = new Vector3(distToHead.x, 0f, distToHead.z);
        transform.position = (spyBall.transform.position - Vector3.down * spyBall.ballRadius) - distToPlayer;
        if (backToHandOnTeleport) {
            spyBall.PickUp(ballAnchor);
        }
        if (teleportParticlePrefab != null) {
            Instantiate(teleportParticlePrefab, transform.position + distToPlayer, Quaternion.identity);
        }
    }

    public bool CheckPlayerPositionValidity () {
        Vector3 shoulderPos = new Vector3(head.position.x, head.position.y * 6f / 7f, head.position.z);
        float maxDist = Vector3.Distance(shoulderPos, spyBall.transform.position);
        if (Physics.Raycast(shoulderPos, (spyBall.transform.position - shoulderPos).normalized, maxDist, ballExclusionMask)) {
            return false;
        }
        return true;
    }
}
