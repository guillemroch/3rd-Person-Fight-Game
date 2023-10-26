using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private InputManager _inputManager;
    private PlayerMovement _playerMovement;
    
    public Transform target; //Object to look at
    public Transform cameraPivot; //Object from where the camera rotates
    public Transform cameraTransform; //Transform of the real camera

    public LayerMask collisionLayers; // Layers for camera collision
    
    private float defaultPosition; // Where the camera goes when there are no collisions
    private Vector3 cameraFollowVelocity = Vector3.zero;
    private Vector3 cameraVectorPosition;

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
    
    public float transitionSlerpAmount = 0.2f;
    public CameraMode cameraMode = CameraMode.Normal;

        

    public void Awake()
    {
        target = FindObjectOfType<PlayerManager>().transform;
        _inputManager = FindObjectOfType<InputManager>();
        _playerMovement = FindObjectOfType<PlayerMovement>();
        cameraTransform = Camera.main.transform;
        defaultPosition = cameraTransform.localPosition.z;
    }

    public void HandleAllCameraMovement()
    {
        
        if (_playerMovement.isHalfLashing)
        {
            cameraMode =  CameraMode.HalfLash;
        }
        else
        {
            cameraMode =  CameraMode.Normal;
        }
        
        FollowTarget();
        RotateCamera();
        HandleCameraCollisions();
    }

    public enum CameraMode
    {
        Normal,
        HalfLash,
        Lash,
        Transition
    }

    private void FollowTarget()
    {
        Vector3 targetPosition = 
            Vector3.SmoothDamp(transform.position, target.position + cameraOffset, ref cameraFollowVelocity, cameraFollowSpeed);

        transform.position = targetPosition;
    }

    private void RotateCamera()
    {
        switch (cameraMode){
            case CameraMode.Normal:
                RotateNormalCamera();
                break;
            case CameraMode.HalfLash:
                RotateHalfLashCamera();
                break;
            case CameraMode.Transition:
                RotateTransitionCamera();
                break;
        }
        
    }

    private void RotateTransitionCamera()
    {
        
        // ! Idea: Use the transitionSlerpAmount to make the transition smooth. when high is like no Lerp, when low is very smooth
        // ! Can use it with a timer to make the transition time and multiply the Lerp amount
        
        
    }

    private void RotateHalfLashCamera()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, Time.deltaTime * transitionSlerpAmount);
    }

    private void RotateNormalCamera()
    {
        pitchAngle += _inputManager.cameraInput.y * cameraPitchSpeed;
        pitchAngle = Mathf.Clamp(pitchAngle, minimumPitchAngle, maximumPitchAngle);
        yawAngle += _inputManager.cameraInput.x * cameraYawSpeed;


        // Get the player's current rotation due to gravity.
        Quaternion playerGravityRotation = Quaternion.FromToRotation(Vector3.up, target.up);

        // Combine the player's gravity rotation with the camera's pitch and yaw rotations.
        Quaternion targetRotation = playerGravityRotation * Quaternion.Euler(new Vector3(pitchAngle, yawAngle, 0));
        
        // // transform.rotation = targetRotation;
        
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * transitionSlerpAmount);
        
        // Apply the same rotation to the camera pivot.
        cameraPivot.localRotation = Quaternion.Euler(Vector3.zero);
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
