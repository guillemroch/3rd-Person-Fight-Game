using UnityEngine;

namespace Player.StateMachine.States.Air{
    public class PlayerAirState : PlayerBaseState
    {
        public PlayerAirState(PlayerStateMachine currentCtx, PlayerStateFactory stateFactory)
            : base(currentCtx, stateFactory, "Air") {
            IsRootState = true;
            InitializeSubState();
        }
        public override void EnterState() {
            Ctx.IsLashing = false;
            Ctx.IsHalfLashing = false;
        }

        public override void UpdateState() {
            CheckSwitchStates();
            HandleRotation();
        }

        public override void FixedUpdateState() {
        }

        public override void ExitState() {
        }

        public override void CheckSwitchStates() {
        
        
        }

        public override void InitializeSubState() {
            if (Ctx.InputManager.IsJumpPressed) {
                SetSubStates(Factory.Jump());
            }
            else {
                SetSubStates(Factory.Fall());
            }
        }
        
        private void HandleRotation()
        {
        
            Ctx.TargetDirection = Vector3.zero; //Resets target direction
        
            //calculate orientation based on camera position
            Ctx.TargetDirection = Ctx.CameraObject.forward * Ctx.InputManager.MovementInput.y +
                                  Ctx.CameraObject.right * Ctx.InputManager.MovementInput.x;
        
            float moveDot = Vector3.Dot(Ctx.TargetDirection, Ctx.GravityDirection);
            float magSquared = Ctx.GravityDirection.sqrMagnitude;
    
            Vector3 projection = (moveDot / magSquared) * Ctx.GravityDirection;
            Ctx.TargetDirection += -projection;
            Ctx.TargetDirection.Normalize();
        
            if (Ctx.TargetDirection == Vector3.zero)
                Ctx.TargetDirection = Ctx.PlayerTransform.forward;
            
            if (Ctx.GravityDirection == Vector3.zero)
                Ctx.GravityDirection = Vector3.down;

            Quaternion targetRotation = 
                Quaternion.LookRotation(Ctx.TargetDirection, -Ctx.GravityDirection);

            Ctx.transform.rotation = 
                Quaternion.Slerp(Ctx.PlayerTransform.rotation, targetRotation, Ctx.RotationSpeed * Time.deltaTime);
        
        }
    }
}
