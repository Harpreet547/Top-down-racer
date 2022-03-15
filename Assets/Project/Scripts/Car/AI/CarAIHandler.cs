using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CarAIHandler : MonoBehaviour {

    public enum AIMode { followPlayer, followWaypoints };

    [Header("AI settings")]
    public AIMode aiMode;
    public float avoidanceRadius =  1.2f;
    public float avoidanceCircleOffsetToFront = 0.5f;
    public bool isAvoidingCars = true;

    [Range(0.0f, 1.0f)]
    public float skillLevel = 0.3f;

    // Local Vars
    Vector3 targetPosition = Vector3.zero;
    Transform targetTransform = null;
    Collider2D carCollider;
    float originalMaxSpeed = 16f;
    // Keeping max speed private so that we can get this from car controller script based on car setting. So, all car models will have same top speed for player and AI.
    float maxSpeed = 10;

    //Avoidance
    Vector2 avoidanceVectorLerped = Vector3.zero;

    // Waypoints
    WaypointNode currentWaypoint = null;
    WaypointNode previousWaypoint = null;
    WaypointNode[] allWaypoints;
    CarControllerMark2 carControllerMark2;

    // Stuck handling
    bool isRunningStuckCheck = false;
    bool isFirstTemporaryWaypoint = false;
    int stuckCheckCounter = 0;
    List<Vector2> temporaryWaypoints = new List<Vector2>();

    private void Awake() {
        carControllerMark2 = GetComponent<CarControllerMark2>();
        allWaypoints = FindObjectsOfType<WaypointNode>();
        carCollider = GetComponent<Collider2D>();
        originalMaxSpeed = carControllerMark2.maxSpeed;;
        maxSpeed = carControllerMark2.maxSpeed;;
    }

    private void Start() {
        SetMaxSpeedBasedOnSkillLevel(maxSpeed);
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
        if(currentWaypoint == null) {
            currentWaypoint = FindClosestWaypoint();
            previousWaypoint = currentWaypoint;
        }
        if(currentWaypoint == null) return;

        targetPosition = currentWaypoint.transform.position;

        // store how close we are to the waypoint
        float distanceToWayPoint = (targetPosition - transform.position).magnitude;

        if(distanceToWayPoint > 20) {
            Vector3 nearestPointOnTheWayPoint = FindNearestPointOnLine(previousWaypoint.transform.position, currentWaypoint.transform.position, transform.position);
            float segments = distanceToWayPoint / 20.0f;
            targetPosition = (targetPosition + nearestPointOnTheWayPoint * segments) / (segments + 1);
            Debug.DrawLine(transform.position, targetPosition, Color.cyan);
        }
        // Check if we are close enough to consider that we have reached the waypoint.
        if(distanceToWayPoint <= currentWaypoint.minDistanceToReactWaypoint) {
            if(currentWaypoint.maxSpeed > 0) SetMaxSpeedBasedOnSkillLevel(currentWaypoint.maxSpeed);
            else SetMaxSpeedBasedOnSkillLevel(1000); // don't slow car down if waypoint does not have max speed.

            // Store the current waypoint as previous before we assign a new current one.
            previousWaypoint = currentWaypoint;

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

        // Apply avoidance to steering
        if(isAvoidingCars) AvoidAICars(vectorToTarget, out vectorToTarget);

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

        // Apply throttle forward based on how much the car wants to turn. If it's a sharp turn this will cause the car to apply less speed forward.
        float reduceSpeedDueToCornering = Mathf.Abs(inputX) / 1.0f;

        // Apply throttle forward based on how much the car wants to turn. If it's a sharp turn this will cause the car to aaply less speed forward.
        return 1.0f - reduceSpeedDueToCornering * skillLevel;
    }

    void SetMaxSpeedBasedOnSkillLevel(float newSpeed) {
        maxSpeed = Mathf.Clamp(newSpeed, 0, originalMaxSpeed);
        float skillbasedMaximumSpeed = Mathf.Clamp(skillLevel, 0.3f, 1.0f);
        maxSpeed *= skillbasedMaximumSpeed;
    }

    // Finds the nearest point on a line.
    Vector2 FindNearestPointOnLine(Vector2 lineStartPoistion, Vector2 lineEndPosition, Vector2 point) {
        // Get deading as a vector
        Vector2 lineHeadingVector = (lineEndPosition - lineStartPoistion);

        // Store the max distance
        float maxDistance = lineHeadingVector.magnitude;
        lineHeadingVector.Normalize();

        // Do projection from the start position to the point
        Vector2 lineVectorStartToPoint = point - lineStartPoistion;
        float dotProduct = Vector2.Dot(lineVectorStartToPoint, lineHeadingVector);

        // Clamp the dot product to maxDistance.
        dotProduct = Mathf.Clamp(dotProduct, 0f, maxDistance);
        return lineStartPoistion + lineHeadingVector * dotProduct;
    }

    // Check for cars ahead of the car.
    bool IsCarInFrontOfAICar(out Vector3 position, out Vector3 otherCarRightVector) {
        // Disable the car's own collider to avoid having the AI car detect itself.
        carCollider.enabled = false;
        // Perform the circle case in front of the car with slight offset forward and only in the car layer.
        // Create util function for checking layer in mask line done in other project.
        RaycastHit2D raycastHit2d = Physics2D.CircleCast(transform.position + transform.up * avoidanceCircleOffsetToFront, avoidanceRadius, transform.up, 12, 1 << gameObject.layer);
        // Enable the colliders again so the car can collide and other cars can detect it.
        carCollider.enabled = true;

        if(raycastHit2d.collider != null) {
            Debug.Log("Ray hit");
            // Draw a red line showing how long the detection is, make it red since we have detected another car.
            Debug.DrawRay(transform.position, transform.up * 12, Color.red);
            position = raycastHit2d.collider.transform.position;
            otherCarRightVector = raycastHit2d.collider.transform.right;
            return true;
        } else {
            // We didn't detect any other car so draw black line with the distance that we use to check for other cars.
            Debug.DrawRay(transform.position, transform.up * 12, Color.black);
        }
        position = Vector3.zero;
        otherCarRightVector = Vector3.zero;
        return false;
    }

    void AvoidAICars(Vector2 vectorToTarget, out Vector2 newVectorToTarget) {
        if(IsCarInFrontOfAICar(out Vector3 otherCarPosition, out Vector3 otherCarRightVector)) {
            Vector2 avoidanceVector = Vector2.zero;

            // Calculate the reflecting vector if we would hit the other car.
            avoidanceVector = Vector2.Reflect((otherCarPosition - transform.position).normalized, otherCarRightVector);

            float distanceToTarget = (targetPosition - transform.position).magnitude;

            // We want to be able to control how much desire the AI has to drive towards the waypoint vs avoiding the other cars.
            // As we get closer to the waypoint the desire to reach the waypoint increases.
            float driveToTargetInfluence = 6.0f / distanceToTarget;

            // Ensure that we limit the value to between 30% and 100% as we always want AI to desire to react the waypoint.
            driveToTargetInfluence = Mathf.Clamp(driveToTargetInfluence, 0.30f, 1.0f);

            // The desire to avoid the car is simply the inverse to reach the waypoint.
            float avoidanceInfluence = 1.0f - driveToTargetInfluence;

            avoidanceVectorLerped = Vector2.Lerp(avoidanceVectorLerped, avoidanceVector, Time.fixedDeltaTime * 4); // 4 is a constant, smaller value will be smoother but less responsive and vice versa.

            newVectorToTarget = vectorToTarget * driveToTargetInfluence + avoidanceVectorLerped * avoidanceInfluence;
            newVectorToTarget.Normalize();

            // Draw the vector which indicates the avoidance vector in green
            Debug.DrawRay(transform.position, avoidanceVector * 10, Color.green);

            // Draw the vector car will actually take in yellow
            Debug.DrawRay(transform.position, newVectorToTarget * 10, Color.yellow);
            return;
        }
        // We need assign a default value if we didn't hit any cars before we exit the function.
        newVectorToTarget = vectorToTarget;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.cyan;
        // Gizmo not considering distance of 12 in Circle cast.
        Gizmos.DrawWireSphere(transform.position + transform.up * avoidanceCircleOffsetToFront, avoidanceRadius);
    }
}
