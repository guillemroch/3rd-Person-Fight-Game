using System;
using System.Collections;
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

            Ctx.StormlightLashingDrain = Vector3.Distance(Vector3.down, Ctx.GravityDirection * Ctx.LashingIntensity);


        }

        public override void FixedUpdateState() {
        }

        public override void ExitState() {
            Ctx.IsLashing = false;
            //Ctx.AnimatorManager.animator.SetBool(Ctx.AnimatorManager.IsLashingHash, false);
            Ctx.GravityDirection.Normalize();
            Ctx.LashingIntensity = 0;
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
            if (Ctx.InputManager.DashInput) {
                SwitchStates(Factory.LashDash());
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
            
            //Diving system
            Ctx.MoveDirection = Ctx.PlayerTransform.forward * Ctx.InputManager.MovementInput.y +
                                Ctx.PlayerTransform.right * Ctx.InputManager.MovementInput.x;
            Ctx.MoveDirection.Normalize();
            
            //Calculate how much the angle of incidence affects the speed
            float speedMultiplier = Mathf.Abs(Ctx.AngleOfIncidence) / 90 ; //First get a magnitude of increase
            speedMultiplier *= 0.75f;
            speedMultiplier = 1 - speedMultiplier; //If the angle is 0, the speed is not affected (x1) If the angle is greater, the speed is reduced
            
            float diveTranslationSpeed = Ctx.GravityDirection.magnitude * 14f * speedMultiplier; //TODO: Adjust this speed based on the AngleOfIncidence
            Ctx.PlayerRigidbody.AddForce(Ctx.MoveDirection * diveTranslationSpeed, ForceMode.Force);
        }
    
        private void HandleRotation() {
            
            //Rotate to face towards the gravity direction
            if (Ctx.GravityDirection == Vector3.zero) 
                Ctx.GravityDirection = Ctx.CameraObject.up;

            if (Ctx.InputManager.SmallLashInput == 0) {
                //Orient up player toward the gravity direction
                Quaternion targetRotation = Quaternion.FromToRotation(Ctx.PlayerTransform.forward, Ctx.GravityDirection.normalized);
                Quaternion targetAngle = Quaternion.AngleAxis(Ctx.AngleOfIncidence, Ctx.PlayerTransform.right);
                targetRotation = Quaternion.Slerp(targetRotation, targetRotation * targetAngle, 50 * Time.deltaTime); 
                
                //Calculate in which direction the forward vector must be
      
                //Apply rotation to transform
                Ctx.PlayerTransform.rotation = Quaternion.Slerp(Ctx.transform.rotation,  targetRotation  * Ctx.PlayerTransform.rotation, Ctx.LerpSpeed);
                
                Vector3 planeOrthogonal = Vector3.Cross(Ctx.PlayerTransform.right, Vector3.down);
                Vector3 projectedVector = Vector3.ProjectOnPlane(Ctx.PlayerTransform.right, planeOrthogonal);
                //float angle = Vector3.Angle(Vector3.down, projectedVector);
                float angle =   Vector3.SignedAngle(Vector3.down, projectedVector, Ctx.GravityDirection.normalized);
                angle = 90 + angle * ( angle > 0 ? -1 : 1);
                Debug.Log("Angle is: " + angle);
              
                Quaternion rollCorrection = Quaternion.AngleAxis(angle, Ctx.GravityDirection.normalized);
                Ctx.PlayerTransform.rotation *= rollCorrection;
            }
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
            
            Ctx.PlayerRigidbody.AddForce(Ctx.GravityDirection.normalized * (Ctx.GravityIntensity * Ctx.GravityMultiplier * Ctx.LashingIntensity * speedMultiplier), ForceMode.Acceleration);

        }
    
        private void HandleLash(float lashAmount) {
        
            //TODO: Remove the Lashing intensity variable [OR NOT] 
            //TODO: Return only if the lash is in the same aprox direction of the current GravityDirection if reached max intensity.
        
            //If lash is at MAX return
            if (lashAmount > 0 && Ctx.LashingIntensity > PlayerStateMachine.MAX_LASHING_INTENSITY)
                return;
        
            //Get direction of lashing based on camera position
            Vector3 lashDirection = Ctx.CameraObject.forward;
            /*if (Physics.Raycast(Ctx.CameraObject.transform.position, lashDirection, out RaycastHit hit, 10000f)) {
                lashDirection = (hit.point - Ctx.transform.position).normalized;
            }*/
            
            if (lashAmount > 0) {
                Ctx.GravityDirection += lashDirection * lashAmount;
                Ctx.LashingIntensity = Ctx.GravityDirection.magnitude;
            }
            else {
                Ctx.GravityDirection += Ctx.GravityDirection.normalized * lashAmount;
                Ctx.LashingIntensity += lashAmount;
            }
        }

        private void HandleSmallLash(float lashAmount) {
            
            if (lashAmount > 0 && Ctx.LashingIntensity > PlayerStateMachine.MAX_LASHING_INTENSITY)
                return;
            
            if (Ctx.GravityDirection == Vector3.zero)
                Ctx.GravityDirection = Ctx.PlayerTransform.up;
            
            Ctx.GravityDirection += Ctx.PlayerTransform.up * lashAmount;
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
