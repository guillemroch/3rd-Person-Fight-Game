using System.Collections;
using System.Collections.Generic;
using Player.StateMachine;
using UnityEngine;

public class PlayerJumpState : PlayerBaseState {

    public PlayerJumpState(PlayerStateMachine currentCtx, PlayerStateFactory stateFactory)
        : base(currentCtx, stateFactory) {
    }

    public override void EnterState() {
        HandleJump();
    }

    public override void UpdateState() {
        CheckSwitchStates();
    }

    public override void FixedUpdateState() { }

    public override void ExitState() {
        
    }

    public override void CheckSwitchStates() {
        SwitchStates(Factory.Fall());
    }

    public override void InitializeSubState() { }

    void HandleJump() {
        Ctx.AnimatorManager.animator.SetBool("isJumping", true);
        Ctx.AnimatorManager.PlayTargetAnimation("Jump", false);

        float jumpingVelocity = Mathf.Sqrt(2 * Ctx.GravityIntensity  *  Ctx.JumpHeight);
        Ctx.PlayerRigidbody.AddForce(jumpingVelocity * -Ctx.GravityDirection, ForceMode.Impulse);
    }
}

