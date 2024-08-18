using System;
using System.Collections;
using UnityEditorInternal;
using UnityEngine;

namespace Player.StateMachine.States.Lash{
    //Sub State of the lash state
    public class PlayerLashState : PlayerBaseState
    {
        
        #region State methods
        public PlayerLashState(PlayerStateMachine currentCtx, PlayerStateFactory stateFactory) : base(currentCtx, stateFactory, "Lash]") { }
        public override void EnterState() {
            Ctx.InAirTimer = 0;
            Ctx.IsGrounded = false;
            CheckSwitchStates();
            Ctx.IsLashing = true;
            //Ctx.AnimatorManager.animator.SetBool(Ctx.AnimatorManager.IsLashingHash, true);
            //Ctx.AnimatorManager.animator.SetBool(Ctx.AnimatorManager.IsHalfLashingHash ,false);
            
            //TODO: [Animation] -> set lash blend tree value
            //Ctx.AnimatorManager.PlayTargetAnimation("Lash");
     
            Ctx.CameraManager.SetCameraMode(CameraManager.CameraMode.Lash);
        
        }

        public override void UpdateState() {
            HandleMovement();
            HandleRotation();
            HandleGroundDetection();
            HandleGravity();
            CheckSwitchStates();

            Ctx.StormlightLashingDrain = Ctx.LashingIntensity;

        }

        public override void FixedUpdateState() {
        }

        public override void ExitState() {
            Ctx.IsLashing = false;
            //Ctx.AnimatorManager.animator.SetBool(Ctx.AnimatorManager.IsLashingHash, false);
            Ctx.GravityDirection.Normalize();
            Ctx.LashingIntensity = 0;
            Ctx.StormlightLashingDrain = 0;
        }

        public override void CheckSwitchStates() {
            //LAND
            if (Ctx.IsGrounded) {
                SwitchStates(Factory.Air());
            }
        
            //LASHINGS
            if (Ctx.InputManager.LashInput) {
                HandleLash(PlayerStateMachine.LASHING_INTENSITY_INCREMENT);
                Ctx.InputManager.ResetLashInput();
            }
            if (Ctx.InputManager.UnLashInput) {
                HandleLash(- PlayerStateMachine.LASHING_INTENSITY_INCREMENT);
                Ctx.InputManager.ResetUnLashInput();
            }
            if (Ctx.InputManager.SmallLashInput > 0 && Ctx.LashCooldown <= 0) {
                HandleSmallLash(PlayerStateMachine.LASHING_INTENSITY_SMALL_INCREMENT * Ctx.InputManager.SmallLashInput);
                Ctx.StartCoroutine(SmallLashCooldown(0.1f));
            }
            if (Ctx.InputManager.SmallUnLashInput > 0 && Ctx.LashCooldown <= 0) {
                HandleSmallLash(PlayerStateMachine.LASHING_INTENSITY_SMALL_INCREMENT * -Ctx.InputManager.SmallUnLashInput);
                Ctx.StartCoroutine(SmallLashCooldown(0.1f));
            }
            
            //DIVE
            if (Ctx.InputManager.DiveInput) {
                Ctx.InputManager.ResetDiveInput();
                HandleDive();
            }

            if (Ctx.InputManager.RollInput != 0) {
                if (Ctx.InputManager.RollInput > 0 && Ctx.AngleOfIncidence < 90) {
                    Ctx.AngleOfIncidence += Ctx.InputManager.RollInput * 1.1f;
                }else if (Ctx.InputManager.RollInput < 0 && Ctx.AngleOfIncidence > -90) {
                    Ctx.AngleOfIncidence += Ctx.InputManager.RollInput * 1.1f;
                }
            }
            
            
            //HALFLASH
            if (Ctx.LashingIntensity <= 0) {
                SwitchStates(Factory.Halflash());
            }
        }

        public override void InitializeSubState() {
        }
    
     
        #endregion
    
        #region private Methods

        private void HandleMovement() {
            //TODO: Add proper animations to this

            Vector3 upAxis = Vector3.Cross( Ctx.GravityDirection, Ctx.PlayerTransform.right);
            //Diving system
            Ctx.MoveDirection = upAxis * Ctx.InputManager.MovementInput.y +
                                Ctx.PlayerTransform.right * Ctx.InputManager.MovementInput.x;
            Ctx.MoveDirection.Normalize();
            //Calculate how much the angle of incidence affects the speed
            float speedMultiplier = Mathf.Abs(Ctx.AngleOfIncidence) / 90 ; //First get a magnitude of increase
            speedMultiplier *= 0.9f;
            speedMultiplier = 0.9f * 2 - speedMultiplier; //If the angle is 0, the speed is not affected (x1) If the angle is greater, the speed is reduced
            //Debug.Log("Speed Multiplier: " + speedMultiplier); 
            float diveTranslationSpeed = Ctx.GravityDirection.magnitude * 14f * speedMultiplier;
            
            //MODE 1 - DISPLACEMENT
            Ctx.PlayerRigidbody.AddForce(Ctx.MoveDirection * diveTranslationSpeed, ForceMode.Force);
            //MODE 2 - ROTATION DEVIATION
            //Ctx.GravityDirection += Ctx.MoveDirection * diveTranslationSpeed;
        }
    
        private void HandleRotation() {
            
            //Rotate to face towards the gravity direction
            if (Ctx.GravityDirection == Vector3.zero) 
                Ctx.GravityDirection = Ctx.CameraObject.up;

            if (Ctx.InputManager.SmallLashInput != 0) return;
            
            //ROTATION ON THE FORWARD AXIS 
            

            Vector3 projectedXZGravity = Vector3.ProjectOnPlane(Ctx.GravityDirection.normalized, Vector3.up);
            projectedXZGravity.Normalize();
            Vector3 rightVector = Vector3.Angle(projectedXZGravity, Vector3.forward) > 90
                ? -Vector3.right
                : Vector3.right;
            Vector3 upCross = Vector3.Cross( projectedXZGravity, rightVector);
            Quaternion targetForward = Quaternion.LookRotation(-upCross, projectedXZGravity);
            
            Ctx.PlayerTransform.rotation = Quaternion.Slerp(Ctx.PlayerTransform.rotation, targetForward, Ctx.LerpSpeed * 0.1f);
            
            //ROTATION ON THE RIGHT AXIS
            Quaternion targetAngle = Quaternion.AngleAxis(Ctx.AngleOfIncidence, Ctx.PlayerTransform.right);
            //Ctx.PlayerTransform.rotation = Quaternion.Slerp(Ctx.transform.rotation, targetRotation   , Ctx.LerpSpeed);
            
        }

        private void HandleGroundDetection() {
        
            //Vector3 rayCastOrigin = Ctx.transform.position - Ctx.RayCastHeightOffset * Ctx.GravityDirection;
            Vector3 rayCastOrigin = Ctx.PlayerRigidbody.worldCenterOfMass;
            
            if (Ctx.InAirTimer <= Ctx.MaxAirSpeed)
                Ctx.InAirTimer += Time.deltaTime;
            
            Ctx.PlayerRigidbody.AddForce(Ctx.GravityDirection * (Ctx.FallingVelocity * Ctx.InAirTimer), ForceMode.Force);
            
            if (Ctx.InAirTimer <= 0.1f) 
                return;
            
            if (Physics.SphereCast(rayCastOrigin, Ctx.RayCastRadius*0.5f, Ctx.GravityDirection, out RaycastHit hit ,Ctx.RayCastMaxDistance, Ctx.GroundLayer))
            {
            
                Ctx.IsGrounded = true;
                
                Ctx.GravityDirection = -hit.normal;  
                Ctx.StartCoroutine(TriggerLandingFromLashingCoroutine(hit.normal, hit.point, 0.25f));
            
                Ctx.InAirTimer = 0;
           
            }
            else
            {
                Ctx.IsGrounded = false;
            }
        }
    
        private void HandleGravity() {
        
            //Calculate how much the angle of incidence affects the speed
            float speedMultiplier = Mathf.Abs(Ctx.AngleOfIncidence) / 90 ; //First get a magnitude of increase
            speedMultiplier *= 0.75f;
            speedMultiplier = (1 - 0.75f/2) + speedMultiplier; // Get a value between 0.75f min and 1.25f max
            
            Ctx.PlayerRigidbody.AddForce(Ctx.GravityDirection * (Ctx.GravityIntensity * Ctx.GravityMultiplier * Ctx.LashingIntensity * speedMultiplier), ForceMode.Acceleration);

        }
    
        private void HandleLash(float lashAmount) {
        
            //TODO: Remove the Lashing intensity variable [OR NOT] 
            //TODO: Return only if the lash is in the same aprox direction of the current GravityDirection if reached max intensity.
            
            //If lash is at MAX return
            if (lashAmount > 0 && Ctx.LashingIntensity > PlayerStateMachine.MAX_LASHING_INTENSITY)
                return;
        
            //Get direction of lashing based on camera position
            Vector3 lashDirection = Ctx.CameraObject.forward;
            if (Physics.Raycast(Ctx.CameraObject.transform.position, lashDirection, out RaycastHit hit, 10000f, Ctx.GroundLayer)) {
                //lashDirection = (hit.point - Ctx.transform.position).normalized;
            }
            
            if (lashAmount >= 0) {
                Vector3 previousDirection = Ctx.GravityDirection;

                Vector3 newDirection = Ctx.LashingIntensity * Ctx.GravityDirection +
                                      lashAmount * lashDirection;
                if (newDirection.magnitude > 1)
                    newDirection.Normalize();
                
                Ctx.GravityDirection = newDirection;
                Ctx.LashingIntensity += lashAmount;

                float angleDifference =
                    Vector3.SignedAngle(previousDirection, Ctx.GravityDirection, Ctx.PlayerTransform.right);
                //Ctx.AngleOfIncidence -= angleDifference;
            }
            else {
                Ctx.LashingIntensity += lashAmount;
            }
            
        }

        private void HandleSmallLash(float lashAmount) {
            
            if (lashAmount > 0 && Ctx.LashingIntensity > PlayerStateMachine.MAX_LASHING_INTENSITY)
                return;
            
            if (Ctx.GravityDirection == Vector3.zero)
                Ctx.GravityDirection = Ctx.PlayerTransform.up;
            
            Ctx.LashingIntensity += lashAmount ;
        }

        private void HandleDive() {
            if (Ctx.AngleOfIncidence > 0) {
                Ctx.AngleOfIncidence = -90;
            }
            else {
                Ctx.AngleOfIncidence = 90;
            }
            Ctx.AnimatorManager.PlayTargetAnimation("FrontFlip Fall to Lash");
        }
        public IEnumerator TriggerLandingFromLashingCoroutine(Vector3 targetNormal, Vector3 hitPoint, float duration)
        {
            //TODO: Make it work without a Coroutine
            Vector3 originalPosition = Ctx.transform.position;
            Vector3 centerOfMass = Ctx.PlayerRigidbody.worldCenterOfMass;
         
            Quaternion startRotation = Ctx.PlayerTransform.rotation;
            Quaternion targetRotation = Quaternion.FromToRotation(Ctx.PlayerTransform.up, targetNormal) * Ctx.PlayerTransform.rotation;

            float timeElapsed = 0;
            while (timeElapsed < duration)
            {
                Ctx.transform.position = centerOfMass;
                Ctx.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, timeElapsed / duration);
                Ctx.transform.position = originalPosition;
            
                Ctx.transform.position = Vector3.Lerp(originalPosition, hitPoint + targetNormal * 0.5f, timeElapsed / duration);
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            Ctx.transform.position = hitPoint;
            Ctx.transform.rotation = targetRotation;
        }
    
        public IEnumerator SmallLashCooldown(float duration)
        {
            Ctx.LashCooldown = duration;
            while (Ctx.LashCooldown >= 0)
            {
                Ctx.LashCooldown -= Time.deltaTime;
                yield return null;
            }
        }
    
    
    

        #endregion
    }
}
