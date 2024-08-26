using UnityEngine;

namespace Player.StateMachine.States.Air{
    public class PlayerAirState : PlayerBaseState
    {
        public PlayerAirState(PlayerStateMachine currentCtx, PlayerStateFactory stateFactory)
            : base(currentCtx, stateFactory, "Air") {
            IsRootState = true;
            InitializeSubState();
        }
        public override void EnterState() {
            Ctx.IsLashing = false;
            Ctx.IsHalfLashing = false;
        }

        public override void UpdateState() {
            CheckSwitchStates();
            HandleRotation();
            HandleFalling();
            HandleGravity();
        }

        public override void FixedUpdateState() {
        }

        public override void ExitState() {
        }

        public override void CheckSwitchStates() {
        
        
        }

        public override void InitializeSubState() {
            if (Ctx.InputManager.IsJumpPressed) {
                SetSubStates(Factory.Jump());
            }
            else {
                SetSubStates(Factory.Fall());
            }
        }
        
        private void HandleRotation()
        {
        
            Ctx.TargetDirection = Vector3.zero; //Resets target direction
        
            //calculate orientation based on camera position
            Ctx.TargetDirection = Ctx.CameraObject.forward * Ctx.InputManager.MovementInput.y +
                                  Ctx.CameraObject.right * Ctx.InputManager.MovementInput.x;
        
            float moveDot = Vector3.Dot(Ctx.TargetDirection, Ctx.GravityDirection);
            float magSquared = Ctx.GravityDirection.sqrMagnitude;
    
            Vector3 projection = (moveDot / magSquared) * Ctx.GravityDirection;
            Ctx.TargetDirection += -projection;
            Ctx.TargetDirection.Normalize();
        
            if (Ctx.TargetDirection == Vector3.zero)
                Ctx.TargetDirection = Ctx.PlayerTransform.forward;

            if (Ctx.GravityDirection == Vector3.zero) {
                Ctx.GravityDirection = Vector3.down;
                Debug.Log("Gravity was zero while falling");
            }

            Quaternion targetRotation = 
                Quaternion.LookRotation(Ctx.TargetDirection, -Ctx.GravityDirection);

            Ctx.transform.rotation = 
                Quaternion.Slerp(Ctx.PlayerTransform.rotation, targetRotation, Ctx.RotationSpeed * Time.deltaTime);
        
        }
        void HandleFalling() {
                
            if (Ctx.InAirTimer <= Ctx.MaxAirSpeed) Ctx.InAirTimer += Time.deltaTime;
                
            Ctx.PlayerRigidbody.AddForce(Ctx.PlayerTransform.forward * (Ctx.LeapingVelocity * Ctx.PlayerRigidbody.velocity.magnitude), ForceMode.Force);
            Ctx.PlayerRigidbody.AddForce(Ctx.GravityDirection * (Ctx.FallingVelocity * Ctx.InAirTimer), ForceMode.Force);
                
            Ctx.PlayerRigidbody.AddForce(Ctx.GravityIntensity * Ctx.GravityMultiplier * Ctx.GravityDirection, ForceMode.Acceleration);
        
            Vector3 rayCastOrigin = Ctx.PlayerRigidbody.worldCenterOfMass;
            rayCastOrigin += Ctx.PlayerTransform.up * Ctx.RayCastHeightOffset;
                    
            if (Physics.SphereCast(rayCastOrigin, Ctx.RayCastRadius, Ctx.GravityDirection, out _, Ctx.RayCastMaxDistance,
                    Ctx.GroundLayer)) {
                Ctx.IsGrounded = true;
                if (Ctx.PlayerRigidbody.velocity.magnitude > 10f) {
                    Ctx.Dammage(Ctx.PlayerRigidbody.velocity.magnitude);
                }
            }
        
        }
        
        void HandleGravity() {
            Ctx.PlayerRigidbody.AddForce(Ctx.GravityIntensity * Ctx.GravityMultiplier * Ctx.GravityDirection, ForceMode.Acceleration);
        
        }
    }
}
