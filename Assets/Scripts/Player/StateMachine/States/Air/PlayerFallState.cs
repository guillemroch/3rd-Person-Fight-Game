using System.Collections;
using System.Collections.Generic;
using Player.StateMachine;
using UnityEngine;

public class PlayerFallState : PlayerBaseState
{
    public PlayerFallState(PlayerStateMachine currentCtx, PlayerStateFactory stateFactory) : base(currentCtx, stateFactory) { }
    public override void EnterState() {
        Debug.Log("Entered Fall Sub State with parent state: " + CurrentSuperState.GetType());

        Ctx.animatorManager.PlayTargetAnimation("Fall", true);
    }

    public override void UpdateState() {
        CheckSwitchStates();
        HandleFalling();
        HandleGravity();
        
    }

    public override void FixedUpdateState() {
    }

    public override void ExitState() {
    }

    public override void CheckSwitchStates() {
         if (Ctx.isGrounded) {
            SwitchStates(Factory.Land());
        }
    }

    public override void InitializeSubState() {
    }

    void HandleFalling() {
        
        if (Ctx.inAirTimer <= Ctx.maxAirSpeed) Ctx.inAirTimer += Time.deltaTime;
        
        Ctx.playerRigidbody.AddForce(Ctx.playerTransform.forward * (Ctx.leapingVelocity * Ctx.playerRigidbody.velocity.magnitude), ForceMode.Force);
        Ctx.playerRigidbody.AddForce(Ctx.gravityDirection * (Ctx.fallingVelocity * Ctx.inAirTimer), ForceMode.Force);
        
        Ctx.playerRigidbody.AddForce(Ctx.gravityIntensity * Ctx.gravityMultiplier * Ctx.gravityDirection, ForceMode.Acceleration);

        Vector3 rayCastOrigin = Ctx.transform.position - Ctx.rayCastHeightOffset * Ctx.gravityDirection;
        rayCastOrigin = Ctx.playerRigidbody.worldCenterOfMass;
        if (Physics.SphereCast(rayCastOrigin, Ctx.rayCastRadius, Ctx.gravityDirection, out RaycastHit hit, Ctx.rayCastMaxDistance,
                Ctx.groundLayer)) {
            Ctx.isGrounded = true;
        }

    }

    void HandleGravity() {
        Ctx.playerRigidbody.AddForce(Ctx.gravityIntensity * Ctx.gravityMultiplier * Ctx.gravityDirection, ForceMode.Acceleration);

    }
}
