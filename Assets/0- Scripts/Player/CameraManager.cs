using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using Player.StateMachine;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class CameraManager : MonoBehaviour
{
    public enum CameraMode{
        Normal, 
        HalfLash, 
        Lash,
        Infusing
    }
    
    [SerializeField] private  CameraMode _cameraMode = CameraMode.Normal;
    
    [Header("References")]
    [SerializeField] private Transform _target; //Object to look at
    [SerializeField] private Transform _cameraPivot; //Object from where the camera rotates
    private InputManager _inputManager;
    private Transform _cameraTransform; //Transform of the real camera
    private Camera _camera;
    private PlayerStateMachine _playerStateMachine;
    private float _defaultPosition; // Where the camera goes when there are no collisions
    private Vector3 _cameraFollowVelocity = Vector3.zero;
    private Vector3 _cameraVectorPosition;
    [SerializeField] private float _pitchRotation; 
    
    
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
    [Space(10)]
    
    [Header("Camera Modes and Settings")]
    [SerializeField] private float _cameraRotationLerp = 0.7f;

    [SerializeField] private bool _invertedPitch;
    [SerializeField] private bool _invertedYaw;
    
    [Header("Normal")]
    [SerializeField] private Vector3 _targetOffset;
    [SerializeField] private float _cameraFollowSpeed = 0.1f;
    [SerializeField] private float _cameraPitchSpeed = 15;
    [SerializeField] private float _cameraYawSpeed = 15;
    
    [SerializeField] private float _sensitivityMultiplier = 1;
    
    [SerializeField] private int _normalFOV = 70;
    
    [Header("Limits")]
    [SerializeField] private float _minimumPitchAngle = -35;
    [SerializeField] private float _maximumPitchAngle = 35;
    
    
    [Header("Halflash")]
    [SerializeField] private Vector3 _cameraHalflashOffset;
    [SerializeField] private float _cameraHalflashPitchSpeed = 25;
    [SerializeField] private float _cameraHalflashYawSpeed = 25;

    [SerializeField] private float _sensitivityHalflashMultiplier = 1;
    
    [Header("Lash")] 
    [SerializeField] private Vector3 _cameraLashOffset;
    [SerializeField] private float _cameraLashFollowSpeed = 0.1f;
    [SerializeField] private float _cameraLashPitchSpeed = 15;
    [SerializeField] private float _cameraLashYawSpeed = 15;
    
    [SerializeField] private float _sensitivityLashMultiplier = 1;
    
    [SerializeField] private int _lashFOV = 90;

    //Centering
    [SerializeField] private float _centerTimer = 0f;
    [SerializeField] private float _timeToCenter = 2f;
    [SerializeField] private Vector2 _centerCameraPosition;
    
    [Header("Infusing")]
    [SerializeField] private Vector3 _cameraInfusingOffset;

    
    public float PitchRotation { get => _pitchRotation; set => _pitchRotation = value; }

    public void Awake()
    {
        _inputManager = FindObjectOfType<InputManager>();
        _camera = Camera.main;
        _cameraTransform = _camera!.transform;
        _defaultPosition = _cameraTransform.localPosition.z;
        _playerStateMachine = FindObjectOfType<PlayerStateMachine>();
    }

    public void HandleAllCameraMovement() {
        
        //Calculate the actual pitch to use the 
        _pitchRotation = _cameraPivot.localRotation.eulerAngles.x;
        if (_pitchRotation > 180f) _pitchRotation -= 360f;
        _pitchRotation /= _maximumPitchAngle;
        
        switch (_cameraMode) {
            case CameraMode.Normal:
                FollowTarget();
                RotateCamera();
                HandleCameraCollisions();
                break;
            case CameraMode.HalfLash:
                FollowTargetHalfLash();
                RotateCameraHalfLash();
                HandleCameraCollisions();
                break;
            case CameraMode.Lash:
                FollowTargetLash();
                RotateCameraLash();
                HandleCameraCollisions();
                break;
            case CameraMode.Infusing:
                FollowTargetInfusing();
                RotateCamera();
                HandleCameraCollisions();
                break;
            default:
                Debug.LogError("No camera mode defined!");
                break;
        }
    }

    private void FollowTarget() {
        Vector3 offsetTargetPosition = _targetOffset.x * transform.right + _targetOffset.y * transform.up +
                                       _targetOffset.z * transform.forward;
        Vector3 lookAtPosition = 
            Vector3.SmoothDamp(transform.position, _target.position + offsetTargetPosition, ref _cameraFollowVelocity, _cameraFollowSpeed);
        transform.position = lookAtPosition;
    }
    private void FollowTargetHalfLash()
    {
         Vector3 offsetTargetPosition = _cameraHalflashOffset.x * transform.right + _cameraHalflashOffset.y * transform.up +
                                               _cameraHalflashOffset.z * transform.forward;
        Vector3 lookAtPosition = 
            Vector3.SmoothDamp(transform.position, _target.position + offsetTargetPosition, ref _cameraFollowVelocity, _cameraFollowSpeed);
        transform.position = lookAtPosition;
    }
    
    private void FollowTargetLash()
    {
        Vector3 offsetedTargetPosition = _target.position + _target.up * (_cameraLashOffset.y * _playerStateMachine.PlayerRigidbody.velocity.magnitude)
            + _cameraLashOffset.x * transform.right + _cameraLashOffset.z * transform.forward;
        Vector3 lookAtPosition = 
            Vector3.SmoothDamp(transform.position, offsetedTargetPosition, ref _cameraFollowVelocity, _cameraLashFollowSpeed);
        transform.position = lookAtPosition;
    }
    private void FollowTargetInfusing() {
        Vector3 offsetTargetPosition = _cameraInfusingOffset.x * _target.right + _cameraInfusingOffset.y * transform.up +
                                       _cameraInfusingOffset.z * transform.forward;
        Vector3 lookAtPosition = 
            Vector3.SmoothDamp(transform.position, _target.position + offsetTargetPosition, ref _cameraFollowVelocity, _cameraFollowSpeed);
        transform.position = lookAtPosition;
    }
    
    
    private void RotateCamera()
    {
        //1- Calculate user input
        float yawAngle = _inputManager.LookInput.x * _cameraYawSpeed * _sensitivityMultiplier;
        yawAngle *= _invertedYaw ? -1 : 1;
        float pitchAngle =  _inputManager.LookInput.y * _cameraPitchSpeed * _sensitivityMultiplier;
        pitchAngle *= _invertedPitch ? -1 : 1;
        
        
        //Limit pitch angle
        float currentPitchAngle = _cameraPivot.localRotation.eulerAngles.x;
        if (currentPitchAngle > 180f) currentPitchAngle -= 360f; // Convert to signed angle
        if (pitchAngle > 0) {
            if (currentPitchAngle + pitchAngle > _maximumPitchAngle) {
                pitchAngle = 0;
            }
        }
        else {
            if (currentPitchAngle + pitchAngle < _minimumPitchAngle) {
                pitchAngle = 0;
            }
        } 
        
        Vector3 rotation = Vector3.zero;
        
        //2 - Apply yaw rotation to transform + Up Vector Correction
        rotation.y = yawAngle;
        Quaternion targetRotation = Quaternion.Euler(rotation);
        //targetRotation = Quaternion.AngleAxis(yawAngle, transform.up);
        //Quaternion upAlignRotation = Quaternion.identity;
        //Quaternion upAlignRotation = Quaternion.FromToRotation(transform.up, _target.up);
        //Quaternion upAlignRotation = Quaternion.RotateTowards(transform.rotation, _target.rotation, 30f);
        //Quaternion upAlignRotation = Quaternion.LookRotation(transform.forward, _target.up);
        //TODO: Make sure the rotation is correct cause it is not
        //transform.rotation = Quaternion.Slerp(transform.rotation, transform.rotation * upAlignRotation * targetRotation, _cameraRotationLerp*Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation,transform.rotation * targetRotation , _cameraRotationLerp*Time.deltaTime);
        //3 - Rotate the pivot point for the pitch
        rotation = Vector3.zero;
        rotation.x = pitchAngle;
        targetRotation = Quaternion.Euler(rotation);
        _cameraPivot.localRotation = Quaternion.Slerp(_cameraPivot.localRotation, _cameraPivot.localRotation * targetRotation, _cameraRotationLerp*Time.deltaTime);
        
    }
    private void RotateCameraHalfLash()
    {
        
        Vector3 rotation = Vector3.zero;
        
        float yawAngle = _inputManager.LookInput.x * _cameraHalflashYawSpeed * _sensitivityHalflashMultiplier;
        yawAngle *= _invertedYaw ? -1 : 1;
        float pitchAngle =  _inputManager.LookInput.y * _cameraHalflashPitchSpeed * _sensitivityHalflashMultiplier;
        pitchAngle *= _invertedPitch ? -1 : 1;
        
        rotation.y = yawAngle;
        Quaternion targetRotation = Quaternion.Euler(rotation);

        transform.rotation = Quaternion.Slerp(transform.rotation, transform.rotation * targetRotation, _cameraRotationLerp*Time.deltaTime);

        rotation = Vector3.zero;
        rotation.x = pitchAngle; 
        targetRotation = Quaternion.Euler(rotation);
        _cameraPivot.localRotation = Quaternion.Slerp(_cameraPivot.localRotation, _cameraPivot.localRotation * targetRotation, _cameraRotationLerp*Time.deltaTime);
    }
    private void RotateCameraLash() {
        //The player controls the camera Yaw and pitch, the Roll is automatically done
            Quaternion targetRotation;

            float yawAngle = _inputManager.LookInput.x * _cameraLashYawSpeed * _sensitivityLashMultiplier;
            yawAngle *= _invertedYaw ? -1 : 1;
            float pitchAngle = _inputManager.LookInput.y * _cameraLashPitchSpeed * _sensitivityLashMultiplier;
            pitchAngle *= _invertedPitch ? -1 : 1;
            
            
            targetRotation = Quaternion.AngleAxis(yawAngle, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, transform.rotation * targetRotation , _cameraRotationLerp * Time.deltaTime);

            targetRotation = Quaternion.AngleAxis(pitchAngle, Vector3.right);
            _cameraPivot.localRotation = Quaternion.Slerp(_cameraPivot.localRotation, _cameraPivot.localRotation * targetRotation, _cameraRotationLerp * Time.deltaTime);
        
        /*if (_inputManager.LookInput.magnitude > 0) {
            _centerTimer = 0;
            Quaternion targetRotation;

            float yawAngle = _inputManager.LookInput.x * _cameraLashYawSpeed * _sensitivityLashMultiplier;
            yawAngle *= _invertedYaw ? -1 : 1;
            float pitchAngle = _inputManager.LookInput.y * _cameraLashPitchSpeed * _sensitivityLashMultiplier;
            pitchAngle *= _invertedPitch ? -1 : 1;
            
            
            targetRotation = Quaternion.AngleAxis(yawAngle, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, transform.rotation * targetRotation , _cameraRotationLerp * Time.deltaTime);

            targetRotation = Quaternion.AngleAxis(pitchAngle, Vector3.right);
            _cameraPivot.localRotation = Quaternion.Slerp(_cameraPivot.localRotation, _cameraPivot.localRotation * targetRotation, _cameraRotationLerp * Time.deltaTime);

        } else {
            
            if (_centerTimer > _timeToCenter) {
                
                Quaternion targetRotation = _target.rotation;
                targetRotation *= Quaternion.AngleAxis(90, _target.right);
                Quaternion centerPitch = Quaternion.AngleAxis(-_centerCameraPosition.y, Vector3.right);
                 
                transform.rotation = Quaternion.Slerp(transform.rotation,targetRotation, _cameraRotationLerp * Time.deltaTime);
                _cameraPivot.localRotation =  Quaternion.Slerp(_cameraPivot.localRotation,centerPitch, _cameraRotationLerp * Time.deltaTime);
                 
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
        } */
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
        if (cameraMode == CameraMode.Normal && _cameraMode != CameraMode.Normal) { //Switching TO Normal mode
            //StartCoroutine(TransitionRotationToNormal(1));
        }
        if (cameraMode != CameraMode.Normal) { //Switching to any other state that is not Normal mode
            //StartCoroutine(TransitionRotationToLash(1));
        }
        _cameraMode = cameraMode;
        //StartCoroutine(Transition(2));
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
    
       public IEnumerator Transition(float duration) {
           _cameraRotationLerp = 1;
           while (duration >= 0)
           { 
               duration -= Time.deltaTime;
               yield return null;
           }

           _cameraRotationLerp = 15;
       }

       public IEnumerator TransitionRotationToLash(float duration) {
           Vector3 rotation = _target.rotation.eulerAngles;
           rotation.x = 0;
           rotation.z = 0;
           Quaternion targetRotation = Quaternion.Euler(rotation);
           Quaternion targetPivotRotation = Quaternion.Euler(new Vector3(0,0,0));
           while (duration >= 0) {
               duration -= Time.deltaTime;        
               
               _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, _lashFOV, 0.8f);

               transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 1 * Time.deltaTime);
               _cameraPivot.localRotation =
                   Quaternion.Slerp(_cameraPivot.localRotation, targetPivotRotation, 1 * Time.deltaTime);
           }
         
           yield return null;  
       }

       public IEnumerator TransitionRotationToNormal(float duration) {
           Quaternion targetRotation = Quaternion.FromToRotation(transform.up, -_playerStateMachine.GravityDirection);
           Quaternion targetPivotRotation = Quaternion.Euler(new Vector3(0,0,0));
           while (duration >= 0) {
               duration -= Time.deltaTime;        
                 
               _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, _normalFOV, 0.8f);
               transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 4 * Time.deltaTime);
               _cameraPivot.localRotation =
                   Quaternion.Slerp(_cameraPivot.localRotation, targetPivotRotation, 4 * Time.deltaTime);
           }

           yield return null;
       }
       
}

