using UnityEngine;

namespace Player.StateMachine.States.Stormlight{
    public class PlayerStormlightState : PlayerBaseState {
    
        public PlayerStormlightState(PlayerStateMachine currentCtx, PlayerStateFactory stateFactory)
            : base(currentCtx, stateFactory, "Stormlight") {
            IsRootState = true; 
            InitializeSubState();
        }

        public override void EnterState() {
            Ctx.IsUsingStormlight = true;
            Ctx.InputManager.ResetStormlightInput();
        }

        public override void UpdateState() {
            HandleStamina();
            CheckSwitchStates();
        }

        public override void FixedUpdateState() { }

        public override void ExitState() {
            Ctx.IsUsingStormlight = false;
        }

        public override void CheckSwitchStates() {
            if (Ctx.Stormlight == 0 ) {
               SwitchStates(Factory.Normal()); 
            } 
        }

        public override void InitializeSubState() {
            SetSubStates(Factory.Grounded());
        }
        private void HandleStamina() {
            Ctx.Stormlight -= Ctx.StormlightDepletionRate;
            if (Ctx.Stormlight < 0) Ctx.Stormlight = 0;
                     
            Ctx.UIManager.StormlightBar.Set(Ctx.Stormlight);
        }

    }
}
