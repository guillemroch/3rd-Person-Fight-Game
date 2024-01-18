using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    #region Variables
    //Player values for movements
    [Header("Movement Variables")]
    public Vector3 moveDirection;
    public Vector3 targetDirection;

    //Movement status flags
    //TODO: IF state machine correctly implemented, these flags should be removed
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
    #region Gizmos
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
    //State variables
    PlayerBaseState _currentState;
    PlayerStateFactory _states;
    
    //getters and setters
    //TODO: Setup getters and setters of local variables
    //TODO: Setup getters and setters for variables from the InputManager and AnimatorManager
    public PlayerBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }
    public InputManager InputManager { get { return inputManager; } }
    public AnimatorManager AnimatorManager { get { return animatorManager; } }
    public Rigidbody PlayerRigidbody { get { return playerRigidbody; } }
    public float GravityIntensity { get { return gravityIntensity; } }
    public float JumpHeight { get { return jumpHeight; } }
    public Vector3 GravityDirection { get { return gravityDirection; } }
    
    
    #endregion
    
    private void Awake()
    {   
        //Setup state
        _states = new PlayerStateFactory(this);
        _currentState = _states.Grounded();
        _currentState.EnterState();
        
        //Get references
        playerManager = GetComponent<PlayerManager>();
        animatorManager = GetComponent<AnimatorManager>();
        inputManager = GetComponent<InputManager>();
        playerRigidbody = GetComponent<Rigidbody>();
        cameraManager = FindObjectOfType<CameraManager>();
        cameraObject = Camera.main.transform;
    }
    

    public void HandleAllStates()
    {
        _currentState.UpdateState();
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
