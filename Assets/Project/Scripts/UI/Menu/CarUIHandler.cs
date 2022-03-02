using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarUIHandler : MonoBehaviour {

    [Header("Car detail")]
    public Image carImage;

    Animator animator;

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    public void SetupCar(CarData carData) {
        carImage.sprite = carData.CarUISprite;
    }

    public void StartCarEntanceAnimation(bool isAppearingOnRightSide) {
        if(isAppearingOnRightSide) animator.Play("Car UI appear from right");
        else animator.Play("Car UI appear from left");
    }

    public void StartCarExitAnimation(bool isExitOnRightSide) {
        if(isExitOnRightSide) animator.Play("Car UI disappear to right");
        else animator.Play("Car UI disappear to left");
    }

    // Event
    public void OnCarExitAnimationCompleted() {
        Destroy(gameObject);
    }
}
