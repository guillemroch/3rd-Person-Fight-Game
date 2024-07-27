using UnityEngine;

namespace Player.StateMachine.States.Normal{
    public class PlayerNormalState : PlayerBaseState {
    
        public PlayerNormalState(PlayerStateMachine currentCtx, PlayerStateFactory stateFactory)
            : base(currentCtx, stateFactory, "Normal") {
            IsRootState = true;
            InitializeSubState();
        }
        public override void EnterState() {
        }

        public override void UpdateState() {
            CheckSwitchStates();
        }

        public override void FixedUpdateState() { }

        public override void ExitState() {
        }

        public override void CheckSwitchStates() {
            if (Ctx.InputManager.StormlightInput ) {
                SwitchStates(Factory.Stormlight());
            }
        }

        public override void InitializeSubState() {
            SetSubStates(Factory.Grounded());
        }

       
    }
}
