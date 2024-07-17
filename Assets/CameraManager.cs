/*
using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraManager : MonoBehaviour
{
    [Header("References")]
    private InputManager _inputManager;
    //private PlayerMovement _playerMovement;
    
    [Header("Target")]
    public Transform target; //Object to look at
    public Transform cameraPivot; //Object from where the camera rotates
    public Transform cameraTransform; //Transform of the real camera
    
    [Header("Camera Settings")]
    public LayerMask collisionLayers; // Layers for camera collision
    
    private float _defaultPosition; // Where the camera goes when there are no collisions
    private Vector3 _cameraFollowVelocity = Vector3.zero;
    private Vector3 _cameraVectorPosition;

    public Vector3 cameraOffset;

    public float cameraCollisionOffset = 0.2f;
    public float minimumCollisionOffset = 0.2f;
    public float cameraCollisionRadius = 0.2f;
    public float cameraFollowSpeed = 0.2f;
    public float cameraPitchSpeed = 2;
    public float cameraYawSpeed = 2;

    public float pitchAngle; //Up and Down
    public float yawAngle;// Left and Right

    public float minimumPitchAngle = -35;
    public float maximumPitchAngle = 35;
    
    [Header("Half Lash Settings")]
    public Vector3 halfLashRotationOffset;
    
    [Header("Transition Settings")]
    public float transitionLerpAmount = 1.5f;
    public float normalLerpAmount = 25f;
    
    public float transitionTime = 1.5f;
    public float lerpAmount;
    public float transitionTimer = 0f;
    public bool transitionStart = false;
    
    [Header("Camera Mode")]
    public CameraMode cameraMode = CameraMode.Normal;
    public CameraMode previousMode;

        

    public void Awake()
    {
        target = FindObjectOfType<PlayerManager>().transform;
        _inputManager = FindObjectOfType<InputManager>();
        //_playerMovement = FindObjectOfType<PlayerMovement>();
        if (Camera.main != null) 
            cameraTransform = Camera.main.transform;
        _defaultPosition = cameraTransform.localPosition.z;
    }

    public void HandleAllCameraMovement()
    {
        previousMode = cameraMode;
        
        /*if (_playerMovement.isHalfLashing)
            cameraMode =  CameraMode.HalfLash;
        
        else if (_playerMovement.isLashing)
            cameraMode =  CameraMode.Lash;
        
        else#1#
            cameraMode =  CameraMode.Normal;


        /*
        if (previousMode != cameraMode)
        {
            transitionStart = true;
        }

        if (transitionStart)
        {
            transitionTimer += Time.deltaTime;
            if (transitionTimer > transitionTime)
            {
                transitionTimer = 0f;
                transitionStart = false;
            }
        }
        #1#
        
        FollowTarget();
        RotateCamera();
        //HandleCameraCollisions();
    }

    public enum CameraMode
    {
        Normal,
        HalfLash,
        Lash
        
    }

    private void FollowTarget()
    {
        float offsetMultiplier = 1;
        if (cameraMode != CameraMode.Normal) offsetMultiplier = 2;
        
        Vector3 targetPosition = 
            Vector3.SmoothDamp(transform.position, target.position + cameraOffset * offsetMultiplier, ref _cameraFollowVelocity, cameraFollowSpeed);

        transform.position = targetPosition;
    }

    private void RotateCamera()
    {
        lerpAmount =  (normalLerpAmount - transitionLerpAmount) / 2 * Mathf.Cos(Mathf.PI * transitionTimer / transitionTime) + (normalLerpAmount + transitionLerpAmount) / 2;
        
        switch (cameraMode){
            case CameraMode.Normal:
                RotateNormalCamera();
                break;
            case CameraMode.HalfLash:
                RotateHalfLashCamera();
                break;
            case CameraMode.Lash:
                RotateHalfLashCamera();
                break;
        }
        
    }


    private void RotateHalfLashCamera()
    {
        Quaternion targetRotation = target.rotation;
        targetRotation *= Quaternion.Euler(halfLashRotationOffset);
        
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * lerpAmount);
    }

    private void RotateNormalCamera()
    {
        pitchAngle += _inputManager.LookInput.y * cameraPitchSpeed;
        pitchAngle = Mathf.Clamp(pitchAngle, minimumPitchAngle, maximumPitchAngle);
        yawAngle += _inputManager.LookInput.x * cameraYawSpeed;


        // Get the player's current rotation due to gravity.
        Quaternion playerGravityRotation = Quaternion.FromToRotation(Vector3.up, target.up);

        // Combine the player's gravity rotation with the camera's pitch and yaw rotations.
        Quaternion targetRotation = playerGravityRotation * Quaternion.Euler(new Vector3(pitchAngle, yawAngle, 0));
        
        
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * lerpAmount);
        
        // Apply the same rotation to the camera pivot.
        cameraPivot.localRotation = Quaternion.Euler(Vector3.zero);
    }



    private void HandleCameraCollisions()
    {
        float targetPosition = _defaultPosition;
        Vector3 direction = cameraTransform.position - cameraPivot.position;
        direction.Normalize();

        if (Physics.SphereCast(cameraPivot.transform.position, cameraCollisionRadius, direction, out var hit,
                Mathf.Abs(targetPosition), collisionLayers))
        {
            float distance = Vector3.Distance(cameraPivot.position, hit.point);
            targetPosition =- (distance - cameraCollisionOffset);
        }

        if (Mathf.Abs(targetPosition) < minimumCollisionOffset)
        {
            targetPosition -= minimumCollisionOffset;
        }

        _cameraVectorPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, 0.2f);
        cameraTransform.localPosition = _cameraVectorPosition;
    }
}
*/

using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using Unity.Collections;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private InputManager inputManager;
    
    public Transform target; //Object to look at
    public Transform cameraPivot; //Object from where the camera rotates
    [ReadOnly]
    public Transform cameraTransform; //Transform of the real camera

    public LayerMask collisionLayers; // Layers for camera collision
    
    private float defaultPosition; // Where the camera goes when there are no collisions
    private Vector3 cameraFollowVelocity = Vector3.zero;
    private Vector3 cameraVectorPosition;

    public float cameraCollisionOffset = 0.2f;
    public float minimumCollisionOffset = 0.2f;
    public float cameraCollisionRadius = 0.2f;
    public float cameraFollowSpeed = 0.1f;
    public float cameraPitchSpeed = 15;
    public float cameraYawSpeed = 15;
    public float _camLookSmoothTime = 1f;
    
    public float pitchAngle; //Up and Down
    public float yawAngle;// Left and Right

    public float minimumPitchAngle = -35;
    public float maximumPitchAngle = 35;


    public void Awake()
    {
        inputManager = FindObjectOfType<InputManager>();
        cameraTransform = Camera.main.transform;
        defaultPosition = cameraTransform.localPosition.z;
    }

    public void HandleAllCameraMovement()
    {
        FollowTarget();
        RotateCamera();
        //HandleCameraCollisions();
    }

    private void FollowTarget()
    {
        Vector3 targetPosition = 
            Vector3.SmoothDamp(transform.position, target.position, ref cameraFollowVelocity, cameraFollowSpeed);

       
        transform.position = targetPosition;
    }

    private void RotateCamera()
    {
        Vector3 rotation;
        Quaternion targetRotation;
            
        /*pitchAngle += inputManager.LookInput.y * cameraPitchSpeed;
        pitchAngle = Mathf.Clamp(pitchAngle, minimumPitchAngle, maximumPitchAngle);
        yawAngle += inputManager.LookInput.x * cameraYawSpeed;*/
        
        
        
       yawAngle = Mathf.Lerp(yawAngle, yawAngle + (inputManager.LookInput.x * cameraYawSpeed), _camLookSmoothTime * Time.deltaTime);

       pitchAngle = Mathf.Lerp(pitchAngle, pitchAngle - (inputManager.LookInput.y * cameraPitchSpeed), _camLookSmoothTime * Time.deltaTime);

        rotation = Vector3.zero;
        rotation.y = yawAngle;
        targetRotation = Quaternion.Euler(rotation);
        transform.rotation = targetRotation;

        rotation = Vector3.zero;
        rotation.x = pitchAngle;
        targetRotation = Quaternion.Euler(rotation);
        cameraPivot.localRotation = targetRotation;
    }

    private void HandleCameraCollisions()
    {
        float targetPosition = defaultPosition;
        RaycastHit hit;
        Vector3 direction = cameraTransform.position - cameraPivot.position;
        direction.Normalize();

        if (Physics.SphereCast(cameraPivot.transform.position, cameraCollisionRadius, direction, out hit,
                Mathf.Abs(targetPosition), collisionLayers))
        {
            float distance = Vector3.Distance(cameraPivot.position, hit.point);
            targetPosition =- (distance - cameraCollisionOffset);
        }

        if (Mathf.Abs(targetPosition) < minimumCollisionOffset)
        {
            targetPosition -= minimumCollisionOffset;
        }

        cameraVectorPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, 0.2f);
        cameraTransform.localPosition = cameraVectorPosition;
    }
}

