using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelTrailRendererHandler : MonoBehaviour {

    public bool isOverpassEmitter = false;
    CarControllerMark2 carControllerMark2;
    TrailRenderer trailRenderer;
    CarLayerHandler carLayerHandler;
    
    private void Awake() {
        carControllerMark2 = GetComponentInParent<CarControllerMark2>();
        carLayerHandler = GetComponentInParent<CarLayerHandler>();

        trailRenderer = GetComponent<TrailRenderer>();
        trailRenderer.emitting = false;
    }

    private void Update() {
        trailRenderer.emitting = false;

        if(carControllerMark2.IsTireScreeching(out float lateralVelocity, out bool isBraking)) {
            if(carLayerHandler.GetIsDrivingOverpass() && isOverpassEmitter)
                trailRenderer.emitting = true;
            if(!carLayerHandler.GetIsDrivingOverpass() && !isOverpassEmitter)
                trailRenderer.emitting = true;
        }
    }
}
