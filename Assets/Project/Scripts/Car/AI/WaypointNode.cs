using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointNode : MonoBehaviour {
    [Header("This is the waypoint we are going towards, not yet reached")]
    public float maxSpeed = 0;
    public float minDistanceToReactWaypoint = 2;
    public bool isFirstWaypoint = false;
    public WaypointNode[] nextWaypointNode;

    private void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(transform.position, minDistanceToReactWaypoint);
    }
}
