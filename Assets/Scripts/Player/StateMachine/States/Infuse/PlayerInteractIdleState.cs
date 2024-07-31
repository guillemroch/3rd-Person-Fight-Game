using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player.StateMachine.States.Infuse{

    public class PlayerInteractIdleState : PlayerBaseState{
        public PlayerInteractIdleState(PlayerStateMachine currentCtx, PlayerStateFactory stateFactory) : 
            base(currentCtx, stateFactory, "Idle Interact") { }
        public override void EnterState() {
        }

        public override void UpdateState() {
            CheckSwitchStates();
        }

        public override void FixedUpdateState() {
            
        }

        public override void ExitState() {
        }

        public override void CheckSwitchStates() {
            if (Ctx.InputManager.MovementInput != Vector2.zero) {
                    SwitchStates(Factory.InteractWalk());
            }
            
            if (!Ctx.IsInfusing) {
                SwitchStates(Factory.Idle());
            }
        }

        public override void InitializeSubState() {
        }
    }
}