using UnityEngine;

namespace Player.StateMachine.States.Air{
    public class PlayerFallState : PlayerBaseState
    {
        public PlayerFallState(PlayerStateMachine currentCtx, PlayerStateFactory stateFactory) : base(currentCtx, stateFactory, "Fall") { }
        public override void EnterState() {

            Ctx.IsFalling = true;
            //Ctx.GravityDirection = Vector3.down;
        }

        public override void UpdateState() {
           
            //HandleFalling();
            //HandleGravity();
            HandleMovement();
             CheckSwitchStates();
        
        }

        public override void FixedUpdateState() {
        }

        public override void ExitState() {
            Ctx.IsFalling = false;
        }

        public override void CheckSwitchStates() {
            if (Ctx.IsGrounded)
                SwitchStates(Factory.Land());
            
            if (!Ctx.IsUsingStormlight) {
                            return;
            }
            if (Ctx.InputManager.LashInput || Ctx.InputManager.SmallLashInput > 0)
                SwitchStates(Factory.Lashings());
        }

        public override void InitializeSubState() {
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
            }

        }

        void HandleGravity() {
            Ctx.PlayerRigidbody.AddForce(Ctx.GravityIntensity * Ctx.GravityMultiplier * Ctx.GravityDirection, ForceMode.Acceleration);

        }
        private void HandleMovement() {
            Ctx.MoveDirection = Ctx.CameraObject.forward * Ctx.InputManager.MovementInput.y + Ctx.CameraObject.right * Ctx.InputManager.MovementInput.x;

            float moveDot = Vector3.Dot(Ctx.MoveDirection, Ctx.GravityDirection);
            float magSquared = Ctx.GravityDirection.sqrMagnitude;
    
            Vector3 projection = (moveDot / magSquared) * Ctx.GravityDirection;
            Ctx.MoveDirection += -projection;
            Ctx.MoveDirection.Normalize();
        
            Ctx.PlayerRigidbody.AddForce(Ctx.MoveDirection * Ctx.RunningSpeed, ForceMode.Force);
        }
    }
}
