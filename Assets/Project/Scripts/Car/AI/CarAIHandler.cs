using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CarAIHandler : MonoBehaviour {

    public enum AIMode { followPlayer, followWaypoints };

    [Header("AI settings")]
    public AIMode aiMode;
    public float maxSpeed = 10;
    
    // Local Vars
    Vector3 targetPosition = Vector3.zero;
    Transform targetTransform = null;

    // Waypoints
    WaypointNode currentWaypoint = null;
    WaypointNode[] allWaypoints;
    CarControllerMark2 carControllerMark2;

    private void Awake() {
        carControllerMark2 = GetComponent<CarControllerMark2>();
        allWaypoints = FindObjectsOfType<WaypointNode>();
    }
    private void FixedUpdate() {
        Vector2 inputVector = Vector2.zero;

        switch (aiMode)
        {
            case AIMode.followPlayer:
                FollowPlayer();
                break;
            case AIMode.followWaypoints:
                FollowWaypoints();
                break;
            default:
                break;
        }
        inputVector.x = TurnTowardTarget();
        inputVector.y = ApplyThrottleOrBrake(inputVector.x);
        carControllerMark2.SetInputVector(inputVector);
    }

    void FollowPlayer() {
        if(targetTransform == null) targetTransform = GameObject.FindGameObjectWithTag("Player").transform;

        if(targetTransform != null) targetPosition = targetTransform.position;

    }

    void FollowWaypoints() {
        if(currentWaypoint == null) currentWaypoint = FindClosestWaypoint();
        if(currentWaypoint == null) return;

        targetPosition = currentWaypoint.transform.position;

        // store how close we are to the waypoint
        float distanceToWayPoint = (targetPosition - transform.position).magnitude;

        // Check if we are close enough to consider that we have reached the waypoint.
        if(distanceToWayPoint <= currentWaypoint.minDistanceToReactWaypoint) {
            if(currentWaypoint.maxSpeed > 0) maxSpeed = currentWaypoint.maxSpeed;
            else maxSpeed = 1000; // don't slow car down if waypoint does not have max speed.
            // If we are close enough then follow to the next waypoint, if there are multiple waypoints then pick one at random.
            currentWaypoint = currentWaypoint.nextWaypointNode[Random.Range(0, currentWaypoint.nextWaypointNode.Length)];
        }
    }

    WaypointNode FindClosestWaypoint() {
        return allWaypoints.OrderBy(t => Vector3.Distance(transform.position, t.transform.position)).FirstOrDefault();
    }

    float TurnTowardTarget() {
        Vector2 vectorToTarget = targetPosition - transform.position;
        vectorToTarget.Normalize();

        // Calculate an angle towards the target
        float angleToTarget = Vector2.SignedAngle(transform.up, vectorToTarget);
        angleToTarget *= -1;

        // We want the car to turn as much as possible if the angle is greater than 45 degrees and we want it to smooth out so it the angle is small we want the to make smaller corrections.
        float steerAmount = angleToTarget / 45.0f;
        steerAmount = Mathf.Clamp(steerAmount, -1.0f, 1.0f);
        return steerAmount;
    }

    float ApplyThrottleOrBrake(float inputX) {
        // if going faster then max speed, don't apply acceleration.
        if(carControllerMark2.GetVelocityMagnitude() > maxSpeed) return 0;
        // Apply throttle forward based on how much the car wants to turn. If it's a sharp turn this will cause the car to aaply less speed forward.
        return 1.0f - Mathf.Abs(inputX) / 1.0f;
    }
}
