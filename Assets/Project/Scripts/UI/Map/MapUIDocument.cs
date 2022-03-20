using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MapUIDocument : MonoBehaviour {
    public Button race1;
    public Button race2;
    public Button race3;
    public Button race4;

    private void Start() {
        VisualElement  root = GetComponent<UIDocument>().rootVisualElement;
        race1 = root.Q<Button>("Race_1");
        race2 = root.Q<Button>("Race_2");
        race3 = root.Q<Button>("Race_3");
        race4 = root.Q<Button>("Race_4");

        race1.clicked += Race1Clicked;
        race2.clicked += Race2Clicked;
        race3.clicked += Race3Clicked;
        race4.clicked += Race4Clicked;
    }

    private void Race1Clicked() {
        SceneManager.LoadScene("SpawnCar");
    }
    private void Race2Clicked() {
        SceneManager.LoadScene("SampleScene");
    }
    private void Race3Clicked() {
        SceneManager.LoadScene("Grid1");
    }
    private void Race4Clicked() {
        SceneManager.LoadScene("AITestingGround");
    }
}
