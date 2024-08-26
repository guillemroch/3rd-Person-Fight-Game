using UnityEngine;

namespace Player.StateMachine.States.Ground{
    public class PlayerWalkState : PlayerBaseState
    {
        public PlayerWalkState(PlayerStateMachine currentCtx, PlayerStateFactory stateFactory) : base(currentCtx, stateFactory, "Walk") { }
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
            if (Ctx.InputManager.DashInput) {
                SwitchStates(Factory.Dash());
            }
            if (Ctx.InputManager.MovementInput == Vector2.zero) {
                SwitchStates(Factory.Idle());
            }else if (!Ctx.InputManager.IsSprintPressed) {
                if (Ctx.InputManager.MoveAmount > 0.5f) {
                    SwitchStates(Factory.Run());
                }
            }
            else if (Ctx.InputManager.IsSprintPressed) {
                SwitchStates(Factory.Sprint());
            }
        }

        public override void InitializeSubState() {
        }

        private void HandleMovement() {
               Vector3 planeNormal = Vector3.Cross(Ctx.PlayerTransform.right, Ctx.PlayerTransform.forward);
            Vector3 projectedCameraForward = Vector3.ProjectOnPlane(Ctx.CameraObject.forward, planeNormal).normalized;
            Vector3 projectedCameraRight = Vector3.ProjectOnPlane(Ctx.CameraObject.right, planeNormal).normalized;
            Ctx.MoveDirection = projectedCameraForward * Ctx.InputManager.MovementInput.y +projectedCameraRight * Ctx.InputManager.MovementInput.x;
            Ctx.MoveDirection.Normalize();
        
            Ctx.PlayerRigidbody.AddForce(Ctx.MoveDirection * Ctx.WalkingSpeed, ForceMode.Force);
        }
    }
}
