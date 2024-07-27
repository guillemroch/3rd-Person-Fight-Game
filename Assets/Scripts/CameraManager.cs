using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Player;
using Player.StateMachine;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

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
    [SerializeField] private Quaternion localTransformRotation;    
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
    private float _minimumPitchAngle = -35;
    [SerializeField]
    private float _maximumPitchAngle = 35;

    
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
    [SerializeField] private float _cameraLashSmoothLerp = 0.7f;

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
        yawAngle = Mathf.Lerp(yawAngle, yawAngle + (_inputManager.LookInput.x * _cameraYawSpeed), _camLookSmoothTime * Time.deltaTime);
        yawAngle = _inputManager.LookInput.x * _cameraYawSpeed;
        pitchAngle = Mathf.Lerp(pitchAngle, pitchAngle - (_inputManager.LookInput.y * _cameraPitchSpeed), _camLookSmoothTime * Time.deltaTime);
        //Limit pitch angle
        pitchAngle = Mathf.Clamp(pitchAngle, _minimumPitchAngle*Mathf.Deg2Rad, _maximumPitchAngle*Mathf.Deg2Rad);
        Vector3 rotation = Vector3.zero;
        
        //2 - Apply yaw rotation to transform + Up Vector Correction
        rotation.y = yawAngle;
        Quaternion targetRotation = Quaternion.Euler(rotation);
        Quaternion upAlignRotation = Quaternion.LookRotation(transform.forward, _target.up);
        //TODO: Make sure the rotation is correct cause it is not
        transform.rotation = Quaternion.Slerp(transform.rotation, upAlignRotation * targetRotation, 0.9f);
        //3 - Rotate the pivot point for the pitch
        rotation = Vector3.zero;
        rotation.x = pitchAngle;
        targetRotation = quaternion.Euler(rotation);
        _cameraPivot.localRotation = targetRotation;
        
    }
    private void RotateCameraHalfLash()
    {
        
        Vector3 rotation;
        Quaternion targetRotation;
        
        yawAngle = Mathf.Lerp(yawAngle, yawAngle + (_inputManager.LookInput.x * _cameraHalflashYawSpeed), _camLookSmoothTime * Time.deltaTime);

        pitchAngle = Mathf.Lerp(pitchAngle, pitchAngle - (_inputManager.LookInput.y * _cameraHalflashPitchSpeed), _camLookSmoothTime * Time.deltaTime);

        rotation = Vector3.zero;
        rotation.y = yawAngle; 
        targetRotation = Quaternion.Euler(rotation);
        transform.rotation = targetRotation;

        rotation = Vector3.zero;
        rotation.x = pitchAngle; 
        targetRotation = Quaternion.Euler(rotation);
        _cameraPivot.localRotation = targetRotation;
    }
    private void RotateCameraLash() {
        //The player controls the camera Yaw and pitch, the Roll is automatically done
        //Quaternion rollCorrection = Quaternion.FromToRotation(transform.up, _target.forward);
        //rollCorrection = Quaternion.AngleAxis(Vector3.Angle(transform.up, -_target.forward), _target.up);
        
        if (_inputManager.LookInput.magnitude > 0) {
            _centerTimer = 0;
            Quaternion targetRotation;

            yawAngle = _inputManager.LookInput.x * _cameraLashYawSpeed;
            pitchAngle = _inputManager.LookInput.y * _cameraLashPitchSpeed;
            targetRotation = Quaternion.AngleAxis(yawAngle, transform.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, transform.rotation * targetRotation , _cameraLashSmoothLerp * Time.deltaTime);

            targetRotation = Quaternion.AngleAxis(pitchAngle, Vector3.right);
            _cameraPivot.localRotation = Quaternion.Slerp(_cameraPivot.localRotation, _cameraPivot.localRotation * targetRotation, _cameraLashSmoothLerp * Time.deltaTime);

        } else {
            
            if (_centerTimer > _timeToCenter) {
                
                Quaternion targetRotation = _target.rotation;
                Quaternion centerPitch = Quaternion.AngleAxis(-_centerCameraPosition.y, Vector2.right);
                 
                transform.rotation = Quaternion.Slerp(transform.rotation,targetRotation, _cameraLashSmoothLerp * Time.deltaTime);
                _cameraPivot.localRotation =  Quaternion.Slerp(_cameraPivot.localRotation,centerPitch, _cameraLashSmoothLerp * Time.deltaTime);
                 
                float angleYaw = Quaternion.Angle(transform.rotation, _target.rotation);
                float anglePitch = Quaternion.Angle(_cameraPivot.localRotation, centerPitch);
                float diff = angleYaw + anglePitch;
                
                if (diff < 0.1) {
                    _centerTimer = 0;
                }                  
            }
            else {
                _centerTimer += Time.deltaTime;
                float rollAngleDiff = Vector3.Angle(transform.up, -_target.forward);
                if (rollAngleDiff > 0.1f) {
                    //transform.up = -_target.forward;
                    //transform.rotation *= rollCorrection;
                    //transform.rotation = Quaternion.Slerp(transform.rotation, transform.rotation * rollCorrection, 0.9f * Time.deltaTime);
                    //Debug.Log("Roll Correction: " + rollAngleDiff);     
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
            //_camera.fieldOfView = _lashFOV;
        }
        else {
            //_camera.fieldOfView = _normalFOV;
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(_target.position, 0.10f);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(_target.position, _target.position + _target.right * 0.2f);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(_target.position, _target.position + _target.up * 0.2f);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(_target.position, _target.position + _target.forward * 0.2f);
        
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 0.25f);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.right * 0.2f);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + transform.up * 2f);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 0.2f);
        
        //Offsetting the pivot for better visual clarity

        Vector3 pivotPosition = _cameraPivot.position - _cameraPivot.forward * 0.5f;
        
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(pivotPosition, 0.05f);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(pivotPosition, pivotPosition + _cameraPivot.right * 0.2f);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(pivotPosition,pivotPosition + _cameraPivot.up * 0.2f);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(pivotPosition, pivotPosition + _cameraPivot.forward * 0.2f);
    }
}

