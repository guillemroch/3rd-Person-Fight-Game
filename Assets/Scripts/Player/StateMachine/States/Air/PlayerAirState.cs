namespace Player.StateMachine.States.Air{
    public class PlayerAirState : PlayerBaseState
    {
        public PlayerAirState(PlayerStateMachine currentCtx, PlayerStateFactory stateFactory)
            : base(currentCtx, stateFactory, "Air") {
            IsRootState = true;
            InitializeSubState();

        }
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
        
        
        }

        public override void InitializeSubState() {
            if (Ctx.InputManager.IsJumpPressed) {
                SwitchStates(Factory.Jump());
            }
            else {
                SwitchStates(Factory.Fall());
            }
        }
    }
}
