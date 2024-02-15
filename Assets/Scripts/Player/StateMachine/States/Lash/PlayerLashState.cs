namespace Player.StateMachine.States.Lash{
    public class PlayerLashState : PlayerBaseState{
        public PlayerLashState(PlayerStateMachine currentCtx, PlayerStateFactory stateFactory) : base(currentCtx,
            stateFactory) {
            IsRootState = true;
            InitializeSubState();
        }

        public override void EnterState() { }

        public override void UpdateState() {
            //TODO: add stamina
        }

        public override void FixedUpdateState() { }

        public override void ExitState() { }

        public override void CheckSwitchStates() { }

        public override void InitializeSubState() {
            SwitchStates(Factory.Halflash());
        }
    }
}
