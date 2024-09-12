using UnityEngine;

namespace Player.StateMachine.States.Alive{
    public class PlayerAliveState : PlayerBaseState {
    
        public PlayerAliveState(PlayerStateMachine currentCtx, PlayerStateFactory stateFactory)
            : base(currentCtx, stateFactory, "Alive") {
            IsRootState = true;
            InitializeSubState();
        }
        public override void EnterState() {
        }

        public override void UpdateState() {
            HandleInteraction();
            HandleStormlight();
            CheckSwitchStates();
        }

        public override void FixedUpdateState() { }

        public override void ExitState() {
        }

        public override void CheckSwitchStates() {
            //TODO: If health == 0 => die
            /*if (Ctx.InputManager.StormlightInput ) {
                //SwitchStates(Factory.Stormlight());
            }*/
            if (Ctx.Health <= 0) {
                SwitchStates(Factory.Death());
            }
            if (Ctx.InputManager.StormlightInput) {
                Ctx.InputManager.ResetStormlightInput();
                
                if (Ctx.Stormlight > 0) {
                    Ctx.IsUsingStormlight = true;
                    
                     Ctx.AnimatorManager.PlayTargetAnimation("Buff");
                     Ctx.ParticleSystem.Play();

                    if (Ctx.Stormlight - Ctx.StormlightBreathConsumption < 0) {
                        Ctx.BreathedStormlight += Ctx.Stormlight;
                        Ctx.Stormlight = 0;
                    }
                    else {
                        Ctx.Stormlight -= Ctx.StormlightBreathConsumption;
                        Ctx.BreathedStormlight += Ctx.StormlightBreathConsumption;
                    }
                }
            }
        }

        public override void InitializeSubState() {
            SetSubStates(Factory.Grounded());
        }

        private void HandleInteraction() {
            Collider[] colliders = Physics.OverlapSphere(Ctx.PlayerTransform.position, Ctx.MaxInteractionDistance, Ctx.InteractionLayer);

            foreach (var collider in colliders) {
                if (collider.gameObject.TryGetComponent(out Pickable interactableObject)) {
                    interactableObject.Interact(out int stormlight);
                    Ctx.Stormlight += stormlight;
                    if (Ctx.Stormlight > 100f) {
                        Ctx.Stormlight = 100;
                    }

                    continue;
                }

                if (collider.gameObject.TryGetComponent(out Infusable infusable)) {
                    infusable.ActivateOverlay();
                }
            }

            //Handling infuse mode changes
            if (Ctx.InputManager.ScrollInput != 0) {
                //Debug.Log("Switched infuse mode: " + Ctx.InputManager.ScrollInput);
                int index = (int)Ctx.InfusingMode;
                index += (int) Ctx.InputManager.ScrollInput;
                if (index > 2)
                    index = 0;
                if (index < 0)
                    index = 2;
                Ctx.InfusingMode = (Infusable.InfusingMode) index;
                Ctx.UIManager.SwitchInfuseModes(Ctx.InfusingMode);
                Ctx.InputManager.ScrollInput = 0;
            }
        }
        
        private void HandleStormlight() {
            if (!Ctx.IsUsingStormlight)
                return;
            
            //UPDATE STORMLIGHT STATE
            //1- Calculate stormlight drain
            Ctx.StormlightDepletionRate = Ctx.StormlightBaseDrain +
                                          Ctx.StormlightHealingDrain +
                                          Ctx.StormlightInfusingDrain +
                                          Ctx.StormlightLashingDrain * 0.1f+
                                          Ctx.StormlightMovementDrain;
           
            Ctx.BreathedStormlight -= Ctx.StormlightDepletionRate * 0.1f;
            if (Ctx.Stormlight < 0) Ctx.Stormlight = 0;
            if (Ctx.BreathedStormlight <= 0) {
                Ctx.BreathedStormlight = 0;
                Ctx.IsUsingStormlight = false;
                Ctx.GravityDirection = Vector3.down;
                Ctx.ParticleSystem.Stop();
            }

            //Healing
            if (Ctx.BreathedStormlight > 0 && Ctx.Health < Ctx.MaxHealth) {
                Ctx.BreathedStormlight-= 0.1f;
                Ctx.Health++;
            }
        }

       
    }
}
