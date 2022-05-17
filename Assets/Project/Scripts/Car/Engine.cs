using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engine : MonoBehaviour {

    // Public
    [Header("Curve to describe how engine gains rpm")]
    public AnimationCurve torqueCurve;
    [Header("Tire diameter in inches")]
    public int tireDiameter = 16;
    public float smoothTime = 0.01f;
    public List<float> gears = new List<float>();
    public int minRPMToShiftUp = 1200;
    public int maxRPMToShiftDown = 600;

    [Header("Variables for debugging")]
    public float velocityInKPH;
    public float wheelsRPM;
    public float engineRPM = 0;
    public float totalPower;
    public int currentGear = 0;

    public float GetAcceleration(float accelerationFactor, float accelerationInput, float velocityVSUp) {

        // currentGearRatio = thirdGearRatio;
        // GetRPM(velocityVSUp);
        // CalculateRPM(accelerationInput);

        velocityInKPH = ConvertVelocityToKPH(velocityVSUp);
        CalculateEnginePower(velocityVSUp, tireDiameter);
        return totalPower;
    }

    void CalculateEnginePower(float velocity, float tireDiameter) {
        float wheelRPM = CalculateWheelRPM(velocity, tireDiameter);
        wheelsRPM = wheelRPM;

        totalPower = torqueCurve.Evaluate(engineRPM) * gears[currentGear];

        float vehicleVelocity;
        engineRPM = Mathf.SmoothDamp(engineRPM, 500 + (Mathf.Abs(wheelRPM) * gears[currentGear]), ref velocity, smoothTime);

        if(engineRPM > minRPMToShiftUp && currentGear < gears.Count - 1) {
            currentGear++;
        } else if(engineRPM < maxRPMToShiftDown && currentGear > 0) {
            currentGear--;
        }
    }

    float CalculateWheelRPM(float velocity, float tireDiameter) {
        float tireParameter = GetTireParameterInMeters(tireDiameter);
        float wheelRPM = ConvertVelocityToMeterPerMinute(velocity) / tireParameter;
        return wheelRPM;
    }

    float ConvertVelocityToMeterPerMinute(float velocity) {
        return velocity * 60;
    }

    float ConvertVelocityToKPH(float velocity) {
        // multiply the speed value by 3.6 to convert from units/sec (mostly visualized as m/s) to kph
        return velocity * 3.6f;
    }

    float ConvertInchToMeter(float tireDiameter) {
        return tireDiameter /  39.37f;
    }

    float GetTireParameterInMeters(float tireDiameter) {
        return Mathf.PI * ConvertInchToMeter(tireDiameter);
    }

}
