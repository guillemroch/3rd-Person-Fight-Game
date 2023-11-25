using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class InputManager : MonoBehaviour
{
    private PlayerInputActions _playerInputs;
    private AnimatorManager _animatorManager;
    private PlayerMovement _playerMovement;

    //Input actions
    public Vector2 movementInput;
    public Vector2 cameraInput;
    public bool sprintInput;
    public bool jumpInput;
    public bool halfLashInput;
    public bool lashInput;
    
    public float moveAmount;
    private void Awake()
    {
        _animatorManager = GetComponent<AnimatorManager>();
        _playerMovement = GetComponent<PlayerMovement>();
    }

    private void OnEnable()
    {
        if (_playerInputs == null)
        {
            _playerInputs = new PlayerInputActions();

            _playerInputs.Player.Move.performed += i => movementInput = i.ReadValue<Vector2>();
            _playerInputs.Player.Look.performed += i => cameraInput = i.ReadValue<Vector2>();

            _playerInputs.Player.Sprint.performed += i => sprintInput = true;
            _playerInputs.Player.Sprint.canceled += i => sprintInput = false;
            
            _playerInputs.Player.Jump.performed += i => jumpInput = true;
            _playerInputs.Player.Jump.canceled += i => jumpInput = false;
            
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
        HandleJumpingInput();
        HandleHalfLashInput();
        HandleConfirmLashingDirectionInput();
        
        //...
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
        if (sprintInput && moveAmount > 0.5f)
        {
            _playerMovement.isSprinting = true;
        }
        else
        {
            _playerMovement.isSprinting = false;
        }
    }

    private void HandleJumpingInput()
    {
        if (jumpInput)
        {
            jumpInput = false;
            _playerMovement.TriggerJumping();
        }
    }

    private void HandleHalfLashInput()
    {
        if (halfLashInput)
        {
            halfLashInput = false;
            _playerMovement.TriggerHalfLash();
        }
    }
    
    private void HandleConfirmLashingDirectionInput()
    {
        if (lashInput)
        {
            lashInput = false;
            _playerMovement.TriggerLash();
        }
    }
    
    
}
