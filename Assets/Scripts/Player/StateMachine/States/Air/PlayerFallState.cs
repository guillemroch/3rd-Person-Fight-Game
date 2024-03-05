using UnityEngine;

namespace Player.StateMachine.States.Air{
    public class PlayerFallState : PlayerBaseState
    {
        public PlayerFallState(PlayerStateMachine currentCtx, PlayerStateFactory stateFactory) : base(currentCtx, stateFactory) { }
        public override void EnterState() {

            Ctx.AnimatorManager.PlayTargetAnimation("Fall");
        }

        public override void UpdateState() {
            CheckSwitchStates();
            HandleFalling();
            HandleGravity();
        
        }

        public override void FixedUpdateState() {
        }

        public override void ExitState() {
        }

        public override void CheckSwitchStates() {
            if (Ctx.isGrounded) SwitchStates(Factory.Land());
            if (Ctx.InputManager.LashInput || Ctx.InputManager.SmallLashInput > 0) SwitchStates(Factory.Lash());
        }

        public override void InitializeSubState() {
        }

        void HandleFalling() {
        
            if (Ctx.InAirTimer <= Ctx.MaxAirSpeed) Ctx.InAirTimer += Time.deltaTime;
        
            Ctx.PlayerRigidbody.AddForce(Ctx.PlayerTransform.forward * (Ctx.LeapingVelocity * Ctx.PlayerRigidbody.velocity.magnitude), ForceMode.Force);
            Ctx.PlayerRigidbody.AddForce(Ctx.GravityDirection * (Ctx.FallingVelocity * Ctx.InAirTimer), ForceMode.Force);
        
            Ctx.PlayerRigidbody.AddForce(Ctx.GravityIntensity * Ctx.GravityMultiplier * Ctx.GravityDirection, ForceMode.Acceleration);

            Vector3 rayCastOrigin = Ctx.PlayerRigidbody.worldCenterOfMass;
            
            if (Physics.SphereCast(rayCastOrigin, Ctx.RayCastRadius, Ctx.GravityDirection, out _, Ctx.RayCastMaxDistance,
                    Ctx.GroundLayer)) {
                Ctx.isGrounded = true;
            }

        }

        void HandleGravity() {
            Ctx.PlayerRigidbody.AddForce(Ctx.GravityIntensity * Ctx.GravityMultiplier * Ctx.GravityDirection, ForceMode.Acceleration);

        }
    }
}
