using UnityEngine;

namespace Player.StateMachine.States.Ground{
    public class PlayerIdleState : PlayerBaseState
    {
        public PlayerIdleState(PlayerStateMachine currentCtx, PlayerStateFactory stateFactory) : base(currentCtx, stateFactory, "Idle") { }
        public override void EnterState() {

        }

        public override void UpdateState() {
            CheckSwitchStates();
        }

        public override void FixedUpdateState() {    }

        public override void ExitState() {    }

        public override void CheckSwitchStates() {
            if (Ctx.InputManager.MovementInput != Vector2.zero && Ctx.InputManager.IsSprintPressed) {
                SwitchStates(Factory.Sprint());
            }
            else if (Ctx.InputManager.MovementInput != Vector2.zero && !Ctx.InputManager.IsSprintPressed) {
                if (Ctx.InputManager.MoveAmount <= 0.5f) {

                    SwitchStates(Factory.Walk());
                }else {
                    SwitchStates(Factory.Run());
                }
            }
        }

        public override void InitializeSubState() {    }
    }
}
