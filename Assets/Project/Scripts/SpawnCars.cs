using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCars : MonoBehaviour {

    private void Start() {
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
        CarData[] carDatas = Resources.LoadAll<CarData>("CarData/");

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            Transform spawnPoint = spawnPoints[i].transform;
            // Currently we are only saving player 1 car.
            int playerSelectedCar = PlayerPrefs.GetInt($"P{i + 1}SelectedCarID");
            foreach (var carData in carDatas)
            {
                if (playerSelectedCar == carData.CarUniqueID)
                {
                    GameObject playerCar = Instantiate(carData.CarPrefab, spawnPoint.position, spawnPoint.rotation);
                    playerCar.GetComponent<CarInputHandler>().playerNumber = i + 1;
                    break;
                }
            };
        }

        GameObject.FindObjectOfType<LeaderboardUIHandler>().InitLeaderboard();
    }
}
