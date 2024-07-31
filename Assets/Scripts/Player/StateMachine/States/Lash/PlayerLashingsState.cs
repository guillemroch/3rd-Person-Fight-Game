namespace Player.StateMachine.States.Lash{
    //Root State of the Lash States
    public class PlayerLashingsState : PlayerBaseState{
        public PlayerLashingsState(PlayerStateMachine currentCtx, PlayerStateFactory stateFactory) : base(currentCtx,
            stateFactory, "Lashings") {
            IsRootState = true;
            InitializeSubState();
        }

        public override void EnterState() { }

        public override void UpdateState() {
        }

        public override void FixedUpdateState() { }

        public override void ExitState() { }

        public override void CheckSwitchStates() { }

        public override void InitializeSubState() {
            if (Ctx.InputManager.LashInput ) {
                SetSubStates(Factory.Halflash());
            }

            if (Ctx.InputManager.SmallLashInput > 0) {
                SetSubStates(Factory.LashDive());
            }
        }
    }
}
