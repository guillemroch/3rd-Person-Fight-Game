using UnityEngine;

namespace Player.StateMachine.States.Ground{
    public class PlayerGroundedState : PlayerBaseState {
    
        public PlayerGroundedState(PlayerStateMachine currentCtx, PlayerStateFactory stateFactory)
            : base(currentCtx, stateFactory, "Grounded") {
            IsRootState = true;
            InitializeSubState();
        
        }

        public override void EnterState() {
            Ctx.CameraManager.SetCameraMode(CameraManager.CameraMode.Normal);
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

            if (!Ctx.IsUsingStormlight) {
                return;
            }
            if (Ctx.InputManager.LashInput || Ctx.InputManager.SmallLashInput > 0) {
                SwitchStates(Factory.Lashing());
            }
        }

        public override void InitializeSubState() {
        
            if (Ctx.InputManager.MovementInput == Vector2.zero) {
                SetSubStates(Factory.Idle());
            }else if (!Ctx.InputManager.IsSprintPressed) {
                if (Ctx.InputManager.MoveAmount <= 0.5f) {

                    SetSubStates(Factory.Walk());
                }else {
                    SetSubStates(Factory.Run());
                }
            }else {
                SetSubStates(Factory.Sprint());
            }
        }

        public void HandleGroundDetection() {
        
            Vector3 rayCastOrigin = Ctx.PlayerRigidbody.worldCenterOfMass;
            rayCastOrigin += Ctx.PlayerTransform.up * Ctx.RayCastHeightOffset; //Consider the orientation of the Player
            RaycastHit hit;
            //Logic for stairs and slopes
            Vector3 targetPosition = Ctx.PlayerTransform.position;
            
        
            if (Physics.SphereCast(
                    rayCastOrigin, Ctx.RayCastRadius, Ctx.GravityDirection, out hit ,Ctx.RayCastMaxDistance, Ctx.GroundLayer))
            {
                Ctx.InAirTimer = 0;
                Ctx.isGrounded = true;
                //Logic for stairs
                Vector3 rayCastHitPoint = hit.point;
                //targetPosition = rayCastHitPoint;
                //[STAIRS] - DELETED FOR LATER FIX
                /*targetPosition.y = rayCastHitPoint.y;

                targetPosition += Vector3.Project(rayCastHitPoint, Ctx.PlayerTransform.up);
                Ctx.PlayerTransform.position =
                   Vector3.Lerp(Ctx.PlayerTransform.position, targetPosition, Time.deltaTime / 0.1f);*/
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
            
            if (Ctx.GravityDirection == Vector3.zero)
                Ctx.GravityDirection = Vector3.down;

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
