using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private InputManager _inputManager;
    private PlayerMovement _playerMovement;
    private CameraManager _cameraManager;

    private void Awake()
    {
        _inputManager = GetComponent<InputManager>();
        _playerMovement = GetComponent<PlayerMovement>();
        _cameraManager = FindObjectOfType<CameraManager>();
    }

    private void Update()
    {
        _inputManager.HandleAllInputs();
    }

    private void FixedUpdate()
    {
        _playerMovement.HandleAllMovement();
    }

    private void LateUpdate()
    {
        _cameraManager.HandleAllCameraMovement();
    }
}
