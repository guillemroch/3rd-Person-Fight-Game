using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    
    //Movement variables
    [Header("Movement Variables")]
    public Vector3 moveDirection;
    
    //Speeds
    [Header("Speeds")] 
    public float movementSpeed = 7;

    public float rotationSpeed = 15;
    //References
    [Header("References")]
    public InputManager inputManager;
    public Transform cameraObject;
    public Rigidbody playerRigidbody;
    private void Awake()
    {
        inputManager = GetComponent<InputManager>();
        playerRigidbody = GetComponent<Rigidbody>();
        cameraObject = Camera.main.transform;
    }

    public void HandleAllMovement()
    {
        HandleMovement();
        HandleRotation();
    }

    private void HandleMovement()
    {
        moveDirection = cameraObject.forward * inputManager.movementInput.y + cameraObject.right * inputManager.movementInput.x;
        moveDirection.y = playerRigidbody.velocity.y;
        moveDirection.Normalize();
        moveDirection *= movementSpeed;
        
        Vector3 movementVelocity = moveDirection;
        playerRigidbody.velocity = movementVelocity;
    }

    private void HandleRotation()
    {
        Vector3 targetDirection = Vector3.zero;

        targetDirection = cameraObject.forward * inputManager.movementInput.y +
                          cameraObject.right * inputManager.movementInput.x;
        targetDirection.Normalize();
        targetDirection.y = 0;

        if (targetDirection == Vector3.zero)
            targetDirection = transform.forward;
        
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        Quaternion playerRotation =
            Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        transform.rotation = playerRotation;
    }
}
