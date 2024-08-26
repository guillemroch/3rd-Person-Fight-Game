using UnityEngine;

namespace Player.StateMachine.States.Ground{
    public class PlayerRunState : PlayerBaseState
    {
        public PlayerRunState(PlayerStateMachine currentCtx, PlayerStateFactory stateFactory) : base(currentCtx, stateFactory, "Run") { }
        public override void EnterState() {
            //Setup starting things for animation or logic
            Ctx.StormlightMovementDrain = 0;
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
            }
            else if (!Ctx.InputManager.IsSprintPressed) {
                if (Ctx.InputManager.MoveAmount <= 0.5f) {
                    SwitchStates(Factory.Walk());
                }
            }
            else {
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

            /*float moveDot = Vector3.Dot(Ctx.MoveDirection, Ctx.GravityDirection);
            float magSquared = Ctx.GravityDirection.sqrMagnitude;
    
            Vector3 projection = (moveDot / magSquared) * Ctx.GravityDirection;
            Ctx.MoveDirection += -projection;*/
            Ctx.MoveDirection.Normalize();
        
            Ctx.PlayerRigidbody.AddForce(Ctx.MoveDirection * Ctx.RunningSpeed, ForceMode.Force);
        }
    }
}
