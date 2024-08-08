using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Player.StateMachine.States.Infuse{

    public class PlayerInfuseState : PlayerBaseState{
        public PlayerInfuseState(PlayerStateMachine currentCtx, PlayerStateFactory stateFactory) :
            base(currentCtx, stateFactory, "Infuse") {
            IsRootState = true;
        }
        
        public override void EnterState() {
            Ctx.CameraManager.SetCameraMode(CameraManager.CameraMode.Infusing);
            Ctx.IsInfusing = true;
            InitializeSubState();
        }

        public override void UpdateState() {
            HandleInteraction();
            CheckSwitchStates();
        }

        public override void FixedUpdateState() {
        }

        public override void ExitState() {
            Ctx.IsInfusing = false;
            Ctx.StormlightInfusingDrain = 0;
        }

        public override void CheckSwitchStates() {
            if (Ctx.InfusableSelectedObject.IsUnityNull()) {
                SwitchStates(Factory.Grounded());
            }
        }

        public override void InitializeSubState() {
            if (Ctx.IsGrounded) {
                SetSubStates(Factory.Grounded());
            }
        }
        
        private void HandleInteraction() {
            Collider[] colliders = Physics.OverlapSphere(Ctx.PlayerTransform.position, Ctx.MaxInteractionDistance, Ctx.InteractionLayer);
            Infusable infusableObject = null;
        
            foreach (var collider in colliders) {
                if (collider.gameObject.TryGetComponent(out Pickable interactableObject)) {
                    interactableObject.Interact(out int stormlight);
                    Ctx.Stormlight += stormlight;
                    break;
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
                Ctx.StormlightInfusingDrain = stormlightDrainadge*0.1f;
            }
            else {
                if (Ctx.InfusableSelectedObject != null) {
                    Ctx.InfusableSelectedObject.Interact(out int stormlightDrainadge);
                    Ctx.StormlightInfusingDrain = stormlightDrainadge*0.1f;
                }
            }
                
            //TODO: Activate Animation
            //TODO: Control distance using wheel or something idk
                    
        }
    }
}