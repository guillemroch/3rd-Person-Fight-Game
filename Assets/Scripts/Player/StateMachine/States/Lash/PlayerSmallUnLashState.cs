using System.Collections;
using System.Collections.Generic;
using Player.StateMachine;
using UnityEngine;

public class PlayerSmallUnLashState : PlayerBaseState
{
    public PlayerSmallUnLashState(PlayerStateMachine currentCtx, PlayerStateFactory stateFactory) : base(currentCtx, stateFactory) { }
    public override void EnterState() {
        
        //Ctx.InputManager.ResetLashInput();
        Ctx.GravityDirection = Ctx.PlayerTransform.up;
        Ctx.LashingIntensity -= PlayerStateMachine.LASHING_INTENSITY_SMALL_INCREMENT;
        Ctx.AnimatorManager.animator.SetBool(Ctx.AnimatorManager.IsHalfLashingHash ,false);
        Ctx.AnimatorManager.animator.SetBool(Ctx.AnimatorManager.IsLashingHash, true);
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
        SwitchStates(Factory.Lash());
    }

    public override void InitializeSubState() {
    }
    
    private void HandleMovement() {
        Ctx.MoveDirection = Ctx.PlayerTransform.right * Ctx.InputManager.MovementInput.y +
                            Ctx.PlayerTransform.forward * -Ctx.InputManager.MovementInput.x;
        

        Ctx.MoveDirection.Normalize();
        
        Quaternion rotation = Quaternion.Euler(Ctx.MoveDirection) * Quaternion.Euler(Ctx.GravityDirection);
        
        Ctx.GravityDirection = rotation * Ctx.GravityDirection;
    }

    private void HandleRotation() {
        Quaternion targetRotation = Quaternion.FromToRotation(Ctx.PlayerTransform.up, Ctx.GravityDirection);
        Ctx.PlayerTransform.rotation = targetRotation * Ctx.PlayerTransform.rotation;
    }

    private void HandleGroundDetection() {
        
        //Vector3 rayCastOrigin = Ctx.transform.position - Ctx.RayCastHeightOffset * Ctx.GravityDirection;
        Vector3 rayCastOrigin = Ctx.PlayerRigidbody.worldCenterOfMass;
        
        if (Ctx.InAirTimer <= Ctx.MaxAirSpeed) Ctx.InAirTimer += Time.deltaTime;
        Ctx.PlayerRigidbody.AddForce(Ctx.GravityDirection * (Ctx.FallingVelocity * Ctx.InAirTimer), ForceMode.Force);
        
        if (Physics.SphereCast(rayCastOrigin, Ctx.RayCastRadius, Ctx.GravityDirection, out RaycastHit hit ,Ctx.RayCastMaxDistance, Ctx.GroundLayer))
        {
           
            Ctx.isGrounded = true;
            Ctx.GravityDirection = -hit.normal;  
            Ctx.StartCoroutine(TriggerLandingFromLashingCoroutine(hit.normal, hit.point, 0.25f));
            
            Ctx.InAirTimer = 0;
           
        }
        else
        {
            Ctx.isGrounded = false;
        }
    }
    
    private void HandleGravity() {
        Ctx.PlayerRigidbody.AddForce(Ctx.GravityDirection * (Ctx.GravityIntensity * Ctx.GravityMultiplier * Ctx.LashingIntensity), ForceMode.Acceleration);

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
}
