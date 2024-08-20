using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReverseInfused : Infusable
{
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnParticleCollision(GameObject other) {
       Debug.Log("Particle collided"); 
    }

    public void Interact(out int value) {
        value = 1;
    }
}
