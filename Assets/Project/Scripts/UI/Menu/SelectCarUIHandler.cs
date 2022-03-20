using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectCarUIHandler : MonoBehaviour {
    [Header("Car prefab")]
    public GameObject carPrefab;

    [Header("Spawn on")]
    public Transform spawnOnTransform;

    bool isChangingCar = false;
    CarData[] carDatas;
    int selectedCarIndex = 0;
    CarUIHandler carUIHandler = null;

    private void Start() {
        carDatas = Resources.LoadAll<CarData>("CarData/");
        StartCoroutine(SpawnCarCO(true));
    }

    private void Update() {
        if(Input.GetKey(KeyCode.LeftArrow)) OnPreviousCar();
        else if(Input.GetKey(KeyCode.RightArrow)) OnNextCar();

        if(Input.GetKeyDown(KeyCode.Space)) OnSelectCar();
    }

    public void OnPreviousCar() {
        if(isChangingCar) return;

        selectedCarIndex--;
        if(selectedCarIndex < 0) selectedCarIndex = carDatas.Length - 1;
        StartCoroutine(SpawnCarCO(true));
    }

    public void OnNextCar() {
        if(isChangingCar) return;

        selectedCarIndex++;
        if(selectedCarIndex > carDatas.Length - 1) selectedCarIndex = 0;
        StartCoroutine(SpawnCarCO(false));
    }

    public void OnSelectCar() {
        PlayerPrefs.SetInt("P1SelectedCarID", carDatas[selectedCarIndex].CarUniqueID);
        PlayerPrefs.Save();

        SceneManager.LoadScene("Map");
    }

    IEnumerator SpawnCarCO(bool isCarAppearingOnRightSide) {
        isChangingCar = true;
        if(carUIHandler != null) carUIHandler.StartCarExitAnimation(!isCarAppearingOnRightSide);

        GameObject instantiatedCar = Instantiate(carPrefab, spawnOnTransform);
        carUIHandler = instantiatedCar.GetComponent<CarUIHandler>();
        carUIHandler.SetupCar(carDatas[selectedCarIndex]);
        carUIHandler.StartCarEntanceAnimation(isCarAppearingOnRightSide);

        yield return new WaitForSeconds(0.4f);

        isChangingCar = false;
    }
}
