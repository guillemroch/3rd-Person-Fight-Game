using Unity.Collections;
using UnityEngine;

namespace Player{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] [ReadOnly] private PlayerInputActions _playerInputs;
        [SerializeField] [ReadOnly] private AnimatorManager _animatorManager;

        //Input actions
        [SerializeField] [ReadOnly] private bool _configurationInput;
        
        [SerializeField] [ReadOnly] private Vector2 _movementInput;
        [SerializeField] [ReadOnly] private Vector2 _lookInput;
        [SerializeField] [ReadOnly] private bool _isSprintPressed;
        [SerializeField] [ReadOnly] private bool _lashInput;
        [SerializeField] [ReadOnly] private bool _unLashInput;
        [SerializeField] [ReadOnly] private float _smallLashInput;
        [SerializeField] [ReadOnly] private float _smallUnLashInput;

        [SerializeField] [ReadOnly] private float _moveAmount;
        [SerializeField] [ReadOnly] private float _rollInput;
        
        [SerializeField] [ReadOnly] private bool _stormlightInput;
        [SerializeField] [ReadOnly] private bool _infuseInput;

        [SerializeField] [ReadOnly] private bool _dashInput;
        [SerializeField] [ReadOnly] private bool _diveInput;

        [SerializeField] [ReadOnly] private bool _lightAttack;
        [SerializeField] [ReadOnly] private bool _heavyAttack;
        [SerializeField] [ReadOnly] private bool _blockAttack;
        
        // getters and setters
        public Vector2 LookInput { get => _lookInput; set => _lookInput = value; }
        public Vector2 MovementInput { get => _movementInput; set => _movementInput = value; }
        public float MoveAmount { get => _moveAmount; set => _moveAmount = value; }
        public bool IsJumpPressed { get; private set; }
        public bool IsSprintPressed { get => _isSprintPressed; set => _isSprintPressed = value; }
        public bool LashInput { get => _lashInput; set => _lashInput = value; }
        public bool UnLashInput { get => _unLashInput; set => _unLashInput = value; }
        public float SmallLashInput { get => _smallLashInput; set => _smallLashInput = value; }
        public float SmallUnLashInput { get => _smallUnLashInput; set => _smallUnLashInput = value; }
        public float RollInput { get => _rollInput; set => _rollInput = value; }
        public bool StormlightInput { get => _stormlightInput; set => _stormlightInput = value; }
        public bool InfuseInput { get => _infuseInput; set => _infuseInput = value; }
        public bool DashInput { get => _dashInput; set => _dashInput = value; }
        public bool DiveInput { get => _diveInput; set => _diveInput = value; }
        public bool LightAttack { get => _lightAttack; set => _lightAttack = value; }
        public bool HeavyAttack { get => _heavyAttack; set => _heavyAttack = value; }
        public bool BlockAttack { get => _blockAttack; set => _blockAttack = value; }
        public bool ConfigurationInput { get => _configurationInput; set => _configurationInput = value; }

        private void Awake()
        {
            _animatorManager = GetComponent<AnimatorManager>();
        }

        private void OnEnable()
        {
            if (_playerInputs == null)
            {
                _playerInputs = new PlayerInputActions();
                
                _playerInputs.UI.Configuration.performed += _ => _configurationInput = true;
                _playerInputs.UI.Configuration.canceled += _ => _configurationInput = false;

                _playerInputs.Player.Move.performed += i => _movementInput = i.ReadValue<Vector2>();
                
                _playerInputs.Player.Look.performed += i => _lookInput = i.ReadValue<Vector2>();

                _playerInputs.Player.Sprint.performed += i => _isSprintPressed = true;
                _playerInputs.Player.Sprint.canceled += i => _isSprintPressed = false;
            
                _playerInputs.Player.Jump.performed += i => IsJumpPressed = true;
                _playerInputs.Player.Jump.canceled += i => IsJumpPressed = false;
            
                _playerInputs.Player.Lash.performed += i => _lashInput = true;
                _playerInputs.Player.Lash.canceled += i => _lashInput = false;
            
                _playerInputs.Player.UnLash.performed += i => _unLashInput = true;
                _playerInputs.Player.UnLash.canceled += i => _unLashInput = false;
                
                _playerInputs.Player.SmallLash.performed += i => _smallLashInput = i.ReadValue<float>();
                _playerInputs.Player.SmallLash.canceled += i => _smallLashInput = 0;
                
                _playerInputs.Player.SmallUnLash.performed += i => _smallUnLashInput = i.ReadValue<float>();
                _playerInputs.Player.SmallUnLash.canceled += i => _smallUnLashInput = 0;
                
                _playerInputs.Player.RollLeft.performed += i => _rollInput = -i.ReadValue<float>();
                _playerInputs.Player.RollLeft.canceled += i => _rollInput = 0;
                
                _playerInputs.Player.RollRight.performed += i => _rollInput = i.ReadValue<float>();
                _playerInputs.Player.RollRight.canceled += i => _rollInput = 0;

                _playerInputs.Player.StormlightInput.performed += i => _stormlightInput = true;
                _playerInputs.Player.StormlightInput.canceled += i => _stormlightInput = false;
                
                _playerInputs.Player.InfuseInput.performed += _ => _infuseInput = true;
                _playerInputs.Player.InfuseInput.canceled += _ => _infuseInput = false;
                
                _playerInputs.Player.DashInput.performed += _ => _dashInput = true;
                _playerInputs.Player.DashInput.canceled += _ => _infuseInput = false;

                _playerInputs.Player.DiveInput.performed += _ => _diveInput = true;
                _playerInputs.Player.DiveInput.canceled += _ => _diveInput = false;
                
                _playerInputs.Player.LightAttackInput.performed += _ => _lightAttack = true;
                _playerInputs.Player.LightAttackInput.canceled += _ => _lightAttack = false;
                
                _playerInputs.Player.HeavyAttackInput.performed += _ => _heavyAttack = true;
                _playerInputs.Player.HeavyAttackInput.canceled += _ => _heavyAttack = false;
                
                _playerInputs.Player.BlockInput.performed += _ => _blockAttack = true;
                _playerInputs.Player.BlockInput.canceled += _ => _blockAttack = false;
            }
            _playerInputs.Enable();
        }

        private void OnDisable()
        {
            _playerInputs.Disable();
        }

        public void HandleAllInputs()
        {
            HandleMovementInput();
            HandleCameraInput();
        }

        private void HandleMovementInput()
        {
            _moveAmount = Mathf.Clamp01(Mathf.Abs(_movementInput.x) + Mathf.Abs(_movementInput.y));
        }

        private void HandleCameraInput()
        {
        
        }
    
        public void ResetJumpInput()
        {
            IsJumpPressed = false;
        }
        public void ResetLashInput()
        {
            _lashInput = false;
        }
    
        public void ResetUnLashInput()
        {
            _unLashInput = false;
        }


        public void ResetStormlightInput() {
            _stormlightInput = false;
        }

        public void ResetDashInput() {
            _dashInput = false;
        }

        public void ResetLightAttackInput() {
            _lightAttack = false;
        }

        public void ResetConfigurationInput() {
            _configurationInput = false;
        }
    }
}
