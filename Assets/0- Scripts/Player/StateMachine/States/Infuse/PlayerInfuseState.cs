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
            Ctx.IsInfusing = true;
            InitializeSubState();
            HandleInteraction();
            if (Ctx.InfusableSelectedObject == null) {
                Debug.Log("Returning as no object was found");
                //SwitchStates(Factory.Grounded());
            }
            else {
                Ctx.CameraManager.SetCameraMode(CameraManager.CameraMode.Infusing);
                Ctx.UIManager.SetKeyStates(InputsUIHelper.KeyUIStates.Infusing);
            }
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
            if (Ctx.InfusableSelectedObject != null) {
                Ctx.InfusableSelectedObject.Release();
                Ctx.InfusableSelectedObject = null;
            }
        }

        public override void CheckSwitchStates() {
            if (Ctx.InfusableSelectedObject.IsUnityNull()) {
                SwitchStates(Factory.Grounded());
            }

            if (Ctx.BreathedStormlight <= 0) {
                SwitchStates(Factory.Grounded());
            }
        }

        public override void InitializeSubState() {
            if (Ctx.IsGrounded) {
                SetSubStates(Factory.Grounded());
            }
        }
        
        private void HandleInteraction() {
            //1- Check interactable colliders in range
            Collider[] colliders = Physics.OverlapSphere(Ctx.PlayerTransform.position, Ctx.MaxInteractionDistance, Ctx.InteractionLayer);
            Infusable infusableObject = null;
        
            //2- If is PICKABLE, Interact
            foreach (var collider in colliders) {
                if (collider.gameObject.TryGetComponent(out Pickable interactableObject)) {
                    interactableObject.Interact(out int stormlight);
                    Ctx.Stormlight += stormlight;
                    break;
                }
                
                //3- If it is Infusable get the object
                collider.gameObject.TryGetComponent(out infusableObject);
            }

            if (Ctx.InputManager.InfuseInput) { 
               
                HandleInfusion(infusableObject);
            }

            //4- Release Object
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
                 infusable.SetMode(Ctx.InfusingMode);
                Ctx.InfusableSelectedObject.Interact(out int stormlightDrainadge);
                HandleInfusionInputs();
               
                Ctx.StormlightInfusingDrain = stormlightDrainadge*0.1f;
            }else {
                if (Ctx.InfusableSelectedObject != null) {
                   Ctx.InfusableSelectedObject.SetMode(Ctx.InfusingMode);
                    Ctx.InfusableSelectedObject.Interact(out int stormlightDrainadge);
                    HandleInfusionInputs();
                    
                    Ctx.StormlightInfusingDrain = stormlightDrainadge*0.1f;
                }
            }
        }

        private void HandleInfusionInputs() {
            if (Ctx.InputManager.LashInput) {
                Ctx.InfusableSelectedObject.AddLash();
                Ctx.InputManager.ResetLashInput();
            }
            
            if (Ctx.InputManager.UnLashInput) {
                Ctx.InfusableSelectedObject.UnLash();
                Ctx.InputManager.ResetUnLashInput();
            }

        }
    }
}