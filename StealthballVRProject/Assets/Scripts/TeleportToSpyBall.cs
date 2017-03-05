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

    public void Teleport (Transform ballAnchor) {
        if (spyBall == null || head == null) {
            Debug.LogError("TeleportToSpyBall: References not set!");
            return;
        }
        if (spyBall.isHeld || !spyBall.isStopped) {
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

}
