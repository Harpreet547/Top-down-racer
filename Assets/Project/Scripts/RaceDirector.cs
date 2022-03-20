using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

enum RaceStartLights {
    red, yellow, green,
}

public class RaceDirector : MonoBehaviour {
    public bool shouldSpawn = true;
    public Image startLights;
    public Sprite redLight;
    public Sprite yellowLight;
    public Sprite greenLight;

    bool isRaceCountdownCORunning = false;
    bool isRaceStarted = false;
    bool areCarsActivated = false;

    RaceStartLights? raceStartLights;

    private void Awake() {
        SpawnCars spawnCars = GetComponent<SpawnCars>();
        if(shouldSpawn) {
            spawnCars.SpawnPlayer();
            spawnCars.SpawnAI();
        }
        // raceStartLights = RaceStartLights.red;
        startLights.enabled = false;
        GameObject.FindObjectOfType<LeaderboardUIHandler>().InitLeaderboard();
    }

    private void Update() {

        if(raceStartLights == RaceStartLights.green && isRaceStarted && !areCarsActivated) {
            ActivateCars();
        }

        if(!isRaceCountdownCORunning && !(raceStartLights == RaceStartLights.green)) {
            StartCoroutine(RaceCountDownCO());
        }
    }

    private void ActivateCars() {
        CarControllerMark2[] carsOnTrack = GameObject.FindObjectsOfType<CarControllerMark2>();
        foreach (var car in carsOnTrack) {
            if(car.CompareTag("Player")) car.GetComponent<CarInputHandler>().enabled = true;
            else if(car.CompareTag("AI")) car.GetComponent<CarAIHandler>().enabled = true;
        }
        areCarsActivated = true;
    }

    public IEnumerator RaceCountDownCO() {
        isRaceCountdownCORunning = true;

        yield return new WaitForSeconds(1.0f);
        
        switch (raceStartLights)
        {
            case RaceStartLights.yellow:
                raceStartLights = RaceStartLights.green;
                startLights.sprite = greenLight;
                Invoke("DisableStartLightsAfterDelay", 0.5f);
                isRaceStarted = true;
                break;
            case RaceStartLights.red:
                raceStartLights = RaceStartLights.yellow;
                startLights.sprite = yellowLight;
                break;
            default:
                raceStartLights = RaceStartLights.red;
                startLights.sprite = redLight;
                startLights.enabled = true;
                break;
        }
        isRaceCountdownCORunning = false;
    }

    // Called using invoke method
    void DisableStartLightsAfterDelay() {
        startLights.enabled = false;
    }
}
