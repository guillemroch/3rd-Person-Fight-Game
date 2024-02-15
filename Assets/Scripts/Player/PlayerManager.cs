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
        private InputManager _inputManager; //Calls Input in Update()
        private PlayerStateMachine _playerStateMachine; //Calls player movement calculations in FixedUpdate()
        private Animator _animator;  //Calls and modifies animator in LateUpdate()
    
    
        private void Awake()
        {

            _inputManager = GetComponent<InputManager>();
            _playerStateMachine = GetComponent<PlayerStateMachine>();
            _animator = GetComponent<Animator>();
        
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
            _animator.SetBool("isGrounded", _playerStateMachine.isGrounded);
        }
    }
}
