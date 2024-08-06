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
            Ctx.MoveDirection = Ctx.CameraObject.forward * Ctx.InputManager.MovementInput.y + Ctx.CameraObject.right * Ctx.InputManager.MovementInput.x;

            float moveDot = Vector3.Dot(Ctx.MoveDirection, Ctx.GravityDirection);
            float magSquared = Ctx.GravityDirection.sqrMagnitude;
    
            Vector3 projection = (moveDot / magSquared) * Ctx.GravityDirection;
            Ctx.MoveDirection += -projection;
            Ctx.MoveDirection.Normalize();
        
            Ctx.PlayerRigidbody.AddForce(Ctx.MoveDirection * Ctx.SprintingSpeed, ForceMode.Force);
        }
    }
}
