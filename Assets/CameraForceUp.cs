using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;


public class CameraForceUp : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public Transform playerTransform;
    
    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.Find("Player").transform;
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //Force the camera orientation to be at the back of the player so that it is looking up
        
    }
}
