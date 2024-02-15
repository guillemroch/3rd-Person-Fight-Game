using UnityEngine;

namespace Player.StateMachine.States.Ground{
    public class PlayerGroundedState : PlayerBaseState {
    
        public PlayerGroundedState(PlayerStateMachine currentCtx, PlayerStateFactory stateFactory)
            : base(currentCtx, stateFactory) {
            IsRootState = true;
            InitializeSubState();
        
        }

        public override void EnterState() {
        }

        public override void UpdateState() {
            CheckSwitchStates();
            HandleRotation();
            HandleGroundDetection();
            HandleGravity();
        }

        public override void FixedUpdateState() { }

        public override void ExitState() {
        }

        public override void CheckSwitchStates() {
            if (Ctx.InputManager.IsJumpPressed || !Ctx.isGrounded) {
                SwitchStates(Factory.Air());
            } 
            if (Ctx.InputManager.HalfLashInput) {
                SwitchStates(Factory.Lash());
            }
        }

        public override void InitializeSubState() {
        
            if (Ctx.InputManager.MovementInput == Vector2.zero) {
                SwitchStates(Factory.Idle());
            }else if (!Ctx.InputManager.IsSprintPressed) {
                if (Ctx.InputManager.MoveAmount <= 0.5f) {

                    SwitchStates(Factory.Walk());
                }else {
                    SwitchStates(Factory.Run());
                }
            }else {
                SwitchStates(Factory.Sprint());
            }
        }

        public void HandleGroundDetection() {
        
            Vector3 rayCastOrigin = Ctx.PlayerRigidbody.worldCenterOfMass;
            
        
            if (Physics.SphereCast(
                    rayCastOrigin, Ctx.RayCastRadius, Ctx.GravityDirection, out _ ,Ctx.RayCastMaxDistance, Ctx.GroundLayer)
               )
            {
                Ctx.InAirTimer = 0;
                Ctx.isGrounded = true;
            }
            else
            {
                Ctx.isGrounded = false;
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


            Quaternion targetRotation = 
                Quaternion.LookRotation(Ctx.TargetDirection, -Ctx.GravityDirection);

            Ctx.transform.rotation = 
                Quaternion.Slerp(Ctx.PlayerTransform.rotation, targetRotation, Ctx.RotationSpeed * Time.deltaTime);
        
        }

        private void HandleGravity() {
            Ctx.PlayerRigidbody.AddForce(
                Ctx.GroundedGravity * Ctx.GravityMultiplier * Ctx.GravityDirection, ForceMode.Acceleration);

        }
    }
}
