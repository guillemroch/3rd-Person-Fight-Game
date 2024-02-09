using System.Collections;
using System.Collections.Generic;
using Player.StateMachine;
using UnityEngine;

public class PlayerHalflashState : PlayerBaseState
{
    public PlayerHalflashState(PlayerStateMachine currentCtx, PlayerStateFactory stateFactory) : base(currentCtx, stateFactory) { }
    public override void EnterState() {
        Debug.Log("<color=lightblue>Entered Halflash substate with parent state: " + CurrentSuperState.GetType() + "</color>");
        Ctx.animatorManager.animator.SetBool("isHalfLashing", true);
        Ctx.animatorManager.PlayTargetAnimation("Half Lashing", false);
        
        Ctx.playerRigidbody.AddForce(Ctx.halfLashingHeight * -Ctx.gravityDirection, ForceMode.Impulse);
        if (Ctx.isGrounded)
            Ctx.StartCoroutine(TriggerHalfLashingRotationCoroutine(0.5f));
        
        Ctx.isGrounded = false;
        
        Ctx.inAirTimer = 0;
    }

    public override void UpdateState() {
        CheckSwitchStates();
        //Make the player rotate with the camera, making that we always see the back of the player
        Ctx.transform.RotateAround(Ctx.playerRigidbody.worldCenterOfMass, Ctx.playerTransform.forward, Ctx.rotationSpeed * Time.deltaTime * -Ctx.inputManager.cameraInput.x);
        Ctx.transform.RotateAround(Ctx.playerRigidbody.worldCenterOfMass, Ctx.playerTransform.right, Ctx.rotationSpeed * Time.deltaTime * -Ctx.inputManager.cameraInput.y);
    }

    
    public override void FixedUpdateState() {
    }

    public override void ExitState() {
    }

    public override void CheckSwitchStates() {
        if (Ctx.InputManager.LashInput) {
            SwitchStates(Factory.Lashing());
        }
    }

    public override void InitializeSubState() {
    }
    
  
    
    public IEnumerator TriggerHalfLashingRotationCoroutine(float duration)
    {
        //isHalfLashingFromGround = true;
        float timeElapsed = 0;
        Quaternion startRotation = Ctx.playerTransform.rotation;
        Quaternion targetRotation = Quaternion.FromToRotation(Ctx.playerTransform.up, Ctx.playerTransform.forward)
                                    * Ctx.playerTransform.rotation;
        while (timeElapsed < duration)
        {
            Ctx.transform.rotation = 
                Quaternion.Slerp(startRotation, targetRotation, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        Ctx.transform.rotation = targetRotation;
    }
}
