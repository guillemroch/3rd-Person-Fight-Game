using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class InputManager : MonoBehaviour
{
    private PlayerInputActions playerInputs;
    private AnimatorManager animatorManager;
    private PlayerMovement playerMovement;

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
        animatorManager = GetComponent<AnimatorManager>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void OnEnable()
    {
        if (playerInputs == null)
        {
            playerInputs = new PlayerInputActions();

            playerInputs.Player.Move.performed += i => movementInput = i.ReadValue<Vector2>();
            playerInputs.Player.Look.performed += i => cameraInput = i.ReadValue<Vector2>();

            playerInputs.Player.Sprint.performed += i => sprintInput = true;
            playerInputs.Player.Sprint.canceled += i => sprintInput = false;
            
            playerInputs.Player.Jump.performed += i => jumpInput = true;
            playerInputs.Player.Jump.canceled += i => jumpInput = false;
            
            playerInputs.Player.HalfLash.performed += i => halfLashInput = true;
            playerInputs.Player.HalfLash.canceled += i => halfLashInput = false;
            
            playerInputs.Player.ConfirmLash.performed += i => lashInput = true;
            playerInputs.Player.ConfirmLash.canceled += i => lashInput = false;

        }
        
        playerInputs.Enable();
    }

    private void OnDisable()
    {
        playerInputs.Disable();
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
        movementInput = movementInput;
        moveAmount = Mathf.Clamp01(Mathf.Abs(movementInput.x) + Mathf.Abs(movementInput.y));
        animatorManager.UpdateAnimatorValues(new Vector2(0, moveAmount), playerMovement.isSprinting);
    }

    private void HandleCameraInput()
    {
        
    }

    private void HandleSprintingInput()
    {
        if (sprintInput && moveAmount > 0.5f)
        {
            playerMovement.isSprinting = true;
        }
        else
        {
            playerMovement.isSprinting = false;
        }
    }

    private void HandleJumpingInput()
    {
        if (jumpInput)
        {
            jumpInput = false;
            playerMovement.TriggerJumping();
        }
    }

    private void HandleHalfLashInput()
    {
        if (halfLashInput)
        {
            halfLashInput = false;
            playerMovement.TriggerHalfLash();
        }
    }
    
    private void HandleConfirmLashingDirectionInput()
    {
        if (lashInput)
        {
            lashInput = false;
            playerMovement.TriggerLash();
        }
    }
    
    
}
