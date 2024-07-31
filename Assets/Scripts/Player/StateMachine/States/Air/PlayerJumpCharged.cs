using System.Collections;
using System.Collections.Generic;
using Player.StateMachine;
using UnityEngine;

namespace Player.StateMachine.States.Air{
    public class PlayerJumpCharged : PlayerBaseState{
        public PlayerJumpCharged(PlayerStateMachine currentCtx, PlayerStateFactory stateFactory) : base(
            currentCtx, stateFactory, "ChargedJump") { }

        public override void EnterState() {
        }

        public override void UpdateState() {
        }

        public override void FixedUpdateState() {
        }

        public override void ExitState() {
        }

        public override void CheckSwitchStates() {
        }

        public override void InitializeSubState() {
        }
    }
}