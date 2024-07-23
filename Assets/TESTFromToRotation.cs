using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TESTFromToRotation : MonoBehaviour{
    [SerializeField] private Transform targetObject;

    [SerializeField] private Quaternion rotation;

    [SerializeField] private Vector3 eulerRotation;

    [SerializeField] [Range(0, 20f)] private float lerp;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update() {
        rotation = Quaternion.FromToRotation(transform.up, targetObject.up);
        rotation *= transform.rotation;
        //eulerRotation = rotation.eulerAngles;
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, lerp * Time.deltaTime);
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.green; 
        Gizmos.DrawRay(targetObject.position, targetObject.up);
        Gizmos.DrawRay(transform.position, transform.up);
        Gizmos.color = Color.red; 
        Gizmos.DrawRay(targetObject.position, targetObject.right);
        Gizmos.DrawRay(transform.position, transform.right);
        Gizmos.color = Color.blue; 
        Gizmos.DrawRay(targetObject.position, targetObject.forward);
        Gizmos.DrawRay(transform.position, transform.forward);
    }
}
