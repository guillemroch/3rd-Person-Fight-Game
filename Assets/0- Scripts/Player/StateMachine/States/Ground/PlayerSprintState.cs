using UnityEngine;

namespace Player.StateMachine.States.Ground{
    public class PlayerSprintState : PlayerBaseState
    {
        public PlayerSprintState(PlayerStateMachine currentCtx, PlayerStateFactory stateFactory) : base(currentCtx, stateFactory, "Sprint") { }
        public override void EnterState() {
            Ctx.StormlightMovementDrain = 1;

        }

        public override void UpdateState() {
            HandleMovement();
            CheckSwitchStates();
        }

        public override void FixedUpdateState() {
        }

        public override void ExitState() {
        }

        public override void CheckSwitchStates() {
            if (Ctx.InputManager.DashInput) {
                SwitchStates(Factory.Dash());
            }
            if (Ctx.InputManager.MovementInput == Vector2.zero) {
                SwitchStates(Factory.Idle());
            }
            else if (!Ctx.InputManager.IsSprintPressed) {
                if (Ctx.InputManager.MoveAmount <= 0.5f) {
                    SwitchStates(Factory.Walk());
                }else {
                    SwitchStates(Factory.Run());
                }
            }
        }

        public override void InitializeSubState() {
        }
    
    
        private void HandleMovement() {
               Vector3 planeNormal = Vector3.Cross(Ctx.PlayerTransform.right, Ctx.PlayerTransform.forward);
            Vector3 projectedCameraForward = Vector3.ProjectOnPlane(Ctx.CameraObject.forward, planeNormal);
            Vector3 projectedCameraRight = Vector3.ProjectOnPlane(Ctx.CameraObject.right, planeNormal);
            Ctx.MoveDirection = projectedCameraForward * Ctx.InputManager.MovementInput.y +projectedCameraRight * Ctx.InputManager.MovementInput.x;
            Ctx.MoveDirection.Normalize();
        
            Ctx.PlayerRigidbody.AddForce(Ctx.MoveDirection * Ctx.SprintingSpeed, ForceMode.Force);
        }
    }
}
