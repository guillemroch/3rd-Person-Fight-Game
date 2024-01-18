using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**
 * PlayerManager
 *
 * In here we have every controller or manager related to the player together.
 * It handles inputs and handle all the movement and animations responding to the correspongin Input.
 * It is also where the camera is called in the LateUpdate()
 */
public class PlayerManager : MonoBehaviour
{
    private InputManager _inputManager; //Calls Input in Update()
    private PlayerStateMachine _playerMovement; //Calls player movement calculations in FixedUpdate()
    private CameraManager _cameraManager; //Calls camera in LateUpdate()
    private Animator _animator;  //Calls and modifies animator in LateUpdate()
    
    //State of the player interaction, the animator sets the value and we store it here
    public bool isInteracting; //TODO: Should it be in _playerMovement such as isJumping and isGrounded??

    private void Awake()
    {

        _inputManager = GetComponent<InputManager>();
        _playerMovement = GetComponent<PlayerStateMachine>();
        _cameraManager = FindObjectOfType<CameraManager>();
        _animator = GetComponent<Animator>();
        
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        _inputManager.HandleAllInputs();
    }

    private void FixedUpdate()
    {
        _playerMovement.HandleAllStates();
    }

    private void LateUpdate()
    {
        _cameraManager.HandleAllCameraMovement();

        isInteracting = _animator.GetBool("isInteracting");
        _playerMovement.isJumping = _animator.GetBool("isJumping");
        _animator.SetBool("isGrounded", _playerMovement.isGrounded);
        _playerMovement.isHalfLashing = _animator.GetBool("isHalfLashing");
        _playerMovement.isLashing = _animator.GetBool("isLashing");

        
        
    }
}
