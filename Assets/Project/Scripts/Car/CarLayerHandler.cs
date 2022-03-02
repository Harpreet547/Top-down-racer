using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarLayerHandler : MonoBehaviour {
    public SpriteRenderer outlineSpriteRenderer;
    List<SpriteRenderer> carsLayerSpriteRenderer = new List<SpriteRenderer>();
    List<Collider2D> overpassColliderList = new List<Collider2D>();
    List<Collider2D> underpassColliderList = new List<Collider2D>();
    Collider2D carCollider;
    bool isDrivingOverpass = false;
    private void Awake() {
        foreach (var spriteRenderer in gameObject.GetComponentsInChildren<SpriteRenderer>()) {
            if(spriteRenderer.sortingLayerName == "Cars") {
                carsLayerSpriteRenderer.Add(spriteRenderer);
            }
        }

        foreach (var overpassColliderGameObject in GameObject.FindGameObjectsWithTag("OverpassCollider")) {
            overpassColliderList.Add(overpassColliderGameObject.GetComponent<Collider2D>());
        }

        foreach (var underpassColliderGameObject in GameObject.FindGameObjectsWithTag("UnderpassCollider")) {
            underpassColliderList.Add(underpassColliderGameObject.GetComponent<Collider2D>());
        }

        carCollider = GetComponentInChildren<Collider2D>();

        // Default drive on underpass
        carCollider.gameObject.layer = LayerMask.NameToLayer("ObjectOnUnderpass");
        
    }

    private void Start() {
        UpdateSortingAndCollisionLayers();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("UnderpassTrigger")) {
            isDrivingOverpass = false;
            carCollider.gameObject.layer = LayerMask.NameToLayer("ObjectOnUnderpass");
        } else if(other.CompareTag("OverpassTrigger")) {
            isDrivingOverpass = true;
            carCollider.gameObject.layer = LayerMask.NameToLayer("ObjectOnOverpass");
        }
        UpdateSortingAndCollisionLayers();
    }

    public bool GetIsDrivingOverpass() {
        return isDrivingOverpass;
    }

    void UpdateSortingAndCollisionLayers() {
        if(isDrivingOverpass) {
            SetSortingLayer("Bridges");
            outlineSpriteRenderer.enabled = false;
        } else {
            SetSortingLayer("Cars");
            outlineSpriteRenderer.enabled = true;
        }
        SetCollisionWithOverpass();
    }

    void SetCollisionWithOverpass() {
        foreach (var overpassCollider in overpassColliderList) {
            Physics2D.IgnoreCollision(carCollider, overpassCollider, !isDrivingOverpass);
        }

        foreach (var underpassCollider in underpassColliderList) {
            // Different in tutorial. tutorial was using if else condition.
            Physics2D.IgnoreCollision(carCollider, underpassCollider, isDrivingOverpass);
        }
    }

    void SetSortingLayer(string layerName) {
        foreach (var spriteRenderer in carsLayerSpriteRenderer) {
            spriteRenderer.sortingLayerName = layerName;
        }
    }
}
