using System.Collections;
using System.Collections.Generic;
using Player.StateMachine;
using UnityEngine;

namespace Player.StateMachine.States.Lash{

    public class PlayerLashDashState : PlayerBaseState{

        public PlayerLashDashState(PlayerStateMachine currentCtx, PlayerStateFactory stateFactory) : base(
            currentCtx, stateFactory, "Lash Dash") { }

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
