using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour{
    [SerializeField] private Transform objectToFollow;
    [SerializeField] private float followLerp;
    

    void Update() {
        transform.rotation = Quaternion.identity;
        transform.position = Vector3.Lerp(transform.position, objectToFollow.position, followLerp*Time.deltaTime);
    }
}
