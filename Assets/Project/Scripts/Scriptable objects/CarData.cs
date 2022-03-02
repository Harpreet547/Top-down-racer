using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New car data", menuName = "Car data", order = 51)]
public class CarData : ScriptableObject {
    [SerializeField]
    private int carUniqueID = 0;

    [SerializeField]
    private Sprite carUISprite;

    [SerializeField]
    private GameObject carPrefab;

    public int CarUniqueID {
        get { return carUniqueID; }
    }

    public Sprite CarUISprite {
        get { return carUISprite; }
    }

    public GameObject CarPrefab {
        get { return carPrefab; }
    }
}
