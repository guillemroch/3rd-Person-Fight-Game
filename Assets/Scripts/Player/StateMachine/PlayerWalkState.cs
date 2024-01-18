using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalkState : PlayerBaseState
{
    public PlayerWalkState(PlayerStateMachine currentCtx, PlayerStateFactory stateFactory) : base(currentCtx, stateFactory) { }
    public override void EnterState() {
        //Setup starting things for animation or logic
    }

    public override void UpdateState() {
        CheckSwitchStates();
    }

    public override void FixedUpdateState() {
    }

    public override void ExitState() {
    }

    public override void CheckSwitchStates() {
        if (Ctx.InputManager.movementInput == Vector2.zero) {
            SwitchStates(Factory.Idle());
        }
        else if (Ctx.InputManager.IsSprintPressed) {
            SwitchStates(Factory.Run());
        }
    }

    public override void InitializeSubState() {
    }
}
