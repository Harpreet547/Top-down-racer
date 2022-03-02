using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarControllerMark2 : MonoBehaviour {
    [Header("Car settings")]
    public float accelerationFactor = 30.0f;
    public float maxSpeed = 20f;
    public float turnFactor = 3.5f;
    public float driftFactor = 0.95f;
    public float minSpeedTurningConstant = 8;
    public float dragFactor = 3.0f;
    public float reverseGearSpeedFactor = 0.3f;
    public float driftSpeedForSkidmarks = 4.0f;

    [Header("Sprites")]
    public SpriteRenderer carSpriteRenderer;
    public SpriteRenderer carShadowRenderer;

    [Header("Jumping")]
    public AnimationCurve jumpCurve;
    public float landingCheckConstant = 1.5f;
    public ParticleSystem landingParticleSystem;
    
    // Local variables
    float accelerationInput = 0;
    float steeringInput = 0;
    float rotationAngle = 0;
    float velocityVSUp = 0;
    bool isJumping = false;
    Rigidbody2D rb;
    Collider2D carCollider;
    CarSFXHandler carSFXHandler;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        carCollider = GetComponent<Collider2D>();
        carSFXHandler = GetComponent<CarSFXHandler>();
    }

    private void FixedUpdate() {
        ApplyEngineForce();
        KillOrthogonalVelocity();
        ApplySteering();
    }

    void ApplyEngineForce() {
        // Don't let the player brake while in air, but we still allow some drag so it can be slowed slightly.
        if(isJumping && accelerationInput < 0) accelerationInput = 0;

        // Calculate how much forwad we are going in term of the direction of our velocity.
        velocityVSUp = Vector2.Dot(transform.up, rb.velocity);

        // Limit forward velocity to maxSpeed.
        if(velocityVSUp > maxSpeed && accelerationInput > 0) return;
        // Limit reverse velocity to maxSpeed / reverseGearSpeedFactor.
        if(velocityVSUp < (-maxSpeed * reverseGearSpeedFactor) && accelerationInput > 0) return;
        // Limit so that we cannot go faster in any direction while accelerating.
        if(rb.velocity.sqrMagnitude > maxSpeed * maxSpeed && accelerationInput > 0 && !isJumping) return;

        // Apply drag  if there is no accelerationInput so that the car stops when player lets go of the acceleration.
        // Time.fixedDeltaTime * dragFactor, here drag factor default value was 3 in tutorial. This could be different from drag factor. Depends upon future testing.
        if(accelerationInput == 0)
            rb.drag = Mathf.Lerp(rb.drag, dragFactor, Time.fixedDeltaTime * dragFactor);
        else
            rb.drag = 0;

        Vector2 engineForceVector = accelerationInput * accelerationFactor * transform.up;
        rb.AddForce(engineForceVector, ForceMode2D.Force);
    }

    void ApplySteering() {
        // Limit car's ability to turn while moving slowly.
        float minSpeedBeforeAllowTurningFactor = (rb.velocity.magnitude / minSpeedTurningConstant);
        minSpeedBeforeAllowTurningFactor = Mathf.Clamp01(minSpeedBeforeAllowTurningFactor);


        rotationAngle -= steeringInput * turnFactor * minSpeedBeforeAllowTurningFactor * (accelerationInput >= 0 ? 1 : -1);
        rb.MoveRotation(rotationAngle);
    }

    void KillOrthogonalVelocity() {
        Vector2 forwardVelocity = transform.up * Vector2.Dot(rb.velocity, transform.up);
        Vector2 rightVelocity = transform.right * Vector2.Dot(rb.velocity, transform.right);

        rb.velocity = forwardVelocity + rightVelocity * driftFactor;
    }

    public void SetInputVector(Vector2 inputVector) {
        steeringInput = inputVector.x;
        accelerationInput = inputVector.y;
    }

    float GetLateralVelocity() {
        // Return how fast car is moving sideways.
        return Vector2.Dot(transform.right, rb.velocity);
    }

    public bool IsTireScreeching(out float lateralVelocity, out bool isBraking) {
        lateralVelocity = GetLateralVelocity();
        isBraking = false;

        if(isJumping) return false;

        if(accelerationInput < 0 && velocityVSUp > 0) {
            isBraking = true;
            return true;
        }

        // if(accelerationInput > 0 && velocityVSUp < 0) {
        //     isBraking = true;
        //     return true;
        // }

        // If we have lot of sideways movement then tires should be screeching
        if(Mathf.Abs(GetLateralVelocity()) > driftSpeedForSkidmarks) return true;

        return false;
    }

    public float GetVelocityMagnitude() {
      return rb.velocity.magnitude;
    }

    public void Jump(float jumpHeightScale, float jumpPushScale) {
        if(!isJumping) {
            StartCoroutine(JumpCo(jumpHeightScale, jumpPushScale));
        }
    }

    private IEnumerator JumpCo(float jumpHeightScale, float jumpPushScale) {
        isJumping = true;

        float jumpStartTime = Time.time;
        float jumpDuration = GetVelocityMagnitude() * 0.05f;

        jumpHeightScale = jumpHeightScale * GetVelocityMagnitude() * 0.05f;
        jumpHeightScale = Mathf.Clamp(jumpHeightScale, 0.0f, 1.0f);

        carCollider.enabled = false;

        carSFXHandler.PlayJumpSFX();

        rb.AddForce(rb.velocity.normalized * 10 * jumpPushScale, ForceMode2D.Impulse);

        // Change sorting layer to avoid going under objects while jumping.
        carSpriteRenderer.sortingLayerName = "Flying";
        carShadowRenderer.sortingLayerName = "Flying";

        while(isJumping) {
            // Percent 0 - 1.0 of where we are in the jumping process
            float jumpCompletedPercentage = (Time.time - jumpStartTime) / jumpDuration;
            jumpCompletedPercentage = Mathf.Clamp01(jumpCompletedPercentage);

            // Take the base scale of 1 and add much we should increase the scale.
            carSpriteRenderer.transform.localScale = Vector3.one + (jumpHeightScale* jumpCurve.Evaluate(jumpCompletedPercentage) * Vector3.one);
            // Change the shadow scale also but make it a bit smaller. In real world this should be the opposite, the higher the object get the larger its shadow gets.
            carShadowRenderer.transform.localScale = carSpriteRenderer.transform.localScale * 0.75f;

            // Offset the shadow a bit. This is not 100% correct either but works good enough.
            carShadowRenderer.transform.localPosition = (new Vector3(1, -1, 0.0f)) * 3 * jumpHeightScale * jumpCurve.Evaluate(jumpCompletedPercentage);

            // When we reach 100% we are breaking the loop
            if(jumpCompletedPercentage == 1.0f) break;
            yield return null;
        }

        // Check if landing is ok or not
        if(Physics2D.OverlapCircle(transform.position, landingCheckConstant)) {
            // Something is below the car and we need to jump again
            isJumping = false;
            // add a small jump and push the car forward a bit.
            Jump(0.2f, 0.6f);
        } else {
            // Handle landing, scale back the object.
            carSpriteRenderer.transform.localScale = Vector3.one;

            // reset the shadows position and scale
            carShadowRenderer.transform.localPosition = Vector3.zero;
            carShadowRenderer.transform.localScale = carSpriteRenderer.transform.localScale;

            carSpriteRenderer.sortingLayerName = "Cars";
            carShadowRenderer.sortingLayerName = "Cars";

            // Play landing particle system if it is a bigger jump.
            if(jumpHeightScale > 0.2f) {
                landingParticleSystem.Play();
                carSFXHandler.PlayLandingSfx();
            }

            carCollider.enabled = true;

            isJumping = false;
        }

    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Jump")) {
            JumpData jumpData = other.GetComponent<JumpData>();
            Jump(jumpData.jumpHeightScale, jumpData.jumpPushScale);
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(transform.position, landingCheckConstant);
    }
}
