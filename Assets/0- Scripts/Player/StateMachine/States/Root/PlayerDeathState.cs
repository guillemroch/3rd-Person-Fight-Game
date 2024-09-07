using System.Collections;
using UnityEngine;

namespace Player.StateMachine.States.Root{
    public class PlayerDeathState : PlayerBaseState {
    
        public PlayerDeathState(PlayerStateMachine currentCtx, PlayerStateFactory stateFactory)
            : base(currentCtx, stateFactory, "Death") {
            IsRootState = true; 
            InitializeSubState();
        }

        public override void EnterState() {
            Ctx.AnimatorManager.PlayTargetAnimation("Death");
        }

        public override void UpdateState() {
            CheckSwitchStates();
        }

        public override void FixedUpdateState() { }

        public override void ExitState() {
           
        }

        public override void CheckSwitchStates() {
            if (Ctx.Stormlight == 0 ) {
            } 
        }

        public override void InitializeSubState() {
            
        }

    }
}
