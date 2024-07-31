using System.Collections;
using System.Collections.Generic;
using Player.StateMachine;
using UnityEngine;
namespace Player.StateMachine.States.Attack{
    public class PlayerAirLightAttackState : PlayerBaseState{
        public PlayerAirLightAttackState(PlayerStateMachine currentCtx, PlayerStateFactory stateFactory) :
            base(currentCtx, stateFactory, "AirLightAttack") { }
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

