using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using Unity.Collections;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public enum CameraMode{
        Normal, 
        HalfLash, 
        Lash
    }
    [Header("References")]
    [SerializeField]
    private Transform _target; //Object to look at
    [SerializeField]
    private Transform _cameraPivot; //Object from where the camera rotates
    
    private InputManager _inputManager;
    private Transform _cameraTransform; //Transform of the real camera
    
    private float _defaultPosition; // Where the camera goes when there are no collisions
    private Vector3 _cameraFollowVelocity = Vector3.zero;
    private Vector3 _cameraVectorPosition;
    
    [Header("Collisions")]
    [SerializeField]
    private float _cameraCollisionOffset = 0.2f;
    [SerializeField]
    private float _minimumCollisionOffset = 0.2f;
    [SerializeField]
    private float _cameraCollisionRadius = 0.2f;
    [SerializeField]
    private LayerMask _collisionLayers; // Layers for camera collision
    
    [Header("Movement Speed")]
    [SerializeField]
    private float _cameraFollowSpeed = 0.1f;
    [SerializeField]
    private float _cameraPitchSpeed = 15;
    [SerializeField]
    private float _cameraYawSpeed = 15;
    [SerializeField]
    [Tooltip("The higher the value the faster the camera moves")]
    private float _camLookSmoothTime = 25f;
    
    private float pitchAngle; //Up and Down
    private float yawAngle;// Left and Right
    
    [Header("Limits")]
    [SerializeField]
    private float minimumPitchAngle = -35;
    [SerializeField]
    private float maximumPitchAngle = 35;

    [Header("Camera Modes and Settings")]
    [SerializeField] private static CameraMode _cameraMode = CameraMode.Normal;

    [SerializeField] private Vector3 _cameraHalfLashOffset;
    
    public void Awake()
    {
        _inputManager = FindObjectOfType<InputManager>();
        _cameraTransform = Camera.main.transform;
        _defaultPosition = _cameraTransform.localPosition.z;
    }

    public void HandleAllCameraMovement()
    {
        if (_cameraMode == CameraMode.Normal) {
            FollowTarget();
            RotateCamera();
            HandleCameraCollisions();
        }else if (_cameraMode == CameraMode.HalfLash) {
            FollowTargetHalfLash();
            RotateCamera();
            HandleCameraCollisions();
        }else if (_cameraMode == CameraMode.Lash) {
            
        }
        else {
            Debug.LogError("No camera mode defined!");
        }
        
    }

    private void FollowTarget()
    {
        Vector3 lookAtPosition = 
            Vector3.SmoothDamp(transform.position, _target.position, ref _cameraFollowVelocity, _cameraFollowSpeed);
        transform.position = lookAtPosition;
    }
    private void FollowTargetHalfLash()
    {
        _cameraTransform.localPosition = _cameraHalfLashOffset;
        Vector3 lookAtPosition = 
            Vector3.SmoothDamp(transform.position, _target.position, ref _cameraFollowVelocity, _cameraFollowSpeed);
        transform.position = lookAtPosition;
        
    }
    private void RotateCamera()
    {
        Vector3 rotation;
        Quaternion targetRotation;
        
       yawAngle = Mathf.Lerp(yawAngle, yawAngle + (_inputManager.LookInput.x * _cameraYawSpeed), _camLookSmoothTime * Time.deltaTime);

       pitchAngle = Mathf.Lerp(pitchAngle, pitchAngle - (_inputManager.LookInput.y * _cameraPitchSpeed), _camLookSmoothTime * Time.deltaTime);

        rotation = Vector3.zero;
        rotation.y = yawAngle;
        targetRotation = Quaternion.Euler(rotation);
        transform.rotation = targetRotation;

        rotation = Vector3.zero;
        rotation.x = pitchAngle;
        targetRotation = Quaternion.Euler(rotation);
        _cameraPivot.localRotation = targetRotation;
    }

    private void HandleCameraCollisions()
    {
        float targetPosition = _defaultPosition;
        RaycastHit hit;
        Vector3 direction = _cameraTransform.position - _cameraPivot.position;
        direction.Normalize();

        if (Physics.SphereCast(_cameraPivot.transform.position, _cameraCollisionRadius, direction, out hit,
                Mathf.Abs(targetPosition), _collisionLayers))
        {
            float distance = Vector3.Distance(_cameraPivot.position, hit.point);
            targetPosition =- (distance - _cameraCollisionOffset);
        }

        if (Mathf.Abs(targetPosition) < _minimumCollisionOffset)
        {
            targetPosition -= _minimumCollisionOffset;
        }

        _cameraVectorPosition.z = Mathf.Lerp(_cameraTransform.localPosition.z, targetPosition, 0.2f);
        _cameraTransform.localPosition = _cameraVectorPosition;
    }


    public static void SetCameraMode(CameraMode cameraMode) {
        _cameraMode = cameraMode;
    }
}

