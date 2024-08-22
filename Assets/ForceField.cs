using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceField : MonoBehaviour{
    public LayerMask mask;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerEnter(Collider other) {
        Debug.Log("Projectile entered");
        if(other.gameObject.layer == LayerMask.NameToLayer("Projectile")) {
            
            float distance = Vector3.Distance(other.transform.position, transform.position);
            Vector3 targetVelocity = (1 / distance) * 10 * (transform.position - other.transform.position);
            
            other.gameObject.GetComponent<Rigidbody>().velocity = targetVelocity;

        }
    }

    private void OnTriggerStay(Collider other) {
        if(((1<<other.gameObject.layer) & mask) != 0){
            Debug.Log("Projectile stayed");
            float distance = Vector3.Distance(other.transform.position, transform.position);
            Vector3 targetVelocity = (1 / distance)  * (transform.position - other.transform.position);
            
            other.gameObject.GetComponent<Rigidbody>().velocity = targetVelocity;

        }
    }
}
