using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickable : MonoBehaviour, Interactable{
    [SerializeField] private int _value;
        
    void Update()
    {
        
    }

    public void Interact(out int value) {
        value = _value;
        DestroyImmediate(gameObject);  
    }

    public void Overlay() {
        Debug.Log("Overlay");
    }
}
