using System.Collections;
using System.Linq;
using UnityEngine;

namespace Player.StateMachine.States.Lash{
    //Sub State of the Lash States
    public class PlayerHalflashState : PlayerBaseState
    {
        public PlayerHalflashState(PlayerStateMachine currentCtx, PlayerStateFactory stateFactory) :
            base(currentCtx, stateFactory, "HalfLash") { }
        public override void EnterState() {
            Ctx.InputManager.ResetLashInput();
            Ctx.IsHalfLashing = true;
            //Ctx.AnimatorManager.animator.SetBool(Ctx.AnimatorManager.IsHalfLashingHash, true);
            //Ctx.AnimatorManager.animator.SetBool(Ctx.AnimatorManager.IsLashingHash, false);
            //Ctx.AnimatorManager.PlayTargetAnimation("Half Lashing");  //TODO: Prevent the use of this methods

            //TODO: [Animation] -> set lash blend tree value
            if (Ctx.IsGrounded)
                Ctx.PlayerRigidbody.AddForce((0.1f + Ctx.HalfLashingHeight) * Vector3.up, ForceMode.Impulse);
            //if (Ctx.isGrounded)
                //Ctx.StartCoroutine(TriggerHalfLashingRotationCoroutine(0.5f));
            //Ctx.GravityDirection = Vector3.zero;
            Ctx.IsGrounded = false;
            Ctx.InAirTimer = 0;
            Ctx.CameraManager.SetCameraMode(CameraManager.CameraMode.HalfLash);
        }

        public override void UpdateState() {
            CheckSwitchStates();
            HandleRotation();

            Ctx.StormlightLashingDrain = 1;
            //Make the player rotate with the camera, making that we always see the back of the player
            Vector2 lookInput = Ctx.InputManager.LookInput;
            //Ctx.transform.localRotation *= Quaternion.Euler(-lookInput.y * Ctx.RotationSpeed * 0.1f , 0 , -lookInput.x * Ctx.RotationSpeed * 0.1f);
        }
    
        public override void FixedUpdateState() {
        }

        public override void ExitState() {
            Ctx.IsHalfLashing = false;
            //Ctx.AnimatorManager.animator.SetBool(Ctx.AnimatorManager.IsHalfLashingHash, false);
        }

        public override void CheckSwitchStates() {
            if (Ctx.InputManager.LashInput || Ctx.InputManager.SmallLashInput > 0) {
                SwitchStates(Factory.Lash());
            }
            if (Ctx.InputManager.UnLashInput) {

                CalculateGravityDirection();
                SwitchStates(Factory.Air());
                Ctx.LashingIntensity = PlayerStateMachine.DEFAULT_LASHING_INTENSITY;
            }
        }

        public override void InitializeSubState() {
        }
        
        public IEnumerator TriggerHalfLashingRotationCoroutine(float duration)
        {
            //isHalfLashingFromGround = true;
            float timeElapsed = 0;
            var rotation = Ctx.PlayerTransform.rotation;
            Quaternion targetRotation = Quaternion.FromToRotation(Ctx.PlayerTransform.up, Ctx.PlayerTransform.forward)
                                        * rotation;
            while (timeElapsed < duration)
            {
                Ctx.transform.rotation = 
                    Quaternion.Slerp(rotation, targetRotation, timeElapsed / duration);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            Ctx.transform.rotation = targetRotation;
        }

        private void HandleRotation() {
            Quaternion targetRotation = Quaternion.FromToRotation(Ctx.PlayerTransform.up, Vector3.up);
            Ctx.transform.rotation = Quaternion.Slerp(Ctx.transform.rotation, Ctx.transform.rotation * targetRotation,
                6f * Time.deltaTime);
        }
        
        private void CalculateGravityDirection()
        {
            Collider[] colliders = Physics.OverlapSphere(Ctx.transform.position , Ctx.MaxUnlashDistance);
            if (colliders.Length > 0){
                colliders.OrderBy(c => Vector3.Distance(Ctx.transform.position, c.transform.position));
                Vector3 closestPoint = colliders[0].ClosestPoint(Ctx.transform.position);
                if (Vector3.Distance(Ctx.transform.position, closestPoint) <= 1)
                {
                    Ctx.GravityDirection = Vector3.down;
                    return;
                }
                //TODO: Fix this if think it is needed
                Ctx.GravityDirection = closestPoint - Ctx.transform.position;
                Ctx.GravityDirection = Vector3.down;
                Debug.Log("Ground found: " + colliders[0].name + " | at point:  " + closestPoint + " which is closer from: " + Ctx.transform.position);
                Debug.DrawRay(Ctx.transform.position, Ctx.GravityDirection * 10, Color.red, 10);
            }
            else
            {
                Debug.Log("No ground found");
                Ctx.GravityDirection = Vector3.down;
            }
        }
        
        
    }
}
