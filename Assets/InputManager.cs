using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private PlayerInputActions playerInputs;
    private AnimatorManager animatorManager;

    //Input actions
    public Vector2 movementInput;
    private float moveAmount;
    private void Awake()
    {
        animatorManager = GetComponent<AnimatorManager>();
    }

    private void OnEnable()
    {
        if (playerInputs == null)
        {
            playerInputs = new PlayerInputActions();

            playerInputs.Player.Move.performed += i => movementInput = i.ReadValue<Vector2>();
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
        //...
    }

    private void HandleMovementInput()
    {
        movementInput = movementInput;
        moveAmount = Mathf.Clamp01(Mathf.Abs(movementInput.x) + Mathf.Abs(movementInput.y));
        animatorManager.UpdateAnimatorValues(new Vector2(0, moveAmount));
    }
}
