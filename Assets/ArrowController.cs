using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour{
    public Rigidbody rb;
    public bool enable = true;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start() {
        //rb.velocity = Vector3.forward * 10;
    }

    // Update is called once per frame
    void Update() {
        if (enable) {
            Quaternion targetRotation = Quaternion.FromToRotation(-transform.forward, rb.velocity.normalized);
            targetRotation = Quaternion.LookRotation(-rb.velocity.normalized, transform.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 1f);
        }
       
    }

    public void SetVelocity(Vector3 direction) {
        rb.velocity = direction;
    }


    public void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, rb.velocity);
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward);
    }

    private void OnCollisionEnter(Collision other) {
        enable = false;
        rb.isKinematic = true;
        rb.velocity = Vector3.zero;
    }
}
