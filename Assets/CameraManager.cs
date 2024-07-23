using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using Player.StateMachine;
using Unity.Collections;
using Unity.Mathematics;
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
    private Camera _camera;
    private PlayerStateMachine _playerStateMachine;
    
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

    [SerializeField]
    private int _normalFOV = 70;
    
    [SerializeField] private float pitchAngle; //Up and Down
    [SerializeField] private float yawAngle;// Left and Right
    
    [Header("Limits")]
    [SerializeField]
    private float minimumPitchAngle = -35;
    [SerializeField]
    private float maximumPitchAngle = 35;

    
    [SerializeField] private  CameraMode _cameraMode = CameraMode.Normal;
    [Header("Camera Modes and Settings")]
    [Header("Halflash")]
    [SerializeField] private Vector3 _cameraHalflashOffset;
    [SerializeField]
    private float _cameraHalflashPitchSpeed = 25;
    [SerializeField]
    private float _cameraHalflashYawSpeed = 25;

    [Header("Lash")] [SerializeField] private Vector3 _cameraLashOffset;
    
    [SerializeField] private int _lashFOV = 90;
    [SerializeField]
    private float _cameraLashFollowSpeed = 0.1f;
    [SerializeField]
    private float _cameraLashPitchSpeed = 15;
    [SerializeField]
    private float _cameraLashYawSpeed = 15;
    [SerializeField] private float _targetLashOffset = 1.5f;

    [SerializeField] private float _centerTimer = 0f;
    [SerializeField] private float _timeToCenter = 2f;
    [SerializeField] private Vector2 _centerCameraPosition; 
    
    
    public void Awake()
    {
        _inputManager = FindObjectOfType<InputManager>();
        _camera = Camera.main;
        _cameraTransform = _camera!.transform;
        _defaultPosition = _cameraTransform.localPosition.z;
        _playerStateMachine = FindObjectOfType<PlayerStateMachine>();
    }

    public void HandleAllCameraMovement()
    {
        if (_cameraMode == CameraMode.Normal) {
            FollowTarget();
            RotateCamera();
            HandleCameraCollisions();
        }else if (_cameraMode == CameraMode.HalfLash) {
            FollowTargetHalfLash();
            RotateCameraHalfLash();
            HandleCameraCollisions();
        }else if (_cameraMode == CameraMode.Lash) {
            FollowTargetLash();
            RotateCameraLash();
            HandleCameraCollisions();
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
        _cameraTransform.localPosition = _cameraHalflashOffset;
        Vector3 lookAtPosition = 
            Vector3.SmoothDamp(transform.position, _target.position, ref _cameraFollowVelocity, _cameraFollowSpeed);
        transform.position = lookAtPosition;
    }
    
    private void FollowTargetLash()
    {
        _cameraTransform.localPosition = _cameraLashOffset;
        Vector3 offsetedTargetPosition = _target.position + _target.up * (_targetLashOffset * _playerStateMachine.PlayerRigidbody.velocity.magnitude);
        Vector3 lookAtPosition = 
            Vector3.SmoothDamp(transform.position, offsetedTargetPosition, ref _cameraFollowVelocity, _cameraLashFollowSpeed);
        transform.position = lookAtPosition;
    }
    private void RotateCamera()
    {
        //1- Calculate user input
        //yawAngle = _inputManager.LookInput.x * _cameraYawSpeed;
        //pitchAngle = _inputManager.LookInput.y * _cameraPitchSpeed;
        yawAngle = Mathf.Lerp(yawAngle, yawAngle + (_inputManager.LookInput.x * _cameraYawSpeed), _camLookSmoothTime * Time.deltaTime);
        yawAngle = _inputManager.LookInput.x * _cameraYawSpeed;
        pitchAngle = Mathf.Lerp(pitchAngle, pitchAngle - (_inputManager.LookInput.y * _cameraPitchSpeed), _camLookSmoothTime * Time.deltaTime);
        Vector3 rotation = Vector3.zero;
        
        
        //2 - Apply yaw rotation to transform + Up Vector Correction
        rotation.y = yawAngle;
        Quaternion targetRotation = Quaternion.Euler(rotation);
        //Quaternion targetRotation = Quaternion.AngleAxis(yawAngle, transform.up);
        //transform.rotation =  Quaternion.FromToRotation(transform.up, _target.up) * targetRotation;
        transform.rotation = Quaternion.LookRotation(transform.forward, _target.up) * targetRotation;
        
        //3 - Rotate the pivot point for the pitch
        //targetRotation = Quaternion.AngleAxis(pitchAngle, _target.right);
        rotation = Vector3.zero;
        rotation.x = pitchAngle;
        targetRotation = quaternion.Euler(rotation);
        _cameraPivot.localRotation = targetRotation;
        
        /*
         Vector3 rotation;
        Quaternion targetRotation;
         // // transform.rotation = _playerStateMachine.PlayerTransform.rotation;
        transform.parent.transform.up = _playerStateMachine.PlayerTransform.up;

        //1 - Calcultate amount of movement inputed by the player.
       yawAngle = Mathf.Lerp(yawAngle, yawAngle + (_inputManager.LookInput.x * _cameraYawSpeed), _camLookSmoothTime * Time.deltaTime);

       yawAngle = _inputManager.LookInput.x * _cameraYawSpeed;
       pitchAngle = Mathf.Lerp(pitchAngle, pitchAngle - (_inputManager.LookInput.y * _cameraPitchSpeed), _camLookSmoothTime * Time.deltaTime);

       //2- Process this input and apply it as a real vector movement
       //! 1-  First move the Camera Manager transform
       rotation = Vector3.zero;
       rotation.y = yawAngle; //TODO: Change this so that it applies with the current gravity direction.
       //rotation = _playerStateMachine.PlayerTransform.up * yawAngle;
       // // targetRotation = Quaternion.AngleAxis(yawAngle, transform.up);
       targetRotation = Quaternion.Euler(rotation);

       // // Quaternion upwardsRotation = Quaternion.LookRotation(_playerStateMachine.PlayerTransform.forward, _playerStateMachine.PlayerTransform.up);

       /*Quaternion upAlignRotation = Quaternion.identity;
       if (Vector3.Angle(transform.up, _target.up) > 1) {
           Debug.Log("Transform up: " + transform.up + " | target up: " + _target.up + " | Angle: " + Vector3.Angle(transform.up, _target.up));
           upAlignRotation = Quaternion.FromToRotation(transform.up, _target.up);
           upAlignRotation *= transform.rotation;
           //transform.rotation = Quaternion.Slerp(transform.rotation, upAlignRotation, 20 * Time.deltaTime);

           //return;
       }#1#

       // // targetRotation = targetRotation * upRotation;//* transform.rotation;
       // // targetRotation = /*transform.rotation  *#1# upAlignRotation * Quaternion.AngleAxis(yawAngle, _target.up);
       //targetRotation = Quaternion.AngleAxis(yawAngle, _target.up);
       // // targetRotation = Quaternion.Slerp(transform.rotation ,targetRotation,  1f*Time.deltaTime);
       //Debug.Log("target rotation: " + targetRotation.eulerAngles + "| upAlignRotation = " + upAlignRotation.eulerAngles + " | inputRotation = " +  Quaternion.AngleAxis(yawAngle, _target.up).eulerAngles );

       //targetRotation = Quaternion.AngleAxis((_inputManager.LookInput.x * _cameraYawSpeed) , transform.up);
       targetRotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.5f*Time.deltaTime);
       //_cameraPivot.rotation = targetRotation;
         //  Quaternion.Slerp(transform.rotation ,targetRotation,  1f*Time.deltaTime);
         transform.rotation = Quaternion.LookRotation(transform.forward, _target.up) * targetRotation; //* Quaternion.Euler(rotation) ;
      // // transform.rotation = upRotation;// * targetRotation;
       // // transform.up = Vector3.Lerp(transform.up, _target.up, 0.1f*Time.deltaTime);
       // // transform.rotation = targetRotation;// * upwardsRotation;

       // 2- Then moves the pivot rotation for the pitch
       rotation = Vector3.zero;
       rotation.x = pitchAngle; //TODO: Change this so that it applies with the current gravity direction.
        //targetRotation = Quaternion.Euler(rotation);
       targetRotation = Quaternion.AngleAxis(pitchAngle, _cameraPivot.right);
       _cameraPivot.localRotation = targetRotation;*/
    }
    private void RotateCameraHalfLash()
    {
        Vector3 rotation;
        Quaternion targetRotation;
        
        yawAngle = Mathf.Lerp(yawAngle, yawAngle + (_inputManager.LookInput.x * _cameraHalflashYawSpeed), _camLookSmoothTime * Time.deltaTime);

        pitchAngle = Mathf.Lerp(pitchAngle, pitchAngle - (_inputManager.LookInput.y * _cameraHalflashPitchSpeed), _camLookSmoothTime * Time.deltaTime);

        rotation = Vector3.zero;
        rotation.y = yawAngle; //TODO: Change this so that it applies with the current gravity direction. 
        targetRotation = Quaternion.Euler(rotation);
        transform.rotation = targetRotation;

        rotation = Vector3.zero;
        rotation.x = pitchAngle; //TODO: Change this so that it applies with the current gravity direction. 
        targetRotation = Quaternion.Euler(rotation);
        _cameraPivot.localRotation = targetRotation;
    }
    private void RotateCameraLash() {
      
        //If there is no input, start timer
        if (_inputManager.LookInput.magnitude > 0) {
            Vector3 rotation;
            Quaternion targetRotation;

            yawAngle = Mathf.Lerp(yawAngle, yawAngle + (_inputManager.LookInput.x * _cameraLashYawSpeed),
                _camLookSmoothTime * Time.deltaTime);

            pitchAngle = Mathf.Lerp(pitchAngle, pitchAngle - (_inputManager.LookInput.y * _cameraLashPitchSpeed),
                _camLookSmoothTime * Time.deltaTime);

            rotation = Vector3.zero;
            rotation.y = yawAngle; //TODO: Change this so that it applies with the current gravity direction. 
            targetRotation = Quaternion.Euler(rotation);
            transform.rotation = targetRotation;

            rotation = Vector3.zero;
            rotation.x = pitchAngle; //TODO: Change this so that it applies with the current gravity direction. 
            targetRotation = Quaternion.Euler(rotation);
            _cameraPivot.localRotation = targetRotation;
            
        } else {
            
            _centerTimer += Time.deltaTime;
            if (_centerTimer >= _timeToCenter) {
                //Recenter camera Loop
                Vector3 rotation;
                Quaternion targetRotation;

                float rotationY = transform.rotation.y - _centerCameraPosition.x;
                float rotationX = _cameraPivot.localRotation.x - _centerCameraPosition.y;
                
                yawAngle = Mathf.Lerp(yawAngle, yawAngle + (rotationY * _cameraLashYawSpeed),
                    _camLookSmoothTime * Time.deltaTime);

                pitchAngle = Mathf.Lerp(pitchAngle, pitchAngle - (rotationX * _cameraLashPitchSpeed),
                    _camLookSmoothTime * Time.deltaTime);

                rotation = Vector3.zero;
                rotation = _playerStateMachine.PlayerTransform.up * yawAngle;
                targetRotation = Quaternion.Euler(rotation);
                transform.rotation = targetRotation;

                rotation = Vector3.zero;
                rotation.x = pitchAngle;
                targetRotation = Quaternion.Euler(rotation);
                _cameraPivot.localRotation = targetRotation;

                if (rotationX + rotationY <= 0.1) {
                    _centerTimer = 0; //Recentering completed, so reset timer
                }
            }
        }
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

    public void SetCameraMode(CameraMode cameraMode) {
        _cameraMode = cameraMode;

        if (cameraMode == CameraMode.Lash) {
            _camera.fieldOfView = _lashFOV;
        }
        else {
            _camera.fieldOfView = _normalFOV;
        }
    }
}

