using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;
using Vector3 = System.Numerics.Vector3;

public class CameraManager : MonoBehaviour
{
    [Header("Special Camera Settings")]
    
    
    [Header("Lash Camera Settings")]
    [SerializeField] private float _rotationSpeed = 1;
    [SerializeField] private float _lerpAmount = 1;
    [SerializeField] private bool _recenteringEnabled = true;
    [SerializeField] private float _recenteringTime = 1;
    [SerializeField] private float _recenteringWaitTime = 1f;
    [SerializeField] private float _recenteringTimer = 0;
    // Lash camera custom tracker that allows the camera to rotate around the player when lashing
    [SerializeField] private Transform _cameraTracker;
    
    [Header("References")]
    [SerializeField] private InputManager _inputManager;
    
    private void Awake()
    {
        _inputManager = FindObjectOfType<InputManager>();
    }
    
    
    public void HandleAllCameraMovement()
    {
        
        Quaternion trackerRotation = _cameraTracker.transform.rotation;
        
        if (_inputManager.LookInput != Vector2.zero)
        {
            Quaternion targetRotation = Quaternion.AngleAxis(_inputManager.LookInput.x * _rotationSpeed, _cameraTracker.up);
            targetRotation *= _cameraTracker.transform.rotation;
            _cameraTracker.transform.rotation = Quaternion.Slerp(trackerRotation, targetRotation, _lerpAmount);
            _recenteringTimer = 0;
        } 
        _recenteringTimer += Time.deltaTime;
        
        // Recenter the camera if the player has not moved the camera for a certain amount of time and the camera is not centered
        if (_recenteringEnabled && _recenteringTimer > _recenteringWaitTime && Mathf.Abs(trackerRotation.eulerAngles.y) > 0.01f) {
            Quaternion targetRotation = Quaternion.AngleAxis(- trackerRotation.eulerAngles.y, _cameraTracker.up);
            targetRotation *= _cameraTracker.transform.rotation;
            _cameraTracker.transform.rotation = Quaternion.Slerp(trackerRotation, targetRotation, _lerpAmount);
            if (trackerRotation.eulerAngles.y <= 0.01f)
                _cameraTracker.transform.rotation = Quaternion.Euler(0, 0, 0);

        }
        
        
    }
}
