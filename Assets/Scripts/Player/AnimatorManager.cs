using Unity.Collections;
using UnityEngine;

namespace Player{
    public class AnimatorManager : MonoBehaviour
    {
        [SerializeField] [ReadOnly] public Animator animator;
        //Blend Tree variables
        [SerializeField] [ReadOnly] private int _horizontalHash;
        [SerializeField] [ReadOnly] private int _verticalHash;
        
        //Other variables
        [SerializeField] [ReadOnly] private int _isGroundedHash;
        [SerializeField] [ReadOnly] private int _isHalfLashingHash;
        [SerializeField] [ReadOnly] private int _isLashingHash;
        [SerializeField] [ReadOnly] private int _isJumpingHash;
        [SerializeField] [ReadOnly] private int _isFallingHash;
        
        public int IsGroundedHash => _isGroundedHash;
        public int IsHalfLashingHash => _isHalfLashingHash;
        public int IsLashingHash => _isLashingHash;
        public int IsJumpingHash => _isJumpingHash;
        public int IsFallingHash => _isFallingHash;


        private void Awake()
        {
            animator = GetComponent<Animator>();
            _horizontalHash = Animator.StringToHash("Horizontal");
            _verticalHash = Animator.StringToHash("Vertical");
            _isGroundedHash = Animator.StringToHash("isGrounded");
            _isHalfLashingHash = Animator.StringToHash("isHalfLashing");
            _isLashingHash = Animator.StringToHash("isLashing");
            _isJumpingHash = Animator.StringToHash("isJumping");
        }

        public void PlayTargetAnimation(string targetAnimation)
        {
            animator.CrossFade(targetAnimation, 0.2f);
        }
        public void UpdateAnimatorValues(Vector2 movementInput, bool isSprinting)
        {
            //Animation Snapping
            //Horizontal
            float snappedHorizontal;
            if ( movementInput.x is > 0 and < 0.55f)
            {
                snappedHorizontal = 0.5f;
            
            }else if ( movementInput.x >= 0.55f)
            {
                snappedHorizontal = 1f;
            }else if ( movementInput.x is < 0 and > -0.55f)
            {
                snappedHorizontal = -0.5f;
            
            }else if ( movementInput.x <= -0.55f)
            {
                snappedHorizontal = -1f;
            }
            else
            {
                snappedHorizontal = 0;
            }

            //Vertical
            float snappedVertical;
            if ( movementInput.y is > 0 and < 0.55f)
            {
                snappedVertical = 0.5f;
            
            }else if ( movementInput.y >= 0.55f)
            {
                snappedVertical = 1f;
            }else if ( movementInput.y is < 0 and > -0.55f)
            {
                snappedVertical = -0.5f;
            
            }else if ( movementInput.y <= -0.55f)
            {
                snappedVertical = -1f;
            }else
            {
                snappedVertical = 0;
            }

            if (isSprinting)
            {
                snappedVertical = 2;
            
            }
        
        
            animator.SetFloat(_horizontalHash, snappedHorizontal, 0.1f, Time.deltaTime);
            animator.SetFloat(_verticalHash, snappedVertical, 0.1f, Time.deltaTime);
        }
    }
}
