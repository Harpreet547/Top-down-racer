using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSFXHandler : MonoBehaviour {
    [Header("Audio Sources")]
    public AudioSource tireScreechingAudioSource;
    public AudioSource engineAudioSource;
    public AudioSource carHitAudioSource;
    public AudioSource carJumpAudioSource;
    public AudioSource carJumpLandingAudioSaurce;
    public float desiredEnginePitch = 0.5f;
    public float desiredTireScreechPitch = 0.5f;

    [Header("Sound modifier")]
    public float engineVolumeFactor = 0.05f;

    // Private
    private CarControllerMark2 carControllerMark2;

    private void Awake() {
        carControllerMark2 = GetComponentInParent<CarControllerMark2>();
    }

    private void Update() {
        UpdateEngineSFX();
        UpdateTireScreechingSFX();
    }

    void UpdateEngineSFX() {
        float velocityMagniture = carControllerMark2.GetVelocityMagnitude();

        // Increase egine volume as the car goes faster
        float desiredEngineVolume = velocityMagniture * engineVolumeFactor;

        engineAudioSource.volume = Mathf.Lerp(engineAudioSource.volume, desiredEngineVolume, Time.deltaTime * 10);

        // To add more variation to the engine sound we also change the pitch.
        desiredEnginePitch = velocityMagniture * 0.2f;
        desiredEnginePitch = Mathf.Clamp(desiredEnginePitch, 0.5f, 2f);
        engineAudioSource.pitch = Mathf.Lerp(engineAudioSource.pitch, desiredEnginePitch, Time.deltaTime * 1.5f);
    }

    void UpdateTireScreechingSFX() {
        if(carControllerMark2.IsTireScreeching(out float lateralVelocity, out bool isBraking)) {
            // If car is braking we want tire screech to be louder and also change the pitch. 
            if(isBraking) {
                tireScreechingAudioSource.volume = Mathf.Lerp(tireScreechingAudioSource.volume, 1.0f, Time.deltaTime * 10);
                desiredTireScreechPitch = Mathf.Lerp(desiredTireScreechPitch, 0.5f, Time.deltaTime * 10);
            } else {
                // If we are not braking we still want to play screech sound if the player is drifting.
                tireScreechingAudioSource.volume = Mathf.Abs(lateralVelocity) * engineVolumeFactor;
                desiredTireScreechPitch = Mathf.Abs(lateralVelocity) * 0.1f;
            }
            // Added by my self
            tireScreechingAudioSource.pitch = Mathf.Lerp(tireScreechingAudioSource.pitch, desiredTireScreechPitch, Time.deltaTime * 1.5f);
        }
        // Fade out the tire screech SFX if we are not screeching
        else tireScreechingAudioSource.volume = Mathf.Lerp(tireScreechingAudioSource.volume, 0, Time.deltaTime * 10);
    }

    public void PlayJumpSFX() {
        carJumpAudioSource.Play();
    }

    public void PlayLandingSfx() {
        carJumpLandingAudioSaurce.Play();
    }
    private void OnCollisionEnter2D(Collision2D other) {
        float relativeVelocity = other.relativeVelocity.magnitude;
        float volume = relativeVelocity * 0.1f;

        carHitAudioSource.pitch = Random.Range(0.95f, 1.05f);
        carHitAudioSource.volume = volume;

        if(!carHitAudioSource.isPlaying) carHitAudioSource.Play();
    }
}
