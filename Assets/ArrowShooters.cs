using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class ArrowShooters : MonoBehaviour{
    public bool shoot;
    public GameObject arrow;
    public Vector3 direction;
    public float strength;
    void Start()
    {
       shoot = true; 
    }

    void Update()
    {
        if (shoot) {
            ShootArrow();
        }
    }

    public void ShootArrow() {
        GameObject t = Instantiate(arrow, transform.position, Quaternion.identity);
        t.gameObject.GetComponent<ArrowController>().SetVelocity(transform.forward * strength);
        shoot = false;

    }
    
}
