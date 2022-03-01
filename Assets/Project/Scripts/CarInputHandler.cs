using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarInputHandler : MonoBehaviour {
    CarControllerMark2 carControllerMark2;

    private void Awake() {
        carControllerMark2 = GetComponent<CarControllerMark2>();
    }

    private void Update() {
        Vector2 inputVector = Vector2.zero;
        inputVector.x = Input.GetAxis("Horizontal");
        inputVector.y = Input.GetAxis("Vertical");
        carControllerMark2.SetInputVector(inputVector);

        if(Input.GetButtonDown("Jump")) {
            carControllerMark2.Jump(1.0f, 0.0f);
        }
    }
}
