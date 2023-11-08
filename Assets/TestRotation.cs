using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Quaternion = System.Numerics.Quaternion;

public class TestRotation : MonoBehaviour
{

    public Transform _transform;
    public Rigidbody _rb;
    public float rotationSpeed = 10f;
    public Vector3 gravity = new Vector3(0,-1,0);
    public Vector3 rotationDirection = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        _transform = transform;
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        _transform.Rotate(rotationDirection);

        gravity = - _transform.up;
            
        _rb.AddForce(gravity, ForceMode.Acceleration);

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(_transform.position, gravity);
    }
}
