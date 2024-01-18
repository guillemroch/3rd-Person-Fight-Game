using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRotation : MonoBehaviour
{

    public Transform _transform;
    public Transform _gravity;
    public Quaternion _rotation;
    void Start()
    {
        _transform = transform;
        
    }

    // Update is called once per frame
    void Update()
    {
        Quaternion rotation = _transform.rotation * _gravity.rotation;
        if (_transform.rotation != _rotation)
        {
            _gravity.rotation = rotation * _gravity.rotation;
            _rotation = _transform.rotation;
        }
        

    }
    
}
