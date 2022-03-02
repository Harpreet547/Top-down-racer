using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelsControl : MonoBehaviour {
    Vector3 localAngle;
    float steeringAngle, maxSteeringAngle = 30f;

    private void Update() {
        steeringAngle = -Input.GetAxis("Horizontal") * maxSteeringAngle;
    }

    private void LateUpdate() {
        localAngle = transform.localEulerAngles;
        localAngle.z = steeringAngle;
        transform.localEulerAngles = localAngle;
    }
}
