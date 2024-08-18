namespace Player.StateMachine.States.Lash{
    //Root State of the Lash States
    public class PlayerLashingsState : PlayerBaseState{
        public PlayerLashingsState(PlayerStateMachine currentCtx, PlayerStateFactory stateFactory) : base(currentCtx,
            stateFactory, "Lashings") {
            IsRootState = true;
            InitializeSubState();
        }

        public override void EnterState() {
            Ctx.UIManager.SetKeyStates(InputsUIHelper.KeyUIStates.Lashings);
        }

        public override void UpdateState() {
            //Check if we need to change state because of stormlight
            CheckSwitchStates();
        }

        public override void FixedUpdateState() { }

        public override void ExitState() {
            Ctx.IsLashing = false;
            Ctx.IsHalfLashing = false;
            Ctx.StormlightLashingDrain = 0;
        }

        public override void CheckSwitchStates() {
            if (!Ctx.IsUsingStormlight) {
                SwitchStates(Factory.Grounded());
            }
        }

        public override void InitializeSubState() {
            if (Ctx.InputManager.LashInput ) {
                SetSubStates(Factory.Halflash());
            }

            if (Ctx.InputManager.SmallLashInput > 0) {
                SetSubStates(Factory.Lash());
            }
        }
    }
}
