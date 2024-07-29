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
            HandleInteraction();
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

        private void HandleInteraction() {
            Collider[] colliders = Physics.OverlapSphere(Ctx.PlayerTransform.position, Ctx.MaxInteractionDistance, Ctx.InteractionLayer);

            foreach (var collider in colliders) {
                if (collider.gameObject.TryGetComponent(out Pickable interactableObject)) {
                    interactableObject.Interact(out int stormlight);
                    Ctx.Stormlight += stormlight;
                }
            }
        }

       
    }
}
