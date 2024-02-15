using UnityEngine;

namespace Player.StateMachine.States.Ground{
    public class PlayerRunState : PlayerBaseState
    {
        public PlayerRunState(PlayerStateMachine currentCtx, PlayerStateFactory stateFactory) : base(currentCtx, stateFactory) { }
        public override void EnterState() {
            //Setup starting things for animation or logic


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
            Ctx.MoveDirection = Ctx.CameraObject.forward * Ctx.InputManager.MovementInput.y + Ctx.CameraObject.right * Ctx.InputManager.MovementInput.x;

            float moveDot = Vector3.Dot(Ctx.MoveDirection, Ctx.GravityDirection);
            float magSquared = Ctx.GravityDirection.sqrMagnitude;
    
            Vector3 projection = (moveDot / magSquared) * Ctx.GravityDirection;
            Ctx.MoveDirection += -projection;
            Ctx.MoveDirection.Normalize();
        
            Ctx.PlayerRigidbody.AddForce(Ctx.MoveDirection * Ctx.RunningSpeed, ForceMode.Force);
        }
    
 
    }
}
