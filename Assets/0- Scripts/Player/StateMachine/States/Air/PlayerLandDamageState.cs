using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player.StateMachine.States.Air{
    public class PlayerLandDamageState : PlayerBaseState{
        public PlayerLandDamageState(PlayerStateMachine currentCtx, PlayerStateFactory stateFactory) : base(
            currentCtx, stateFactory, "LandDammage") { }

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