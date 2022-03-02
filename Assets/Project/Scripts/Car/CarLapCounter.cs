using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class CarLapCounter : MonoBehaviour {
    public Text carPositionText;
    int passedCheckpointNumber = 0;
    float timeAtLastPassedCheckpoint = 0;
    int numberOfPassedCheckpoints = 0;
    int lapsCompleted = 0;
    bool isRaceFinished = false;
    int carPosition = 0;
    public int lapsToComplete = 2;
    bool isHideRoutineRunning = false;
    float hideUIDelayTime;

    // C# events.
    public event Action<CarLapCounter> OnPassCheckpoint;

    private void OnTriggerEnter2D(Collider2D other) {
        if(isRaceFinished) return;
        if(other.CompareTag("Checkpoint")) {
            Checkpoint checkpoint = other.GetComponent<Checkpoint>();
            // Make sure no checkpoint is missed
            if(checkpoint.checkpointNumber == passedCheckpointNumber + 1) {
                passedCheckpointNumber = checkpoint.checkpointNumber;
                timeAtLastPassedCheckpoint = Time.time;
                numberOfPassedCheckpoints++;

                if(checkpoint.isFinishLine) {
                    passedCheckpointNumber = 0;
                    lapsToComplete++;
                    if(lapsCompleted >= lapsToComplete) isRaceFinished = true;
                }
                // Invoke the passed checkpoint event.
                OnPassCheckpoint?.Invoke(this);

                // Now show the car position as it has been calculated.
                if(isRaceFinished) StartCoroutine(ShowPositionCO(100));
                else StartCoroutine(ShowPositionCO(1.5f));
            }
        }
    }

    IEnumerator ShowPositionCO(float delayUntilHidePosition) {
        hideUIDelayTime += delayUntilHidePosition;

        carPositionText.text = carPosition.ToString();
        carPositionText.gameObject.SetActive(true);

        if(!isHideRoutineRunning) {
            isHideRoutineRunning = true;

            yield return new WaitForSeconds(hideUIDelayTime);
            carPositionText.gameObject.SetActive(false);

            isHideRoutineRunning = false;
        }

    }

    public void SetCarPosition(int position) {
        carPosition = position;
    }

    public int GetNumberOfPassedCheckpoints() {
        return numberOfPassedCheckpoints;
    }

    public float GetTimeAtLastCheckpoint() {
        return timeAtLastPassedCheckpoint;
    }
}
