using Player.StateMachine;
using UnityEngine;

namespace Player{
    public class InputManager : MonoBehaviour
    {
        private PlayerInputActions _playerInputs;
        private AnimatorManager _animatorManager;

        //Input actions
        private Vector2 _movementInput;
        private bool _isSprintPressed;
        private bool _halfLashInput;
        private bool _lashInput;
        private float _moveAmount;
        public Vector2 MovementInput {
            get => _movementInput;
            set => _movementInput = value;
        }

        public float MoveAmount {
            get => _moveAmount;
            set => _moveAmount = value;
        }

        
    
        // getters and setters
        public bool IsJumpPressed { get; private set; }

        public bool IsSprintPressed { get => _isSprintPressed; set => _isSprintPressed = value; }
        public bool HalfLashInput { get => _halfLashInput; set => _halfLashInput = value; }

        public bool LashInput { get => _lashInput; set => _lashInput = value; }

        private void Awake()
        {
            _animatorManager = GetComponent<AnimatorManager>();
        }

        private void OnEnable()
        {
            if (_playerInputs == null)
            {
                _playerInputs = new PlayerInputActions();

                _playerInputs.Player.Move.performed += i => _movementInput = i.ReadValue<Vector2>();

                _playerInputs.Player.Sprint.performed += i => _isSprintPressed = true;
                _playerInputs.Player.Sprint.canceled += i => _isSprintPressed = false;
            
                _playerInputs.Player.Jump.performed += i => IsJumpPressed = true;
                _playerInputs.Player.Jump.canceled += i => IsJumpPressed = false;
            
                _playerInputs.Player.HalfLash.performed += i => _halfLashInput = true;
                _playerInputs.Player.HalfLash.canceled += i => _halfLashInput = false;
            
                _playerInputs.Player.ConfirmLash.performed += i => _lashInput = true;
                _playerInputs.Player.ConfirmLash.canceled += i => _lashInput = false;

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
            _animatorManager.UpdateAnimatorValues(new Vector2(0, _moveAmount), _isSprintPressed && _moveAmount > 0.5f);
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
    
        public void ResetHalfLashInput()
        {
            _halfLashInput = false;
        }



 
    
    
    }
}
