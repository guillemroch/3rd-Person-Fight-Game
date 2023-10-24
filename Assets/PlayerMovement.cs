using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;
/**
 * PlayerMovement:
 * Calculates player movement
 */

public class PlayerMovement : MonoBehaviour
{
    //Player values for movements
    [Header("Movement Variables")]
    public Vector3 moveDirection;

    public Vector3 targetDirection;

    //Movement status flags
    [Header("Movement Flags")] 
    public bool isSprinting;
    public bool isGrounded;
    public bool isJumping;
    public bool isHalfLashing;

    
    //Falling and ground detection variables
    [Header("Falling")] 
    public float inAirTimer;
    public float leapingVelocity;
    public float fallingVelocity;
    public LayerMask groundLayer;
    public float rayCastHeightOffset = 0.5f;
    public float rayCastMaxDistance = 1;

    [Header("Gravity")] 
    public Vector3 gravityDirection = Vector3.down; //What is the current gravity orientation for the player
    public float gravityIntensity = 9.8f;
    public float gravityMultiplier = 2;
    public float groundedGravity = 0.5f;

    [Header("Lashings")] 
    public float halfLashingHeight = 1.0f;
    
    //Speeds
    [Header("Speeds")] 
    public float movementSpeed ;
    public float walkingSpeed = 1.5f;
    public float runningSpeed = 6f;
    public float sprintingSpeed = 8f;
    public float rotationSpeed = 15;
    
    //Jump
    [Header("Jump Speeds")] 
    public float jumpHeight = 3;

    //References
    [Header("References")] 
    public PlayerManager playerManager;
    public AnimatorManager animatorManager;
    public InputManager inputManager;
    public Transform cameraObject;
    public Rigidbody playerRigidbody;

    
    //Gizmos
    [Header("=====================")]
    [Header("### DEBUG SECTION ###")]  [Header("Gizmos")]
    public bool enabledGizmos;
    
    [Header("Local Player Transform")] 
    public bool isTransformGizmoEnabled;
    public Vector3 centerOfMassGizmo;
    public Color upGizmoColor = Color.green;
    public Color rightGizmoColor = Color.red;
    public Color frontGizmoColor = Color.blue;

    [Header("Ground Detection")] 
    public bool isGroundDetectionGizmoEnabled;
    
    public Color originGizmoColor = Color.magenta;
    public Vector3 gizmoRayCastOrigin;
    public Color hitGizmoColor = Color.green;
    public Color rayGizmoColor = Color.yellow;
    
    
    [Header("Forces Applied and Movement")]
    [Header("Gravity Forces")]
    public bool isGravityDirectionGizmoEnabled;
    public Color gravityGizmoColor = Color.cyan;
    public Vector3 gravityGizmoForceVector;
    
    [Header("Movement Forces")]
    public bool isVelocityGizmoEnabled;
    public Color movementGizmoColor = Color.red;
    public Vector3 movementGizmoForceVector;
    
    [Header("Falling Forces")]
    public bool isFallingForcesGizmoEnabled;
    public Color fallingForcesGizmoColor = Color.blue;
    public Vector3 fallingForcesGizmoVector;

    [Header("Jumping Forces")]
    public bool isJumpingForcesGizmoEnabled;
    public Color jumpingForcesGizmoColor = Color.green;
    public Vector3 jumpingForcesGizmoVector;

    [Header("Lashing Forces")]
    public bool isLashingForcesGizmoEnabled;
    public Color lashingForcesGizmoColor = Color.magenta;
    public Vector3 lashingForcesGizmoVector;

    [Header("TOTAL FORCES")] 
    public bool isTotalForcesGizmoEnabled;
    public Color totalForcesGizmoColor = Color.red;
    public Vector3 totalForcesGizmoVector;

    [Header("Real Velocity")] public Vector3 realVelocity;
    [Header("Targeted Direction")] 
    public bool isTargetedDirectionGizmoEnabled;
    public Vector3 targetedDirectionGizmo;
    public Color targetDirectionGizmoColor = Color.yellow;

    
   
    //------------ METHODS --------------------------------------------------------------------
    
    /**
     * Gets all the needed references in Awake()
     */
    private void Awake()
    {
        
        //Get references
        playerManager = GetComponent<PlayerManager>();
        animatorManager = GetComponent<AnimatorManager>();
        inputManager = GetComponent<InputManager>();
        playerRigidbody = GetComponent<Rigidbody>();
        cameraObject = Camera.main.transform;
    }

    /**
     * Method called in the PlayerManager
     * Handles all of the movement function
     */
    public void HandleAllMovement()
    {
#if UNITY_EDITOR
        

        gravityGizmoForceVector = Vector3.zero;
        movementGizmoForceVector = Vector3.zero;
        fallingForcesGizmoVector = Vector3.zero;
        jumpingForcesGizmoVector = Vector3.zero;
        lashingForcesGizmoVector = Vector3.zero;
        totalForcesGizmoVector = Vector3.zero;
        realVelocity = playerRigidbody.velocity;
        targetedDirectionGizmo = Vector3.zero;
        
#endif
        
        //First Handle the Falling
        HandleFallingAndLanding();
        
        if (playerManager.isInteracting)
            return;
        
        
        //If the player is interacting or jumping we do not want him to move
        HandleMovement();
        HandleRotation();
        HandleGravity();
    }

    
    /**
     * Handles the movement vector, this is the speed and direction
     */
    private void HandleMovement()
    {
        
        moveDirection = cameraObject.forward * inputManager.movementInput.y + cameraObject.right * inputManager.movementInput.x;
        moveDirection.y = 0;
        moveDirection.Normalize();

        if (isSprinting)
        {
            movementSpeed = sprintingSpeed;
        }
        else
        {
            if (inputManager.moveAmount >= 0.5f)
            {
                movementSpeed = runningSpeed;
            }
            else
            {
                movementSpeed = walkingSpeed;
            }
        }
        
        //moveDirection *= movementSpeed;

        
        //Vector3 movementVelocity = moveDirection;
        //playerRigidbody.velocity = movementVelocity;
        movementGizmoForceVector = moveDirection * movementSpeed;
        playerRigidbody.AddForce(moveDirection * movementSpeed, ForceMode.Force);
        totalForcesGizmoVector += movementGizmoForceVector;
    }

    /**
     * Handles the player rotation
     */
    private void HandleRotation()
    {
        
        targetDirection = Vector3.zero; //Resets target direction

       if (!isHalfLashing)
       {
            //calculate orientation based on camera position
            targetDirection = cameraObject.forward * inputManager.movementInput.y +
                              cameraObject.right * inputManager.movementInput.x;
            targetDirection.Normalize();
            targetDirection.y = 0;

            //Debug.Log(inputManager.cameraInput.x + "," + inputManager.cameraInput.y);
            if (targetDirection == Vector3.zero)
                targetDirection = transform.forward;

            targetedDirectionGizmo = targetDirection;

            Quaternion targetRotation = Quaternion.LookRotation(targetDirection, transform.up);

            transform.rotation =  Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            return;
       }

       if (isHalfLashing)
       {
           //Make the player rotate with the camera, making that we always see the back of the player
           
           transform.RotateAround(playerRigidbody.worldCenterOfMass, transform.up, rotationSpeed * Time.deltaTime * inputManager.cameraInput.x);
           transform.RotateAround(playerRigidbody.worldCenterOfMass, transform.right, rotationSpeed * Time.deltaTime * -inputManager.cameraInput.y);

       }
       
    }
    

    
    /**
     * Handles falling and landing
     */
    private void HandleFallingAndLanding()
    {
        if (isHalfLashing) return;
        RaycastHit hit;
        Vector3 rayCastOrigin = transform.position - rayCastHeightOffset * gravityDirection;
        gizmoRayCastOrigin = rayCastOrigin;

        if (!isGrounded && !isJumping)
        {
            if (!playerManager.isInteracting)
            {
                animatorManager.PlayTargetAnimation("Fall", true);
            }

            inAirTimer += Time.deltaTime;
            playerRigidbody.AddForce(transform.forward * (leapingVelocity * playerRigidbody.velocity.magnitude), ForceMode.Force);
            playerRigidbody.AddForce(gravityDirection * (fallingVelocity * inAirTimer), ForceMode.Force);
            
            //Gizmos
            fallingForcesGizmoVector =
                transform.forward * leapingVelocity + gravityDirection * (fallingVelocity * inAirTimer);
            totalForcesGizmoVector += fallingForcesGizmoVector;
        }
        
        if (Physics.SphereCast(rayCastOrigin, 0.2f, gravityDirection, out hit ,rayCastMaxDistance, groundLayer))
        {
            if (!isGrounded && !playerManager.isInteracting)
            {
                animatorManager.PlayTargetAnimation("Land", true);
            }

            inAirTimer = 0;
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    /**
     * Handles Jumping
     */
    public void HandleJumping()
    {
        if (isGrounded)
        {
            animatorManager.animator.SetBool("isJumping", true);
            animatorManager.PlayTargetAnimation("Jump", false);

            float jumpingVelocity = Mathf.Sqrt(2 * gravityIntensity  *  jumpHeight);
            playerRigidbody.AddForce(jumpingVelocity * -gravityDirection, ForceMode.Impulse);
            jumpingForcesGizmoVector = jumpingVelocity * -gravityDirection;
            totalForcesGizmoVector += jumpingForcesGizmoVector;
        }
    }

    public void HandleHalfLash()
    {
        if (isHalfLashing) return;
        
        animatorManager.animator.SetBool("isHalfLashing", true);
        animatorManager.PlayTargetAnimation("Half Lashing", false);

        
        playerRigidbody.AddForce(halfLashingHeight * -gravityDirection, ForceMode.Impulse);
        lashingForcesGizmoVector = halfLashingHeight * -gravityDirection;
        totalForcesGizmoVector += lashingForcesGizmoVector;
        
        //Debug.Log("Velocity = " + playerRigidbody.velocity);

        
        inAirTimer = 0;
    }
    
    public void HandleLash() 
    {
        gravityDirection = cameraObject.forward;

        isHalfLashing = false; //TODO: Change this to a state machine
        animatorManager.animator.SetBool("isHalfLashing", false);
        
        
    }
    

    public void HandleGravity()
    {
        gravityGizmoForceVector = Vector3.zero;
        if (isJumping) return;
        if (isHalfLashing)
        {
            return;
        } 
        if (isGrounded)
        {
            //playerRigidbody.velocity += groundedGravity * gravityMultiplier * transform.up;
            playerRigidbody.AddForce(groundedGravity * gravityMultiplier * gravityDirection, ForceMode.Acceleration);
            gravityGizmoForceVector = groundedGravity * gravityMultiplier * gravityDirection;
        }
        else
        {
            playerRigidbody.AddForce(gravityIntensity * gravityMultiplier * gravityDirection, ForceMode.Acceleration);
            gravityGizmoForceVector = gravityIntensity * gravityMultiplier * gravityDirection;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        //checks if the current layer of the game object is included in the groundLayer LayerMask
        if ((groundLayer.value & (1 << other.gameObject.layer))!= 0)
        {
            //Debug.Log("Player has hit the ground");
            isGrounded = true;
            /*Quaternion.Slerp(transform.rotation,
                Quaternion.FromToRotation(transform.up, other.contacts[0].normal) * transform.rotation, 0.1f);*/
            transform.up = other.contacts[0].normal; //TODO: Make it so that the player rotates slowly to the normal of the ground
            gravityDirection = - other.contacts[0].normal;
        }
    }


#if UNITY_EDITOR
    
    /**
     * Debug Gizmos
     * Cyan Ray: Ray
     * Magenta cube: rayCastOrigin
     * Green Sphere: Raycast hit
     */
    private void OnDrawGizmos()
    {

        var playerTransform = transform;
        var posOffset = new Vector3(0f, 1f, 0f);
        var position = playerTransform.position;
        
        if (!enabledGizmos) return;

        if (isTransformGizmoEnabled)
        {
            Gizmos.color = upGizmoColor;
            centerOfMassGizmo = playerRigidbody.worldCenterOfMass;
            Gizmos.DrawSphere(centerOfMassGizmo, 0.05f);
            Gizmos.DrawRay(position, playerTransform.up);
            Gizmos.color = frontGizmoColor;
            Gizmos.DrawRay(position, playerTransform.forward);
            Gizmos.color = rightGizmoColor;
            Gizmos.DrawRay(position, playerTransform.right);
        }

        if (isGroundDetectionGizmoEnabled)
        {
            Gizmos.color = rayGizmoColor;
            Gizmos.DrawRay(gizmoRayCastOrigin, gravityDirection);
            Gizmos.color = originGizmoColor;
            Gizmos.DrawCube(gizmoRayCastOrigin, new Vector3(0.15f, 0.05f, 0.15f));
            Gizmos.color = hitGizmoColor;
            Physics.SphereCast(gizmoRayCastOrigin, 0.2f, gravityDirection, out var hit, rayCastMaxDistance, groundLayer);
            Gizmos.DrawSphere(hit.point, 0.05f);
        }

        if (isGravityDirectionGizmoEnabled)
        {
            Gizmos.color = gravityGizmoColor;
            Gizmos.DrawRay(position, gravityGizmoForceVector);
        }



        if (isVelocityGizmoEnabled)
        {
            Gizmos.color = movementGizmoColor;
            Gizmos.DrawRay(position, movementGizmoForceVector);
        }
    
        if (isFallingForcesGizmoEnabled)
        {
            Gizmos.color = fallingForcesGizmoColor;
            Gizmos.DrawRay(position, fallingForcesGizmoVector);
        }
        if (isJumpingForcesGizmoEnabled)
        {
            Gizmos.color = jumpingForcesGizmoColor;
            Gizmos.DrawRay(position, jumpingForcesGizmoVector);
        }
        if (isLashingForcesGizmoEnabled)
        {
            Gizmos.color = lashingForcesGizmoColor;
            Gizmos.DrawRay(position, lashingForcesGizmoVector);
        }
        if (isTotalForcesGizmoEnabled)
        {
            Gizmos.color = totalForcesGizmoColor;
            Gizmos.DrawRay(position, totalForcesGizmoVector);
        }

        if (isTargetedDirectionGizmoEnabled)
        {
            Gizmos.color = targetDirectionGizmoColor;
            Gizmos.DrawRay(position, targetedDirectionGizmo);
            Gizmos.DrawSphere(position + targetedDirectionGizmo, 0.05f);
        }

    }
#endif

}
