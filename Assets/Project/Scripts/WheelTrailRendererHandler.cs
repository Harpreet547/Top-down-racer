using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelTrailRendererHandler : MonoBehaviour {

    CarControllerMark2 carControllerMark2;
    TrailRenderer trailRenderer;
    
    private void Awake() {
        carControllerMark2 = GetComponentInParent<CarControllerMark2>();

        trailRenderer = GetComponent<TrailRenderer>();
        trailRenderer.emitting = false;
    }

    private void Update() {
        if(carControllerMark2.IsTireScreeching(out float lateralVelocity, out bool isBraking))
            trailRenderer.emitting = true;
        else trailRenderer.emitting = false;
    }
}
