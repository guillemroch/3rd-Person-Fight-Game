using System.Collections;
using System.Collections.Generic;
using Player.StateMachine;
using UnityEngine;

public class PlayerLashingState : PlayerBaseState
{
    public PlayerLashingState(PlayerStateMachine currentCtx, PlayerStateFactory stateFactory) : base(currentCtx, stateFactory) { }
    public override void EnterState() {
        
        Debug.Log("Enter Lashing State");
        
        Ctx.gravityDirection = Ctx.playerTransform.up;

        Ctx.isHalfLashing = false; //TODO: Change this to a state machine
        Ctx.animatorManager.animator.SetBool("isHalfLashing", false);
        Ctx.animatorManager.animator.SetBool("isLashing", true);
    }

    public override void UpdateState() {
        HandleMovement();
        HandleRotation();
        HandleGroundDetection();
        HandleGravity();
        CheckSwitchStates();
        
    }

    public override void FixedUpdateState() {
    }

    public override void ExitState() {
    }

    public override void CheckSwitchStates() {
        if (Ctx.isGrounded) {
            SwitchStates(Factory.Air());
        }
        if (Ctx.InputManager.HalfLashInput) {
            SwitchStates(Factory.Halflash());
        }
    }

    public override void InitializeSubState() {
    }
    
    private void HandleMovement() {
        Ctx.moveDirection = Ctx.playerTransform.right * Ctx.inputManager.movementInput.y +
                            Ctx.playerTransform.forward * -Ctx.inputManager.movementInput.x;
        

        Ctx.moveDirection.Normalize();
        
        Quaternion rotation = Quaternion.Euler(Ctx.moveDirection) * Quaternion.Euler(Ctx.gravityDirection);
        
        Ctx.gravityDirection = rotation * Ctx.gravityDirection;
    }

    private void HandleRotation() {
        Quaternion targetRotation = Quaternion.FromToRotation(Ctx.playerTransform.up, Ctx.gravityDirection);
        Ctx.playerTransform.rotation = targetRotation * Ctx.playerTransform.rotation;
    }

    private void HandleGroundDetection() {
        
        Vector3 rayCastOrigin = Ctx.transform.position - Ctx.rayCastHeightOffset * Ctx.gravityDirection;
        rayCastOrigin = Ctx.playerRigidbody.worldCenterOfMass;
        
        if (Ctx.inAirTimer <= Ctx.maxAirSpeed) Ctx.inAirTimer += Time.deltaTime;
        Ctx.playerRigidbody.AddForce(Ctx.gravityDirection * (Ctx.fallingVelocity * Ctx.inAirTimer), ForceMode.Force);
        
        if (Physics.SphereCast(rayCastOrigin, Ctx.rayCastRadius, Ctx.gravityDirection, out RaycastHit hit ,Ctx.rayCastMaxDistance, Ctx.groundLayer))
        {
           
            Ctx.isGrounded = true;
            Ctx.gravityDirection = -hit.normal;  
            Ctx.StartCoroutine(TriggerLandingFromLashingCoroutine(hit.normal, hit.point, 0.25f));
            
            Ctx.inAirTimer = 0;
           
        }
        else
        {
            Ctx.isGrounded = false;
        }
    }
    
    private void HandleGravity() {
        Ctx.playerRigidbody.AddForce(Ctx.gravityIntensity * Ctx.gravityMultiplier * Ctx.gravityDirection, ForceMode.Acceleration);

    }
    
    public IEnumerator TriggerLandingFromLashingCoroutine(Vector3 targetNormal, Vector3 hitPoint, float duration)
    {
        //TODO: Make it work without a Coroutine
        //isLandingFromLashing = true;
        Vector3 originalPosition = Ctx.transform.position;
        Vector3 centerOfMass = Ctx.playerRigidbody.worldCenterOfMass;
         
        Quaternion startRotation = Ctx.playerTransform.rotation;
        Quaternion targetRotation = Quaternion.FromToRotation(Ctx.playerTransform.up, targetNormal) * Ctx.playerTransform.rotation;

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
        Ctx.isLashing = false;
        //isLandingFromLashing = false;
    }
}
