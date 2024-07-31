using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player.StateMachine.States.Infuse{
    public class PlayerInteractingWalkState : PlayerBaseState{
        public PlayerInteractingWalkState(PlayerStateMachine currentCtx, PlayerStateFactory stateFactory) :
            base(currentCtx, stateFactory, "Interacting Walk") { }
        public override void EnterState() {
        }

        public override void UpdateState() {
           CheckSwitchStates(); 
            HandleMovement();
        }

        public override void FixedUpdateState() {
        }

        public override void ExitState() {
        }

        public override void CheckSwitchStates() {
            if (Ctx.InputManager.MovementInput == Vector2.zero) {
                SwitchStates(Factory.InteractIdleState());
            }

            if (!Ctx.IsInfusing) {
                SwitchStates(Factory.Idle());
            }
        }

        public override void InitializeSubState() {
        }
        
        private void HandleMovement() {
            Ctx.MoveDirection = Ctx.CameraObject.forward * Ctx.InputManager.MovementInput.y + Ctx.CameraObject.right * Ctx.InputManager.MovementInput.x;
        
            float moveDot = Vector3.Dot(Ctx.MoveDirection, Ctx.GravityDirection);
            float magSquared = Ctx.GravityDirection.sqrMagnitude;
            
            Vector3 projection = (moveDot / magSquared) * Ctx.GravityDirection;
            Ctx.MoveDirection += -projection;
            Ctx.MoveDirection.Normalize();
                
            Ctx.PlayerRigidbody.AddForce(Ctx.MoveDirection * Ctx.WalkingSpeed, ForceMode.Force);
        }
    }
}