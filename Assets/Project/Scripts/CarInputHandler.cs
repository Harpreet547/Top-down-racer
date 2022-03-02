using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarInputHandler : MonoBehaviour {

    public int playerNumber = 1;
    public bool isUIInput = false;

    Vector2 inputVector = Vector2.zero;

    CarControllerMark2 carControllerMark2;

    private void Awake() {
        carControllerMark2 = GetComponent<CarControllerMark2>();
    }

    private void Update() {
        if(isUIInput) {
            carControllerMark2.SetInputVector(inputVector);
            return;
        }

        inputVector = Vector2.zero;
        switch (playerNumber)
        {
            case 1:
                Debug.Log(Input.GetAxis("Vertical_P1"));
                inputVector.x = Input.GetAxis("Horizontal_P1");
                inputVector.y = Input.GetAxis("Vertical_P1");
                break;
            case 2:
                inputVector.x = Input.GetAxis("Horizontal_P2");
                inputVector.y = Input.GetAxis("Vertical_P2");
                break;
            // case 3:
            //     inputVector.x = Input.GetAxis("Horizontal_P3");
            //     inputVector.y = Input.GetAxis("Vertical_P3");
            //     break;
            // case 4:
            //     inputVector.x = Input.GetAxis("Horizontal_P4");
            //     inputVector.y = Input.GetAxis("Vertical_P4");
            //     break;
            default:
                break;
        }
        carControllerMark2.SetInputVector(inputVector);
    }

    public void SetInput(Vector2 newInput) {
        inputVector = newInput;
    }
}
