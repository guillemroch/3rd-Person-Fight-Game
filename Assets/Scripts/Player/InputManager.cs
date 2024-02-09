using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;
using UnityEngine.Serialization;

public class InputManager : MonoBehaviour
{
    private PlayerInputActions _playerInputs;
    private AnimatorManager _animatorManager;
    private PlayerStateMachine _playerMovement;

    //Input actions
    public Vector2 movementInput;
    public Vector2 cameraInput;
    public bool isSprintPressed;
    public bool isJumpPressed;
    public bool halfLashInput;

  
    public bool lashInput;
    
    public float moveAmount;
    
    // getters and setters
    public bool IsJumpPressed { get => isJumpPressed; set => isJumpPressed = value; }
    public bool IsSprintPressed { get => isSprintPressed; set => isSprintPressed = value; }
    public bool HalfLashInput { get => halfLashInput; set => halfLashInput = value; }

    public bool LashInput { get => lashInput; set => lashInput = value; }

    private void Awake()
    {
        _animatorManager = GetComponent<AnimatorManager>();
        _playerMovement = GetComponent<PlayerStateMachine>();
    }

    private void OnEnable()
    {
        if (_playerInputs == null)
        {
            _playerInputs = new PlayerInputActions();

            _playerInputs.Player.Move.performed += i => movementInput = i.ReadValue<Vector2>();
            _playerInputs.Player.Look.performed += i => cameraInput = i.ReadValue<Vector2>();

            _playerInputs.Player.Sprint.performed += i => isSprintPressed = true;
            _playerInputs.Player.Sprint.canceled += i => isSprintPressed = false;
            
            _playerInputs.Player.Jump.performed += i => isJumpPressed = true;
            _playerInputs.Player.Jump.canceled += i => isJumpPressed = false;
            
            _playerInputs.Player.HalfLash.performed += i => halfLashInput = true;
            _playerInputs.Player.HalfLash.canceled += i => halfLashInput = false;
            
            _playerInputs.Player.ConfirmLash.performed += i => lashInput = true;
            _playerInputs.Player.ConfirmLash.canceled += i => lashInput = false;

        }
        
        _playerInputs.Enable();
    }

    private void OnDisable()
    {
        _playerInputs.Disable();
    }

    public void HandleAllInputs()
    {
        HandleMovementInput();
        HandleCameraInput();
        HandleSprintingInput();
    
    }

    private void HandleMovementInput()
    {
        moveAmount = Mathf.Clamp01(Mathf.Abs(movementInput.x) + Mathf.Abs(movementInput.y));
        _animatorManager.UpdateAnimatorValues(new Vector2(0, moveAmount), _playerMovement.isSprinting);
    }

    private void HandleCameraInput()
    {
        
    }

    private void HandleSprintingInput()
    {
        if (isSprintPressed && moveAmount > 0.5f)
        {
            _playerMovement.isSprinting = true;
        }
        else
        {
            _playerMovement.isSprinting = false;
        }
    }

 
    
    
}
