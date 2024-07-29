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
            Ctx.ParticleSystem.enabled = true;
        }

        public override void UpdateState() {
            HandleStamina();
            HandleInteraction();
            CheckSwitchStates();
        }

        public override void FixedUpdateState() { }

        public override void ExitState() {
            Ctx.IsUsingStormlight = false;
            Ctx.GravityDirection = Vector3.down;
            Ctx.LashingIntensity = 0;
            Ctx.ParticleSystem.enabled = false;
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
        private void HandleInteraction() {
            Collider[] colliders = Physics.OverlapSphere(Ctx.PlayerTransform.position, Ctx.MaxInteractionDistance, Ctx.InteractionLayer);
            Infusable infusableObject = null;
        
            foreach (var collider in colliders) {
                if (collider.gameObject.TryGetComponent(out Pickable interactableObject)) {
                    interactableObject.Interact(out int stormlight);
                    Ctx.Stormlight += stormlight;
                }
                collider.gameObject.TryGetComponent(out infusableObject);
            }

            if (Ctx.InputManager.InfuseInput) { 
                HandleInfusion(infusableObject);
            }

            if (Ctx.InfusableSelectedObject != null && !Ctx.InputManager.InfuseInput) {
                Ctx.InfusableSelectedObject.Release();
                Ctx.InfusableSelectedObject = null;
            }
        }

        private void HandleInfusion(Infusable infusable) {
            if (infusable != null) {
                if (Ctx.InfusableSelectedObject == null) {
                    Ctx.InfusableSelectedObject = infusable;
                }
                Ctx.InfusableSelectedObject.Interact(out int stormlightDrainadge);
                Ctx.Stormlight -= stormlightDrainadge*0.1f;
            }
            else {
                if (Ctx.InfusableSelectedObject != null) {
                    Ctx.InfusableSelectedObject.Interact(out int stormlightDrainadge);
                    Ctx.Stormlight -= stormlightDrainadge*0.1f;
                }
            }
        
            //TODO: Activate Animation
            //TODO: Control distance using wheel or something idk
            
        }

    }
}
