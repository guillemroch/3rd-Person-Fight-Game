
using System.Collections;
using Player;
using UnityEngine;

/**
 * PlayerMovement:
 * Calculates player movement
 **/

public class PlayerMovement : MonoBehaviour
{

    #region Variables
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
    public bool isLashing;

  
    //Falling and ground detection variables
    [Header("Falling")] 
    public float inAirTimer;
    public float maxAirSpeed = 25f;
    public float leapingVelocity;
    public float fallingVelocity;
    public LayerMask groundLayer;
    public float rayCastHeightOffset = 0.5f;
    [Range(0.1f, 1.5f)]
    public float rayCastMaxDistance = 1;
    [Range(0.1f, 1.5f)]
    public float rayCastRadius = 0.2f;

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
    
    [Header("Stamina")]
    public float stamina = 100;
    public float staminaRegenRate = 1;
    public float staminaDepletionRate = 1;
    
    
    //Jump
    [Header("Jump Speeds")] 
    public float jumpHeight = 3;

    //References
    [Header("References")] 
    public PlayerManager playerManager;
    public AnimatorManager animatorManager;
    public InputManager inputManager;
    public Transform cameraObject;
    public Transform playerTransform;
    public Rigidbody playerRigidbody;
    public CameraManager cameraManager;
    
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

    #endregion
    
   
    //------------ METHODS --------------------------------------------------------------------
    
    #region Player Logic
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
        cameraManager = FindObjectOfType<CameraManager>();
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
        
        playerTransform = transform;
        
        //First Handle the Falling
        if (!isHalfLashing)
            HandleFallingAndLanding();
        
        if (playerManager.isInteracting)
            return;
        
        
        //If the player is interacting or jumping we do not want him to move

        if (!isHalfLashing && !isLashing )
        {
            HandleMovement();
            
        } else if (isLashing)
        {
            HandleLashMovement();
        }
        
        if (isHalfLashing)
        {
            HandleHalfLashingRotation();
        }
        else if (!isLashing)
        {
            HandleRotation();
        }
        else
        {
            HandleLashingRotation();
        }

        if (!isHalfLashing && !isJumping)
        {
            HandleGravity();
        }
            
    }

    
    /**
     * Handles the movement vector, this is the speed and direction
    */
    private void HandleMovement()
    {
        if (isHalfLashing || isLashing) return;
        
        moveDirection = cameraObject.forward * inputManager.movementInput.y + cameraObject.right * inputManager.movementInput.x;

        float moveDot = Vector3.Dot(moveDirection, gravityDirection);
        float magSquared = gravityDirection.sqrMagnitude;
    
        Vector3 projection = (moveDot / magSquared) * gravityDirection;
        moveDirection += -projection;
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
        
        movementGizmoForceVector = moveDirection * movementSpeed;
        playerRigidbody.AddForce(moveDirection * movementSpeed, ForceMode.Force);
        
        totalForcesGizmoVector += movementGizmoForceVector;
    }

    private void HandleLashMovement()
    {
        moveDirection = playerTransform.right * inputManager.movementInput.y +
                        playerTransform.forward * -inputManager.movementInput.x;
        

        moveDirection.Normalize();
        
        Quaternion rotation = Quaternion.Euler(moveDirection) * Quaternion.Euler(gravityDirection);
        
        gravityDirection = rotation * gravityDirection;

    }

    /**
     * Handles the player rotation
    */
    private void HandleRotation()
    {
        
        if (isHalfLashing || isLashing) return;
        
        
        targetDirection = Vector3.zero; //Resets target direction
        
        //calculate orientation based on camera position
        targetDirection = cameraObject.forward * inputManager.movementInput.y +
                          cameraObject.right * inputManager.movementInput.x;
        
        float moveDot = Vector3.Dot(targetDirection, gravityDirection);
        float magSquared = gravityDirection.sqrMagnitude;
    
        Vector3 projection = (moveDot / magSquared) * gravityDirection;
        targetDirection += -projection;
        targetDirection.Normalize();
        
        if (targetDirection == Vector3.zero)
            targetDirection = playerTransform.forward;

        targetedDirectionGizmo = targetDirection;

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection, -gravityDirection);

        transform.rotation = Quaternion.Slerp(playerTransform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        
    }

    private void HandleHalfLashingRotation()
    {
        targetedDirectionGizmo = playerTransform.up;
        //Make the player rotate with the camera, making that we always see the back of the player
        transform.RotateAround(playerRigidbody.worldCenterOfMass, playerTransform.forward, rotationSpeed * Time.deltaTime * -inputManager.cameraInput.x);
        transform.RotateAround(playerRigidbody.worldCenterOfMass, playerTransform.right, rotationSpeed * Time.deltaTime * -inputManager.cameraInput.y);

    }
    
    private void HandleLashingRotation()
    {
        targetedDirectionGizmo = playerTransform.up;

        Quaternion targetRotation = Quaternion.FromToRotation(playerTransform.up, gravityDirection);
        playerTransform.rotation = targetRotation * playerTransform.rotation;
    }
    

    
    /**
     * Handles falling and landing
     */
    private void HandleFallingAndLanding()
    {
        if (isHalfLashing) return;
        Vector3 rayCastOrigin = transform.position - rayCastHeightOffset * gravityDirection;
        rayCastOrigin = playerRigidbody.worldCenterOfMass;
        gizmoRayCastOrigin = rayCastOrigin;

        if (!isGrounded && !isJumping)
        {
            if (!playerManager.isInteracting && !isLashing)
            {
                animatorManager.PlayTargetAnimation("Fall", true);
            }
            
            if (inAirTimer <= maxAirSpeed) inAirTimer += Time.deltaTime;
            if (!isLashing)
                playerRigidbody.AddForce(playerTransform.forward * (leapingVelocity * playerRigidbody.velocity.magnitude), ForceMode.Force);
            playerRigidbody.AddForce(gravityDirection * (fallingVelocity * inAirTimer), ForceMode.Force);
            
            //Gizmos
            fallingForcesGizmoVector =
                transform.forward * leapingVelocity + gravityDirection * (fallingVelocity * inAirTimer);
            totalForcesGizmoVector += fallingForcesGizmoVector;
        }
        
        if (Physics.SphereCast(rayCastOrigin, rayCastRadius, gravityDirection, out RaycastHit hit ,rayCastMaxDistance, groundLayer))
        {
            if (!isGrounded && !playerManager.isInteracting)
            {
                animatorManager.PlayTargetAnimation("Land", true);
            }

            if (isLashing)
            {
              isGrounded = true;
              gravityDirection = -hit.normal;  
              StartCoroutine(TriggerLandingFromLashingCoroutine(hit.normal, hit.point, 0.25f));
            }
            

            inAirTimer = 0;
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    public IEnumerator TriggerLandingFromLashingCoroutine(Vector3 targetNormal, Vector3 hitPoint, float duration)
    {
        //TODO: Make it work without a Coroutine
        //isLandingFromLashing = true;
        Vector3 originalPosition = transform.position;
        Vector3 centerOfMass = playerRigidbody.worldCenterOfMass;
         
        Quaternion startRotation = playerTransform.rotation;
        Quaternion targetRotation = Quaternion.FromToRotation(playerTransform.up, targetNormal) * playerTransform.rotation;

        float timeElapsed = 0;
        while (timeElapsed < duration)
        {
            transform.position = centerOfMass;
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, timeElapsed / duration);
            transform.position = originalPosition;
            
            transform.position = Vector3.Lerp(originalPosition, hitPoint + targetNormal * 0.5f, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = hitPoint;
        transform.rotation = targetRotation;
        isLashing = false;
       //isLandingFromLashing = false;
    }

    /**
     * Handles Jumping
     */
    public void TriggerJumping()
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

    public void TriggerHalfLash()
    {
        if (isHalfLashing) return;
        
        animatorManager.animator.SetBool("isHalfLashing", true);
        animatorManager.PlayTargetAnimation("Half Lashing", false);
        
        playerRigidbody.AddForce(halfLashingHeight * -gravityDirection, ForceMode.Impulse);
        if (isGrounded)
            StartCoroutine(TriggerHalfLashingRotationCoroutine(0.5f));

        lashingForcesGizmoVector = halfLashingHeight * -gravityDirection;
        totalForcesGizmoVector += lashingForcesGizmoVector;
        isGrounded = false;
        
        inAirTimer = 0;
    }
    
    public IEnumerator TriggerHalfLashingRotationCoroutine(float duration)
    {
        //isHalfLashingFromGround = true;
        float timeElapsed = 0;
        Quaternion startRotation = playerTransform.rotation;
        Quaternion targetRotation = Quaternion.FromToRotation(playerTransform.up, playerTransform.forward) * playerTransform.rotation;
        while (timeElapsed < duration)
        {
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        transform.rotation = targetRotation;
        //isHalfLashingFromGround = false;
    }
    
    public void TriggerLash() 
    {
        if (isLashing || !isHalfLashing) return;
        
        gravityDirection = playerTransform.up;

        isHalfLashing = false; //TODO: Change this to a state machine
        animatorManager.animator.SetBool("isHalfLashing", false);
        animatorManager.animator.SetBool("isLashing", true);
        
        
        
    }
    

    private void HandleGravity()
    {
        gravityGizmoForceVector = Vector3.zero;
        if (isJumping) return;
        if (isHalfLashing) return;
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

 
    
    #endregion


#if UNITY_EDITOR
    
    /**
     * Debug Gizmos
     * Cyan Ray: Ray
     * Magenta cube: rayCastOrigin
     * Green Sphere: Raycast hit
     */
    private void OnDrawGizmos()
    {

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
            Physics.SphereCast(gizmoRayCastOrigin, rayCastRadius, gravityDirection, out var hit, rayCastMaxDistance, groundLayer);
            if (isGrounded) Gizmos.DrawSphere(hit.point, rayCastRadius);
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
            Gizmos.DrawRay(position, targetedDirectionGizmo * 100);
            Gizmos.DrawSphere(position + targetedDirectionGizmo, 0.05f);
        }
        

    }
#endif

}

