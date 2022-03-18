using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCars : MonoBehaviour {

    public bool shouldSpawn = true;
    private void Awake() {
        if(!shouldSpawn) return;
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
        CarData[] carDatas = Resources.LoadAll<CarData>("CarData/");
        CarData[] aiCarDatas = Resources.LoadAll<CarData>("AICarData/");
        
        // Keep track of players. Populate rest as of the cars as AI.
        int numberOfPlayers = 0;
        for (int i = 0; i < spawnPoints.Length; i++) {
            Transform spawnPoint = spawnPoints[i].transform;
            // Currently we are only saving player 1 car.
            int playerSelectedCar = PlayerPrefs.GetInt($"P{i + 1}SelectedCarID");
            if(playerSelectedCar != 0) {
                foreach (var carData in carDatas)
                {
                    if (playerSelectedCar == carData.CarUniqueID)
                    {
                        GameObject playerCar = Instantiate(carData.CarPrefab, spawnPoint.position, spawnPoint.rotation);
                        playerCar.GetComponent<CarInputHandler>().playerNumber = i + 1;
                        break;
                    }
                };
                numberOfPlayers++;
            }
        }

        // Populate Remaining players as AI Cars.
        // TODO: Create CarData for AI cars in future using prefab variants and populate from those.
        // TODO: Create a system to spawn AI according to level and increase difficulty if it is a latter level.
        for (int i = numberOfPlayers; i < spawnPoints.Length; i++) {
            Transform spawnPoint = spawnPoints[i].transform;

            CarData aiCarData = aiCarDatas[Random.Range(0, aiCarDatas.Length - 1)];
            GameObject aiCar = Instantiate(aiCarData.CarPrefab, spawnPoint.position, spawnPoint.rotation);
            aiCar.GetComponent<CarInputHandler>().playerNumber = i + 1;
            aiCar.tag = "AI";
        }

        GameObject.FindObjectOfType<LeaderboardUIHandler>().InitLeaderboard();
    }
}
