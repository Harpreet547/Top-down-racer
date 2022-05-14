using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TransmissionType {
    fourSspeed = 4, fiveSpeed = 5, sixSpeed = 6, sevenSpeed = 7,
}

public class Engine : MonoBehaviour {

    // Public
    public TransmissionType transmissionType;
    [Header("Tire diameter in inches")]
    public int tireDiameter = 16;

    [Range(0, 1)]
    public float firstGearRatio;
    [Range(0, 1)]
    public float secondGearRatio;
    [Range(0, 1)]
    public float thirdGearRatio;
    [Range(0, 1)]
    public float fourthGearRatio;
    [Range(0, 1)]
    public float fifthGearRatio;
    [Range(0, 1)]
    public float sixthGearRatio;
    [Range(0, 1)]
    public float seventhGearRatio;

    // Private
    int currentGear = 1;
    float currentGearRatio;

    public float GetAcceleration(float accelerationFactor, float accelerationInput, float velocityVSUp) {

        currentGearRatio = firstGearRatio;
        GetRPM(velocityVSUp);
        return 0;
    }

    float GetRPM(float velocityVSUp) {
        // rpm = (mph * gearRation * 336) / tireDiameter
        float rpm = ((velocityVSUp * 10) * currentGearRatio * 336) / tireDiameter;
        Debug.Log(rpm);
        return 0;
    }
}
