using System;
using Cinemachine;
using UnityEditor;
using UnityEngine;

namespace Player.StateMachine{
    [SaveDuringPlay]
    public class PlayerStateMachine : MonoBehaviour
    {
        
        //TODO: Create in game menu to modify these values and store them in a save file
        #region Variables
        //Player values for movements
        [Header("Movement Variables")]
        [SerializeField] private Vector3 _moveDirection;
        [SerializeField] private Vector3 _targetDirection;

        //Movement status flags
        [Header("Movement Flags")] 
        [SerializeField] public bool isGrounded;
        
        //Falling and ground detection variables
        [Header("Falling")] 
        [SerializeField] private float _inAirTimer;
        [SerializeField] private float _maxAirSpeed = 25f;
        [SerializeField] private float _leapingVelocity;
        [SerializeField] private float _fallingVelocity;
        [SerializeField] private LayerMask _groundLayer;
        [SerializeField] private float _rayCastHeightOffset = 0.5f;
        [Range(0.1f, 2f)]
        [SerializeField] private float _rayCastMaxDistance = 1;

        [Range(0.1f, 1.5f)] 
        [SerializeField] private float _rayCastRadius = 0.2f;

        [Header("Gravity")] 
        [SerializeField] private Vector3 _gravityDirection = Vector3.down; //What is the current gravity orientation for the player
        [SerializeField] private float _gravityIntensity = 9.8f;
        [SerializeField] private float _gravityMultiplier = 2;
        [SerializeField] private float _groundedGravity = 0.5f;

        [Header("Lashings")] 
        [SerializeField] private float _halfLashingHeight = 1.0f;
        [SerializeField] private float _lashingIntensity = 10;
        [SerializeField] private float _lashCooldown = 0;
        [SerializeField] private float _halfLashRotationSpeed = 30;
        [SerializeField] private float  _rollSpeed = 10;
        [SerializeField] private float  _rollLerpSpeed = 0.5f;
        [SerializeField] private float _maxUnlashDistance = 10;
        
        [SerializeField] public const float DEFAULT_LASHING_INTENSITY = 5;
        [SerializeField] public const float LASHING_INTENSITY_INCREMENT = 5;
        [SerializeField] public const float LASHING_INTENSITY_SMALL_INCREMENT = 1;
        [SerializeField] public const float MAX_LASHING_INTENSITY = 200;
    
        
        [Header("Rotation Lashing")]
        [SerializeField] private  float maxAngle = 90;//maximum angle it can rotate
        [SerializeField] private float direction = 0; //Direction of the rotation
        [Range(0, 1)]
        [SerializeField] private float precision = 0.99f; //Defined for the calculation of the maximum time to reach the max angle
        [Range(0, 5)]
        [SerializeField] private float damping = 2.1f; //How smooth the rotation is at the end
        [Range(0, 1)]
        [SerializeField] private float lerpSpeed = 0.5f;
        [SerializeField] private float offset = 0; //Calculated based on the precision, the maximum angle and the damping
        [SerializeField] private float maxTime = 1; //Calculated based on the precision, the maximum angle and the damping
        [SerializeField] private float timeElapsed = 0;
        
        [SerializeField] private Vector3 rotationAxis = Vector3.forward;

        //Speeds
        [Header("Speeds")] 
        [SerializeField] private float _movementSpeed ;
        [SerializeField] private float _walkingSpeed = 1.5f;
        [SerializeField] private float _runningSpeed = 6f;
        [SerializeField] private float _sprintingSpeed = 8f;
        [SerializeField] private float _rotationSpeed = 15;

        [Header("Stormlight")] 
        [SerializeField] private bool _isUsingStormlight = false; 
        [SerializeField] private float _stormlight = 100;
        [SerializeField] private float _stormlightRegenRate = 1;
        [SerializeField] private float _stormlightDepletionRate = 0.1f;
    
        //Jump
        [Header("Jump Speeds")] 
        [SerializeField] private float _jumpHeight = 3;
        
        //Interaction
        [Header("Interactions")] 
        [SerializeField] private float _maxInteractionDistance = 2f;
        [SerializeField] private LayerMask _interactionLayer;
        [SerializeField] private Infusable _infusableSelectedObject;
        [SerializeField] private bool _isInfusing;
        
        

        //References
        [Header("References")] 
        [SerializeField] private PlayerManager _playerManager;
        [SerializeField] private AnimatorManager _animatorManager;
        [SerializeField] private InputManager _inputManager;
        [SerializeField] private Transform _cameraObject;
        [SerializeField] private CameraManager _cameraManager;
        [SerializeField] private Transform _playerTransform;
        [SerializeField] private Rigidbody _playerRigidbody;
        [SerializeField] private UIManager _uiManager;
        [SerializeField] private TrailRenderer _particleSystem;
        
        //State variables
        [SerializeField] public PlayerStateFactory _states;
        [SerializeField] public PlayerBaseState _currentState;
    
        //getters and setters
        public CameraManager CameraManager { get => _cameraManager; set => _cameraManager = value; }
        public Vector3 MoveDirection { get => _moveDirection; set => _moveDirection = value; }
        public Vector3 TargetDirection { get => _targetDirection; set => _targetDirection = value; }
        public bool IsGrounded { get => isGrounded; set => isGrounded = value; }
        public float InAirTimer { get => _inAirTimer; set => _inAirTimer = value; }
        public float MaxAirSpeed { get => _maxAirSpeed; set => _maxAirSpeed = value; }
        public float LeapingVelocity { get => _leapingVelocity; set => _leapingVelocity = value; }
        public float FallingVelocity { get => _fallingVelocity; set => _fallingVelocity = value; }
        public LayerMask GroundLayer { get => _groundLayer; set => _groundLayer = value; }
        public float RayCastRadius { get => _rayCastRadius; set => _rayCastRadius = value; }
        public float RayCastHeightOffset { get => _rayCastHeightOffset; set => _rayCastHeightOffset = value; }
        public float RayCastMaxDistance { get => _rayCastMaxDistance; set => _rayCastMaxDistance = value; }
        public Vector3 GravityDirection { get => _gravityDirection; set => _gravityDirection = value; }
        public float GravityIntensity { get => _gravityIntensity; set => _gravityIntensity = value; }
        public float GravityMultiplier { get => _gravityMultiplier; set => _gravityMultiplier = value; }
        public float GroundedGravity { get => _groundedGravity; set => _groundedGravity = value; }
        public float HalfLashingHeight { get => _halfLashingHeight; set => _halfLashingHeight = value; }
        public float MovementSpeed { get => _movementSpeed; set => _movementSpeed = value; }
        public float WalkingSpeed { get => _walkingSpeed; set => _walkingSpeed = value; }
        public float RunningSpeed { get => _runningSpeed; set => _runningSpeed = value; }
        public float SprintingSpeed { get => _sprintingSpeed; set => _sprintingSpeed = value; }
        public float RotationSpeed { get => _rotationSpeed; set => _rotationSpeed = value; }
        public float Stormlight { get => _stormlight; set => _stormlight = value; }
        public float StormlightRegenRate { get => _stormlightRegenRate; set => _stormlightRegenRate = value; }
        public float StormlightDepletionRate { get => _stormlightDepletionRate; set => _stormlightDepletionRate = value; }
        public float JumpHeight { get => _jumpHeight; set => _jumpHeight = value; }
        public PlayerManager PlayerManager { get => _playerManager; set => _playerManager = value; }
        public AnimatorManager AnimatorManager { get => _animatorManager; set => _animatorManager = value; }
        public InputManager InputManager { get => _inputManager; set => _inputManager = value; }
        public Transform CameraObject { get => _cameraObject; set => _cameraObject = value; }
        public Transform PlayerTransform { get => _playerTransform; set => _playerTransform = value; }
        public Rigidbody PlayerRigidbody { get => _playerRigidbody; set => _playerRigidbody = value; }
        public PlayerStateFactory States { get => _states; set => _states = value; }
        public PlayerBaseState CurrentState { get => _currentState; set => _currentState = value; }
        public float LashingIntensity { get => _lashingIntensity; set => _lashingIntensity = value; }
        public float LashCooldown { get => _lashCooldown; set => _lashCooldown = value; }
        public float HalfLashRotationSpeed { get => _halfLashRotationSpeed; set => _halfLashRotationSpeed = value; }
        public float RollSpeed { get => _rollSpeed; set => _rollSpeed = value; }
        public float RollLerpSpeed { get => _rollLerpSpeed; set => _rollLerpSpeed = value; }
        public float MaxUnlashDistance { get => _maxUnlashDistance; set => _maxUnlashDistance = value; }
        public float MaxAngle { get => maxAngle; set => maxAngle = value; }
        public float Direction { get => direction; set => direction = value; }
        public float Precision { get => precision; set => precision = value; }
        public float Damping { get => damping; set => damping = value; }
        public float LerpSpeed { get => lerpSpeed; set => lerpSpeed = value; }
        public float Offset { get => offset; set => offset = value; }
        public float MaxTime { get => maxTime; set => maxTime = value; }
        public float TimeElapsed { get => timeElapsed; set => timeElapsed = value; }
        public Vector3 RotationAxis { get => rotationAxis; set => rotationAxis = value; }
        public UIManager UIManager { get => _uiManager; set => _uiManager = value; }
        public bool IsUsingStormlight { get => _isUsingStormlight; set => _isUsingStormlight = value; }
        public TrailRenderer ParticleSystem { get => _particleSystem; set => _particleSystem = value; }
        public float MaxInteractionDistance { get => _maxInteractionDistance; set => _maxInteractionDistance = value; }
        public LayerMask InteractionLayer { get => _interactionLayer; set => _interactionLayer = value; }

        public Infusable InfusableSelectedObject
            {
            get => _infusableSelectedObject;
            set => _infusableSelectedObject = value;
            }

        public bool IsInfusing { get => _isInfusing; set => _isInfusing = value; }

        #endregion
    
        private void Awake()
        {   
            //Setup state
            _states = new PlayerStateFactory(this);
            _currentState = _states.Normal();
            _currentState.EnterState();
        
            //Get references
            _playerManager = GetComponent<PlayerManager>();
            _animatorManager = GetComponent<AnimatorManager>();
            _inputManager = GetComponent<InputManager>();
            _playerRigidbody = GetComponent<Rigidbody>();
            _cameraObject = Camera.main!.transform;
            _uiManager = FindObjectOfType<UIManager>();
            _particleSystem = GetComponent<TrailRenderer>();
        }
        
        public void HandleAllStates()
        {
            _currentState.UpdateStates();
            //Debug.Log("States: [" + _currentState?.name + "] ||=> [" + _currentState?._currentSubState?.name + "] ||=> [" + _currentState?._currentSubState?._currentSubState?.name +  "] ");
        }

        public void OnDrawGizmos() {
            /*Handles.SphereHandleCap(0, _playerTransform.position, Quaternion.identity, 0.1f, EventType.Repaint);
            Handles.ArrowHandleCap(0, _playerTransform.position,
                Quaternion.LookRotation(_playerTransform.up, _playerTransform.right), 1f, EventType.Repaint);
            Handles.color = Color.yellow;
            Handles.ArrowHandleCap(0, _playerTransform.position,
                Quaternion.LookRotation(_gravityDirection.normalized, _playerTransform.forward),
                _gravityDirection.magnitude, EventType.Repaint);
            Handles.ArrowHandleCap(0, _playerTransform.position,
                Quaternion.LookRotation(_cameraObject.forward, _playerTransform.up), 1f, EventType.Repaint);*/

            Gizmos.color = Color.green;
            Gizmos.DrawRay(_playerTransform.position, _playerTransform.up * 0.2f);
            Gizmos.color = Color.red;
            Gizmos.DrawRay(_playerTransform.position, _playerTransform.right * 0.2f);
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(_playerTransform.position, _playerTransform.forward * 0.2f);
        }
    }
}
