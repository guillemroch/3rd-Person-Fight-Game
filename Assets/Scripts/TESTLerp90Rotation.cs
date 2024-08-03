using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Unity.Collections;
using UnityEngine;

public class TESTLerp90Rotation : MonoBehaviour
{
    public float maxAngle = 90;//maximum angle it can rotate
    public float direction = 0; //Direction of the rotation
    [Range(0, 1)]
    public float precision = 0.99f; //Defined for the calculation of the maximum time to reach the max angle
    [Range(0, 5)]
    public float damping = 2.1f; //How smooth the rotation is at the end
    [Range(0, 1)]
    public float lerpSpeed = 0.5f;
    public float offset = 0; //Calculated based on the precision, the maximum angle and the damping
    public float maxTime = 1; //Calculated based on the precision, the maximum angle and the damping
    public float timeElapsed = 0;
    
    
    public bool leftInput = false;
    public bool rightInput = false;
    
    public Vector3 rotationAxis = Vector3.forward;
    
    
    [Unity.Collections.ReadOnly] public Transform transform;
    // Start is called before the first frame update
    void Start()
    {
        transform = GetComponent<Transform>();
        
        float limit = maxAngle* precision;
        //Maximum time to reach the max angle
        offset = (Mathf.Log((-limit)/(-maxAngle + limit)) / damping); 
        maxTime = offset*2;
    }

    // Update is called once per frame
    void Update() {

        if (leftInput || rightInput) {
            if (Mathf.Abs(timeElapsed) <= maxTime) {
                if (leftInput && direction != 11)
                    direction = -1;
                else if (rightInput && direction != -1)
                    direction = 1;
                timeElapsed += Time.deltaTime;
            }
        }
        else {
            if (timeElapsed > 1 - precision) {
                timeElapsed -= Time.deltaTime ;
            }
            direction = 0;
            
                
        }
        
        
        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(rotationAxis * direction * (maxAngle * Sigmoid(timeElapsed-offset))), lerpSpeed);
    }
    
    public float Sigmoid(float x) {
        return (1 / (1 + Mathf.Exp(-damping * x)));
    }
    
}
