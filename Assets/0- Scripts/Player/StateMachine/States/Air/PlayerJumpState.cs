using UnityEngine;

namespace Player.StateMachine.States.Air{
    public class PlayerJumpState : PlayerBaseState {

        public PlayerJumpState(PlayerStateMachine currentCtx, PlayerStateFactory stateFactory)
            : base(currentCtx, stateFactory, "Jump") {
        }

        public override void EnterState() {
            Ctx.AnimatorManager.animator.SetBool("IsJumping", false);
            HandleJump();
        }

        public override void UpdateState() {
            CheckSwitchStates();
            HandleMovement();
    
        }

        public override void FixedUpdateState() { }

        public override void ExitState() {
        
        }

        public override void CheckSwitchStates() {
            //if () {
            if (Ctx.AnimatorManager.animator.GetBool("IsJumping"))
                SwitchStates(Factory.Fall());
            //}
        }

        public override void InitializeSubState() { }

        void HandleJump() {
            Ctx.InputManager.ResetJumpInput();
            Ctx.AnimatorManager.PlayTargetAnimation("Jump Blend Tree");
            float jumpingVelocity = Mathf.Sqrt(2 * Ctx.GravityIntensity  *  Ctx.JumpHeight);
            Ctx.PlayerRigidbody.AddForce(jumpingVelocity * -Ctx.GravityDirection, ForceMode.Impulse);
        }
        
        private void HandleMovement() {

            Ctx.InAirTimer += Time.deltaTime;
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

