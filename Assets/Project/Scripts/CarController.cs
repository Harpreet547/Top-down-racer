using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour {
    Rigidbody2D rb;

    public float accelerationPower = 5f;
    public float steeringPower = 5f;

    float steeringAmount, speed, direction;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate() {
        steeringAmount = - Input.GetAxis("Horizontal");
        speed = Input.GetAxis("Vertical") * accelerationPower;
        direction = Mathf.Sign(Vector2.Dot(rb.velocity, rb.GetRelativeVector(Vector2.up)));

        rb.rotation += steeringAmount * steeringPower * rb.velocity.magnitude * direction;
        rb.AddRelativeForce(Vector2.up * speed);
        rb.AddRelativeForce((steeringAmount / 2) * rb.velocity.magnitude * (-Vector2.right));
    }
}
