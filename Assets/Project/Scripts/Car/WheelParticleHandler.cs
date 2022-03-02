using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelParticleHandler : MonoBehaviour {

    public float emissionRateReductionFactor = 5;
    float particleEmissionRate = 0;
    CarControllerMark2 carControllerMark2;
    ParticleSystem particleSystemSmoke;
    ParticleSystem.EmissionModule particleSystemEmissionModule;

    private void Awake() {
        carControllerMark2 = GetComponentInParent<CarControllerMark2>();
        particleSystemSmoke = GetComponent<ParticleSystem>();

        particleSystemEmissionModule = particleSystemSmoke.emission;
        particleSystemEmissionModule.rateOverTime = 0;
    }

    private void Update() {
        particleEmissionRate = Mathf.Lerp(particleEmissionRate, 0, Time.deltaTime * emissionRateReductionFactor);
        particleSystemEmissionModule.rateOverTime = particleEmissionRate;

        if(carControllerMark2.IsTireScreeching(out float lateralVelocity, out bool isBraking)) {
            // If car tires are screeching, emit smoke. If player is braking emit a lot of smoke.
            if(isBraking) particleEmissionRate = 30;
            else particleEmissionRate = Mathf.Abs(lateralVelocity) * 2;
        }
    }
}
