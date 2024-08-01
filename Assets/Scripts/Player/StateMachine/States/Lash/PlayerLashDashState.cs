using System.Collections;
using System.Collections.Generic;
using Player.StateMachine;
using UnityEngine;

namespace Player.StateMachine.States.Lash{

    public class PlayerLashDashState : PlayerBaseState{

        public PlayerLashDashState(PlayerStateMachine currentCtx, PlayerStateFactory stateFactory) : base(
            currentCtx, stateFactory, "Lash Dash") { }

        public override void EnterState() {
            Ctx.InputManager.ResetDashInput();
        }

        public override void UpdateState() {
            HandleDash();
                        CheckSwitchStates();
        }

        public override void FixedUpdateState() {
        }

        public override void ExitState() {
        }

        public override void CheckSwitchStates() {
            SwitchStates(Factory.Halflash());
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
