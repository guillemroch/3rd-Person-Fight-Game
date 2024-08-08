using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player.StateMachine.States.Ground{

    public class PlayerDashState : PlayerBaseState{
        public PlayerDashState(PlayerStateMachine currentCtx, PlayerStateFactory stateFactory) :
            base(currentCtx, stateFactory, "Dash") { }
        public override void EnterState() {
            Ctx.InputManager.ResetDashInput();
            //Play animation
            Ctx.AnimatorManager.PlayTargetAnimation("Dash"); 
            //TODO: [Animation] -> set lash blend tree value
            Ctx.StormlightMovementDrain = 4;
        }

        public override void UpdateState() {

            HandleDash();
            CheckSwitchStates();
        }

        public override void FixedUpdateState() {
        }

        public override void ExitState() {
            Ctx.StormlightMovementDrain = 0;
        }

        public override void CheckSwitchStates() {
            SwitchStates(Factory.Grounded());
        }

        public override void InitializeSubState() {
        }

        private void HandleDash() {
            Vector3 dashDirection = Ctx.CameraObject.forward * Ctx.InputManager.MovementInput.y + Ctx.CameraObject.right * Ctx.InputManager.MovementInput.x;

            if (dashDirection == Vector3.zero)
                dashDirection = Ctx.PlayerTransform.forward;
            
            dashDirection.Normalize();
            Ctx.PlayerRigidbody.AddForce(dashDirection * Ctx.DashForce, ForceMode.Impulse);
        }
    }
}