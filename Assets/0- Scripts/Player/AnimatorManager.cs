using System;
using Unity.Collections;
using UnityEngine;

namespace Player{
    public class AnimatorManager : MonoBehaviour
    {
        [SerializeField] [ReadOnly] public Animator animator;
       
        [Header("Variable Hashes")]
        //Blend Tree variables
        [SerializeField] [ReadOnly] private int _velocityXHash;
        [SerializeField] [ReadOnly] private int _velocityYHash;
        [SerializeField] [ReadOnly] private int _velocityZHash;
        //Other variables
        [SerializeField] [ReadOnly] private int _isGroundedHash;
        [SerializeField] [ReadOnly] private int _isHalfLashingHash;
        [SerializeField] [ReadOnly] private int _isLashingHash;
        [SerializeField] [ReadOnly] private int _isJumpingHash;
        [SerializeField] [ReadOnly] private int _isInteractingHash;
        [SerializeField] [ReadOnly] private int _isFallingHash;
        [SerializeField] [ReadOnly] private int _lookYHash;
        [SerializeField] [ReadOnly] private int _diveAngleHash;

        [Header("Variables Values")]
        [SerializeField] private float _velocityX;
        [SerializeField] private float _velocityY;
        [SerializeField] private float _velocityZ;
        [SerializeField] private bool _isGrounded; 
        [SerializeField] private bool _isLashing; 
        [SerializeField] private bool _isHalfLashing; 
        [SerializeField] private bool _isInteracting;
        [SerializeField] private bool _isFalling;
        [SerializeField] private float _lookY;
        [SerializeField] private float _diveAngle;

        public float VelocityX { get => _velocityX; set => _velocityX = value; }
        public float VelocityY { get => _velocityY; set => _velocityY = value; }
        public float VelocityZ { get => _velocityZ; set => _velocityZ = value; }
        public bool IsGrounded { get => _isGrounded; set => _isGrounded = value; }
        public bool IsLashing { get => _isLashing; set => _isLashing = value; }
        public bool IsHalfLashing { get => _isHalfLashing; set => _isHalfLashing = value; }
        public bool IsInteracting { get => _isInteracting; set => _isInteracting = value; }
        public bool IsFalling { get => _isFalling; set => _isFalling = value; }
        public float LookY { get => _lookY; set => _lookY = value; }
        public float DiveAngle { get => _diveAngle; set => _diveAngle = value; }

        private void Awake()
        {
            //animator = GetComponent<Animator>();
            _velocityXHash = Animator.StringToHash("VelocityX");
            _velocityYHash = Animator.StringToHash("VelocityY");
            _velocityZHash = Animator.StringToHash("VelocityZ");
            _isGroundedHash = Animator.StringToHash("IsGrounded");
            _isHalfLashingHash = Animator.StringToHash("IsHalfLashing");
            _isLashingHash = Animator.StringToHash("IsLashing");
            _isJumpingHash = Animator.StringToHash("IsJumping");
            _isInteractingHash = Animator.StringToHash("IsInteracting");
            _isFallingHash = Animator.StringToHash("IsFalling");
            _lookYHash = Animator.StringToHash("LookY");
            _diveAngleHash = Animator.StringToHash("DiveAngle");
        }

        public void UpdateValues() {
            animator.SetFloat(_velocityXHash, _velocityX, 0.2f, Time.deltaTime);
            animator.SetFloat(_velocityYHash, _velocityY, 0.2f, Time.deltaTime);
            animator.SetFloat(_velocityZHash, _velocityZ, 0.2f, Time.deltaTime);
            animator.SetBool(_isGroundedHash, _isGrounded);
            animator.SetBool(_isHalfLashingHash, _isHalfLashing);
            animator.SetBool(_isLashingHash, _isLashing);
            animator.SetBool(_isInteractingHash, _isInteracting);
            animator.SetBool(_isFallingHash, _isFalling);
            animator.SetFloat(_lookYHash, _lookY);
           
            animator.SetLayerWeight( animator.GetLayerIndex("Interaction"), _isInteracting ? 1 : 0); 
            //animator.SetBool(_isInteractingHash, _ velocityX);
            animator.SetFloat(_diveAngleHash, _diveAngle, 0.2f, Time.deltaTime);
        }

        public void PlayTargetAnimation(string targetAnimation)
        {
            animator.CrossFade(targetAnimation, 0.1f);
        }
        public void UpdateVelocityValues(Vector2 movementInput, bool isSprinting)
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
        
        
            //animator.SetFloat(_horizontalHash, snappedHorizontal, 0.1f, Time.deltaTime);
            //animator.SetFloat(_verticalHash, snappedVertical, 0.1f, Time.deltaTime);
        }
    }
}
