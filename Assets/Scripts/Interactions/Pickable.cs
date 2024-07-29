using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickable : MonoBehaviour, Interactable{
    [SerializeField] private int _value;
        
    void Update()
    {
        
    }

    public void Interact(out int value) {
        
        Debug.Log("Interacted with Pickle");
        value = _value;
        DestroyImmediate(this.gameObject);  
        
    }

    public void Overlay() {
        Debug.Log("Overlay");
    }
}
