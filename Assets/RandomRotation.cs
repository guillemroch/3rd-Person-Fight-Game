using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRotation : MonoBehaviour
{
    

    void Update() {
        
        float randomAngleZ = Random.Range(-36f, 36f);
        float randomAngleX = Random.Range(-36f, 36f);
        float randomAngleY = Random.Range(6f, 100f);
        Quaternion randomRotationY = Quaternion.AngleAxis(randomAngleY, Vector3.up);
        Quaternion randomRotationZ = Quaternion.AngleAxis(randomAngleZ , Vector3.forward);
        Quaternion randomRotationX = Quaternion.AngleAxis(randomAngleX , Vector3.right);
        transform.rotation =Quaternion.Slerp(transform.rotation, transform.rotation * randomRotationY * randomRotationX * randomRotationZ, 0.4f);
        
    }
}
