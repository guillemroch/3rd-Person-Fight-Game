using System.Collections;
using UnityEngine;

namespace Player.StateMachine.States.Lash{
    public class PlayerHalflashState : PlayerBaseState
    {
        public PlayerHalflashState(PlayerStateMachine currentCtx, PlayerStateFactory stateFactory) : base(currentCtx, stateFactory) { }
        public override void EnterState() {
            Ctx.InputManager.ResetLashInput();
            Ctx.AnimatorManager.animator.SetBool(Ctx.AnimatorManager.IsHalfLashingHash, true);
            Ctx.AnimatorManager.PlayTargetAnimation("Half Lashing");
        
            Ctx.PlayerRigidbody.AddForce(Ctx.HalfLashingHeight * -Ctx.GravityDirection, ForceMode.Impulse);
            if (Ctx.isGrounded)
                Ctx.StartCoroutine(TriggerHalfLashingRotationCoroutine(0.5f));
        
            Ctx.isGrounded = false;
        
            Ctx.InAirTimer = 0;
        }

        public override void UpdateState() {
            CheckSwitchStates();
            //Make the player rotate with the camera, making that we always see the back of the player
            Vector2 lookInput = Ctx.InputManager.LookInput;
            Ctx.transform.localRotation *= Quaternion.Euler(-lookInput.y * Ctx.RotationSpeed * 0.1f , 0 , -lookInput.x * Ctx.RotationSpeed * 0.1f);;
        }

    
        public override void FixedUpdateState() {
        }

        public override void ExitState() {
        }

        public override void CheckSwitchStates() {
            if (Ctx.InputManager.LashInput || Ctx.InputManager.SmallLashInput > 0) {
                SwitchStates(Factory.Lash());
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
    }
}
