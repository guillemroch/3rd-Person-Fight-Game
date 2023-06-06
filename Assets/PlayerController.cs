using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
  

    public PlayerInputActions playerControls;
    public Rigidbody rb;
    
    //Speeds
    public float walkSpeed = 10.0f;
    public float runSpeed = 20.0f;
    public float jumpHeight = 10.0f;

    private InputAction movement;
    private InputAction abilities;

    private Vector3 moveDirection = Vector3.zero;

    private void Awake(){
        playerControls = new PlayerInputActions();
    }

    private void OnEnable()
    {
        movement = playerControls.Player.Move;
        movement.Enable();

        abilities = playerControls.Player.Abilities;
        abilities.Enable();
        abilities.performed += Ability;
    }

    private void OnDisable()
    {
        movement.Disable();
        abilities.Disable();
    }
    void Start()
    {

    }

    void Update()
    {
        moveDirection = movement.ReadValue<Vector3>();
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector3(moveDirection.x * walkSpeed, rb.velocity.y, moveDirection.z * walkSpeed);
    }

    private void Ability(InputAction.CallbackContext context)
    {
        
            Debug.Log("Ability");
        
    }
}
