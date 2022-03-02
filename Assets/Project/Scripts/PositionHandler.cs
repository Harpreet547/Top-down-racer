using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PositionHandler : MonoBehaviour {
    LeaderboardUIHandler leaderboardUIHandler;
    List<CarLapCounter> carLapCounters = new List<CarLapCounter>();
    private void Awake() {
        CarLapCounter[] carLapCountersArray = FindObjectsOfType<CarLapCounter>();
        carLapCounters = carLapCountersArray.ToList<CarLapCounter>();

        // Hook up pased checkboint event.
        foreach (var carLapCounter in carLapCounters) {
            carLapCounter.OnPassCheckpoint += OnPassCheckpoint;
        }

        leaderboardUIHandler = FindObjectOfType<LeaderboardUIHandler>();
    }
    private void Start() {
        leaderboardUIHandler.UpdateList(carLapCounters);
    }

    void OnPassCheckpoint(CarLapCounter carLapCounter) {
        // Sort the cars position first based on how many checkpoints they have passed, more is better. Then sort on time where shorter time is better.
        carLapCounters = carLapCounters.OrderByDescending(s => s.GetNumberOfPassedCheckpoints()).ThenBy(s => s.GetTimeAtLastCheckpoint()).ToList();
    
        // Get position of car passed in OnPassCheckpoint
        int carPosition = carLapCounters.IndexOf(carLapCounter) + 1;

        carLapCounter.SetCarPosition(carPosition);
        leaderboardUIHandler.UpdateList(carLapCounters);
    }
}
