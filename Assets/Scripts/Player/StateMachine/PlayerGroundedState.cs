using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : PlayerBaseState {

    public PlayerGroundedState(PlayerStateMachine currentCtx, PlayerStateFactory stateFactory)
        : base(currentCtx, stateFactory) {
        InitializeSubState();
        IsRootState = true;
    }

    public override void EnterState() { }

    public override void UpdateState() {
        CheckSwitchStates();
    }

    public override void FixedUpdateState() { }

    public override void ExitState() { }

    public override void CheckSwitchStates() {
        if (Ctx.InputManager.IsJumpPressed) {
            SwitchStates(Factory.Jump());
        }
    }

    public override void InitializeSubState() {
        if (Ctx.InputManager.movementInput == Vector2.zero) {
            SwitchStates(Factory.Idle());
        }else if (!Ctx.InputManager.IsSprintPressed) {
            SwitchStates(Factory.Walk());
        }else {
            SwitchStates(Factory.Run());
        }
    }
}
