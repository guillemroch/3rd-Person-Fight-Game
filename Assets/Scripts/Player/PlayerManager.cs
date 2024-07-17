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
        
    
        private void Awake()
        {

            _inputManager = GetComponent<InputManager>();
            _playerStateMachine = GetComponent<PlayerStateMachine>();
            _animatorManager = GetComponent<AnimatorManager>();
            _animator = GetComponent<Animator>();
            _cameraManager = FindObjectOfType<CameraManager>();
        
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            _inputManager.HandleAllInputs();
        }

        private void FixedUpdate()
        {
            _playerStateMachine.HandleAllStates();
        }

        private void LateUpdate()
        {
            _cameraManager.HandleAllCameraMovement();
            _animator.SetBool(_animatorManager.IsGroundedHash.GetHashCode(), _playerStateMachine.isGrounded);
        }
    }
}
