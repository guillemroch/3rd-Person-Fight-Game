using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Player.StateMachine{
    public class PlayerStateMachine : MonoBehaviour
    {
        
        //TODO: Create in game menu to modify these values and store them in a save file
        #region Variables

        [Header("Time scale")] 
        [SerializeField] [Range(0,1)] private float _timeScale = 1f;
        
        //Player values for movements
        [Header("Movement Variables")]
        [SerializeField] private Vector3 _moveDirection;
        [SerializeField] private Vector3 _targetDirection;

        [Header("Health")]
        [SerializeField] private float _health = 100f;
        [SerializeField] private float _maxHealth = 100f;
        
        //Movement status flags
        [Header("Movement Flags")] 
        [SerializeField] private bool _isGrounded;
        [SerializeField] private bool _isFalling;
        [SerializeField] private bool _isUsingStormlight; 
        [SerializeField] private bool _isInfusing;
        [SerializeField] private bool _isLashing;
        [SerializeField] private bool _isHalfLashing;
        
        //Jump
        [Header("Jump Speeds")] 
        [SerializeField] private float _jumpHeight = 3;
        [SerializeField] private float _dashForce = 25;
        
        //Falling and ground detection variables
        [Header("Falling")] 
        [SerializeField] private float _inAirTimer;
        [SerializeField] private float _maxAirSpeed = 25f;
        [SerializeField] private float _leapingVelocity;
        [SerializeField]
        [Tooltip("Speed that the players falls at")] private float _fallingVelocity;
        [SerializeField] private LayerMask _groundLayer;
        
        [Header("Raycast values")]
        //Raycast values and origins
        [SerializeField] private float _rayCastHeightOffset = 0.5f;
        [Range(0.1f, 2f)]
        [SerializeField] private float _rayCastMaxDistance = 1;
        [Range(0.1f, 1.5f)] 
        [SerializeField] private float _rayCastRadius = 0.2f;

        [Header("Gravity")] 
        [SerializeField] private Vector3 _gravityDirection = Vector3.down; //What is the current gravity orientation for the player
        [SerializeField] private float _gravityIntensity = 9.8f;
        [SerializeField] [Tooltip("Change this to make the player fall faster")] private float _gravityMultiplier = 2;
        [SerializeField] private float _groundedGravity = 0.5f;

        [Header("Lashings")] 
        [SerializeField] [Tooltip("Height of the jump")] private float _halfLashingHeight = 1.0f;
        [SerializeField] [Tooltip("Added speed per each lash")]private float _lashingIntensity = 10;
        [SerializeField] private float _lashCooldown = 0;
        [SerializeField] private float _halfLashRotationSpeed = 30;
        [SerializeField] private float _rollSpeed = 10;
        [SerializeField] private float _rollLerpSpeed = 0.5f;
        [SerializeField] private float _maxUnlashDistance = 10;
        [SerializeField] private float _angleOfIncidence = 0; // -90 means fall position, 90 means dive position 
        
        [SerializeField] public const float DEFAULT_LASHING_INTENSITY = 5;
        [SerializeField] public const float LASHING_INTENSITY_INCREMENT = 5;
        [SerializeField] public const float LASHING_INTENSITY_SMALL_INCREMENT = 1;
        [SerializeField] public const float MAX_LASHING_INTENSITY = 200;
    
        
        [Header("Rotation Lashing")]
        //Todo: Remove all of this as i think i do not use it anymore
        [SerializeField] private  float _maxAngle = 90;//maximum angle it can rotate
        [SerializeField] private float _direction = 0; //Direction of the rotation
        [Range(0, 1)]
        [SerializeField] private float _precision = 0.99f; //Defined for the calculation of the maximum time to reach the max angle
        [Range(0, 5)]
        [SerializeField] private float _damping = 2.1f; //How smooth the rotation is at the end
        [Range(0, 1)]
        [SerializeField] private float _lerpSpeed = 0.5f;
        [SerializeField] private float _offset = 0; //Calculated based on the precision, the maximum angle and the damping
        [SerializeField] private float _maxTime = 1; //Calculated based on the precision, the maximum angle and the damping
        [SerializeField] private float _timeElapsed = 0;
        
        [SerializeField] private Vector3 _rotationAxis = Vector3.forward;

        //Speeds
        [Header("Speeds")] 
        [SerializeField] private float _movementSpeed ;
        [SerializeField] private float _walkingSpeed = 1.5f;
        [SerializeField] private float _runningSpeed = 6f;
        [SerializeField] private float _sprintingSpeed = 8f;
        [SerializeField] private float _rotationSpeed = 15;

        [Header("Stormlight")] 
        [SerializeField] private float _stormlight = 100;
        [SerializeField] private float _breathedStormlight = 0;
        [SerializeField] private float _stormlightRegenRate = 1;
        [SerializeField] private float _stormlightDepletionRate = 0.1f;
        [SerializeField] private float _stormlightBreathConsumption = 20f;
        
        [SerializeField] private float _stormlightBaseDrain = 0.1f;
        [SerializeField] private float _stormlightLashingDrain = 0;
        [SerializeField] private float _stormlightInfusingDrain = 0;
        [SerializeField] private float _stormlightMovementDrain = 0;
        [SerializeField] private float _stormlightHealingDrain = 0;
        
        //Interaction
        [Header("Interactions")] 
        [SerializeField] private float _maxInteractionDistance = 2f;
        [SerializeField] private LayerMask _interactionLayer;
        [SerializeField] private Infusable _infusableSelectedObject;
        [SerializeField] private Infusable.InfusingMode _infusingMode = Infusable.InfusingMode.Full;

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
        [SerializeField] private ParticleSystem _particleSystem;
        [SerializeField] private GameObject _spear;
        
        //State variables
         private PlayerStateFactory _states;
         private PlayerBaseState _currentState;
    
        //getters and setters
        public CameraManager CameraManager { get => _cameraManager; set => _cameraManager = value; }
        public Vector3 MoveDirection { get => _moveDirection; set => _moveDirection = value; }
        public Vector3 TargetDirection { get => _targetDirection; set => _targetDirection = value; }
        public bool IsGrounded { get => _isGrounded; set => _isGrounded = value; }
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
        public float LerpSpeed { get => _lerpSpeed; set => _lerpSpeed = value; }
        public Vector3 RotationAxis { get => _rotationAxis; set => _rotationAxis = value; }
        public UIManager UIManager { get => _uiManager; set => _uiManager = value; }
        public bool IsUsingStormlight { get => _isUsingStormlight; set => _isUsingStormlight = value; }
        public ParticleSystem ParticleSystem { get => _particleSystem; set => _particleSystem = value; }
        public float MaxInteractionDistance { get => _maxInteractionDistance; set => _maxInteractionDistance = value; }
        public LayerMask InteractionLayer { get => _interactionLayer; set => _interactionLayer = value; }
        public float AngleOfIncidence { get => _angleOfIncidence; set => _angleOfIncidence = value; }

        public Infusable InfusableSelectedObject
            {
            get => _infusableSelectedObject;
            set => _infusableSelectedObject = value;
            }

        public bool IsInfusing { get => _isInfusing; set => _isInfusing = value; }
        public float DashForce { get => _dashForce; set => _dashForce = value; }
        public bool IsLashing { get => _isLashing; set => _isLashing = value; }
        public bool IsHalfLashing { get => _isHalfLashing; set => _isHalfLashing = value; }
        public bool IsFalling { get => _isFalling; set => _isFalling = value; }
        public float StormlightBaseDrain { get => _stormlightBaseDrain; set => _stormlightBaseDrain = value; }
        public float StormlightLashingDrain { get => _stormlightLashingDrain; set => _stormlightLashingDrain = value; }

        public float StormlightInfusingDrain
            {
            get => _stormlightInfusingDrain;
            set => _stormlightInfusingDrain = value;
            }

        public float StormlightMovementDrain
            {
            get => _stormlightMovementDrain;
            set => _stormlightMovementDrain = value;
            }

        public float StormlightHealingDrain { get => _stormlightHealingDrain; set => _stormlightHealingDrain = value; }
        public GameObject Spear { get => _spear; set => _spear = value; }
        public Infusable.InfusingMode InfusingMode { get => _infusingMode; set => _infusingMode = value; }

        public float BreathedStormlight { get => _breathedStormlight; set => _breathedStormlight = value; }

        public float StormlightBreathConsumption
            {
            get => _stormlightBreathConsumption;
            set => _stormlightBreathConsumption = value;
            }

        public float Health { get => _health; set => _health = value; }
        public float MaxHealth { get => _maxHealth; set => _maxHealth = value; }

        private String stateString;
        private int stateCount = 0;
        #endregion
    
        private void Awake() {
            Time.timeScale = 1;
            //Setup state
            _states = new PlayerStateFactory(this);
            _currentState = _states.Alive();
            _currentState.EnterState();
        
            //Get references
            _playerManager = GetComponent<PlayerManager>();
            _animatorManager = GetComponent<AnimatorManager>();
            _inputManager = GetComponent<InputManager>();
            _playerRigidbody = GetComponent<Rigidbody>();
            _cameraObject = Camera.main!.transform;
            _uiManager = FindObjectOfType<UIManager>();
            _particleSystem = GetComponent<ParticleSystem>();
        }
        
        public void HandleAllStates()
        {
            _currentState.UpdateStates();
            Time.timeScale = _timeScale;
            
            String currentState = "States: [" + _currentState?.name + "] ||=> [" + _currentState?._currentSubState?.name + "] ||=> [" + _currentState?._currentSubState?._currentSubState?.name +  "] " + "] ||=> [" + _currentState?._currentSubState?._currentSubState?._currentSubState?.name +  "] " + "] ||=> [" + _currentState?._currentSubState?._currentSubState?._currentSubState?._currentSubState?.name +  "] ";
            
            if (currentState.CompareTo(stateString) != 0) {
                
                Debug.Log("States: [" + _currentState?.name + "] ||=> [" + _currentState?._currentSubState?.name + "] ||=> [" + _currentState?._currentSubState?._currentSubState?.name +  "] " + "] ||=> [" + _currentState?._currentSubState?._currentSubState?._currentSubState?.name +  "] " + "] ||=> [" + _currentState?._currentSubState?._currentSubState?._currentSubState?._currentSubState?.name +  "] == stateCount ==> "+ stateCount);
                stateString = "States: [" + _currentState?.name + "] ||=> [" + _currentState?._currentSubState?.name +
                              "] ||=> [" + _currentState?._currentSubState?._currentSubState?.name + "] " + "] ||=> [" +
                              _currentState?._currentSubState?._currentSubState?._currentSubState?.name + "] " +
                              "] ||=> [" + _currentState?._currentSubState?._currentSubState?._currentSubState
                                  ?._currentSubState?.name + "] ";
                stateCount = 0;
            }
            else {
                stateCount++;
            }
         
        }

        public void UpdateAnimatorValues() {
            float velocityZ = InputManager.MoveAmount;
            if (velocityZ < 0.35f) {
                velocityZ = 0;
            }
            if (InputManager.IsSprintPressed) {
                velocityZ *= 2;
            }

            float velocityX = 0;
            if (_isInfusing) {
                velocityX = InputManager.MovementInput.x;
            }

            float velocityY = _playerRigidbody.velocity.magnitude;
            //TODO: normalize Y to adjust to animations and use stormlight thingui to adjust value too
            _animatorManager.VelocityX = velocityX;
            _animatorManager.VelocityY = velocityY ;
            _animatorManager.VelocityZ = velocityZ;
            _animatorManager.IsGrounded = _isGrounded ;
            _animatorManager.IsInteracting = _isInfusing;
            _animatorManager.IsLashing = _isLashing;
            _animatorManager.IsHalfLashing = _isHalfLashing;
            _animatorManager.IsFalling = _isFalling;
            _animatorManager.LookY = _cameraManager.PitchRotation;

            _animatorManager.DiveAngle = _angleOfIncidence / 90;
        }
        public void OnDrawGizmos() {
#if UNITY_EDITOR
           /* Handles.SphereHandleCap(0, _playerTransform.position, Quaternion.identity, 0.1f, EventType.Repaint);
            Handles.ArrowHandleCap(0, _playerTransform.position,
                Quaternion.LookRotation(_playerTransform.up, _playerTransform.right), 1f, EventType.Repaint);
            Handles.color = Color.yellow;
            Handles.ArrowHandleCap(0, _playerTransform.position,
                Quaternion.LookRotation(_gravityDirection.normalized, _playerTransform.forward),
                _gravityDirection.magnitude, EventType.Repaint);
            Handles.ArrowHandleCap(0, _playerTransform.position,
                Quaternion.LookRotation(_cameraObject.forward, _playerTransform.up), 1f, EventType.Repaint);*/
#endif
            // Player Transform
            Gizmos.color = Color.green;
            Gizmos.DrawRay(_playerTransform.position, _playerTransform.up * 0.2f);
            Gizmos.color = Color.red;
            Gizmos.DrawRay(_playerTransform.position, _playerTransform.right * 0.2f);
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(_playerTransform.position, _playerTransform.forward * 0.2f);
            
            /*Vector3 planeOrthogonal = Vector3.Cross(PlayerTransform.right, Vector3.down);
            Vector3 projectedVector = Vector3.ProjectOnPlane(GravityDirection, planeOrthogonal);
            //float angle = Vector3.Angle(Vector3.down, projectedVector);
            float angle = Vector3.SignedAngle(Vector3.down, projectedVector, Vector3.up);

            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(_playerTransform.position, GravityDirection);
            Gizmos.color = Color.magenta;
            Gizmos.DrawRay(_playerTransform.position, planeOrthogonal);
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(_playerTransform.position, projectedVector);*/
            
           //GROUND DETECTION RAYCAST 
           Vector3 rayCastOrigin = PlayerRigidbody.worldCenterOfMass;
           rayCastOrigin += PlayerTransform.up * RayCastHeightOffset; //Consider the orientation of the Player
           RaycastHit hit;

           Gizmos.color = Color.yellow;
           Gizmos.DrawSphere(rayCastOrigin, 0.1f);
           Gizmos.DrawRay(rayCastOrigin, GravityDirection*RayCastMaxDistance);
           if (Physics.SphereCast(
                   rayCastOrigin,RayCastRadius,GravityDirection, out hit,RayCastMaxDistance,
                   GroundLayer)) {
               Gizmos.color = Color.magenta;
               Gizmos.DrawCube(hit.point, Vector3.one * RayCastRadius);
           }
        }

        public void Dammage(float health) {
            if (Health > 0) {
                Health -= health;
                if (Health < 0)
                    Health = 0;
            }
        }

        public void ResetLevel() {
            Time.timeScale = 1;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
