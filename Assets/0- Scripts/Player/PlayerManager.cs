using System;
using Player.StateMachine;
using UnityEngine;

namespace Player{
 
    /**
 * PlayerManager
 *
 * In here we have every controller or manager related to the player together.
 * It handles inputs and handle all the movement and animations responding to the corresponding Input.
 * It is also where the camera is called in the LateUpdate()
 */
    public class PlayerManager : MonoBehaviour
    {
        [SerializeField] private InputManager _inputManager; //Calls Input in Update()
        [SerializeField] private PlayerStateMachine _playerStateMachine; //Calls player movement calculations in FixedUpdate()
        [SerializeField] private AnimatorManager _animatorManager; //Calls and modifies animator in LateUpdate()
        [SerializeField] private Animator _animator;  //Calls and modifies animator in LateUpdate()
        [SerializeField] private CameraManager _cameraManager; //Calls and modifies camera in LateUpdate()
        [SerializeField] private UIManager _uiManager; // Calls and modifies UI elements in FixedUpdate()
    
        private void Awake()
        {
            _inputManager = GetComponent<InputManager>();
            _playerStateMachine = GetComponent<PlayerStateMachine>();
            _animatorManager = GetComponent<AnimatorManager>();
            _cameraManager = FindObjectOfType<CameraManager>();
            _uiManager = FindObjectOfType<UIManager>();
        
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Start() {
            _uiManager.Initialize(_playerStateMachine);
        }

        private void Update()
        {
            _inputManager.HandleAllInputs();
            _animatorManager.UpdateValues();
        }

        private void FixedUpdate()
        {
            _playerStateMachine.HandleAllStates();
            _uiManager.UpdateUI();
        }

        private void LateUpdate()
        {
            _cameraManager.HandleAllCameraMovement();
            _playerStateMachine.UpdateAnimatorValues();
        }
    }
}
