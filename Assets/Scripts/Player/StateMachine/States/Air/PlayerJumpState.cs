using UnityEngine;

namespace Player.StateMachine.States.Air{
    public class PlayerJumpState : PlayerBaseState {

        public PlayerJumpState(PlayerStateMachine currentCtx, PlayerStateFactory stateFactory)
            : base(currentCtx, stateFactory) {
        }

        public override void EnterState() {

            HandleJump();
        }

        public override void UpdateState() {
            CheckSwitchStates();
        }

        public override void FixedUpdateState() { }

        public override void ExitState() {
        
        }

        public override void CheckSwitchStates() {
            SwitchStates(Factory.Fall());
        }

        public override void InitializeSubState() { }

        void HandleJump() {
            Ctx.InputManager.ResetJumpInput();
            Ctx.AnimatorManager.animator.SetBool(Ctx.AnimatorManager.IsJumpingHash, true);
            Ctx.AnimatorManager.PlayTargetAnimation("Jump");

            float jumpingVelocity = Mathf.Sqrt(2 * Ctx.GravityIntensity  *  Ctx.JumpHeight);
            Ctx.PlayerRigidbody.AddForce(jumpingVelocity * -Ctx.GravityDirection, ForceMode.Impulse);
        }
    }
}

