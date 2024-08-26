using UnityEngine;

namespace Player.StateMachine.States.Root{
    public class PlayerDeathState : PlayerBaseState {
    
        public PlayerDeathState(PlayerStateMachine currentCtx, PlayerStateFactory stateFactory)
            : base(currentCtx, stateFactory, "Death") {
            IsRootState = true; 
            InitializeSubState();
        }

        public override void EnterState() {
            Ctx.IsUsingStormlight = true;
            Ctx.InputManager.ResetStormlightInput();
            //Ctx.ParticleSystem.enabled = true;
        }

        public override void UpdateState() {
            HandleStamina();
            CheckSwitchStates();
        }

        public override void FixedUpdateState() { }

        public override void ExitState() {
            Ctx.IsUsingStormlight = false;
            Ctx.GravityDirection = Vector3.down;
            Ctx.LashingIntensity = 0;
            //Ctx.ParticleSystem.enabled = false;
        }

        public override void CheckSwitchStates() {
            if (Ctx.Stormlight == 0 ) {
               //SwitchStates(Factory.Normal()); 
            } 
        }

        public override void InitializeSubState() {
            SetSubStates(Factory.Grounded());
        }
        private void HandleStamina() {
            Ctx.Stormlight -= Ctx.StormlightDepletionRate;
            if (Ctx.Stormlight < 0) Ctx.Stormlight = 0;
                     
            Ctx.UIManager.StormlightBar.Set(Ctx.Stormlight, Ctx.BreathedStormlight);
        }


 

    }
}
