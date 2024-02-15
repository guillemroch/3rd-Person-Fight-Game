namespace Player.StateMachine.States.Air{
    public class PlayerLandState : PlayerBaseState
    {
        public PlayerLandState(PlayerStateMachine currentCtx, PlayerStateFactory stateFactory) : base(currentCtx, stateFactory) { }
        public override void EnterState() {

            Ctx.AnimatorManager.PlayTargetAnimation("Land");
        }

        public override void UpdateState() {
            CheckSwitchStates();
        }

        public override void FixedUpdateState() {
        }

        public override void ExitState() {

        }

        public override void CheckSwitchStates() {
            SwitchStates(Factory.Grounded());
        }

        public override void InitializeSubState() {
        }
    }
}
