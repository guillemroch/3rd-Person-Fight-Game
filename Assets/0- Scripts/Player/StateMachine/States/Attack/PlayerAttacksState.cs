using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Player.StateMachine.States.Attack{

public class PlayerAttacksState : PlayerBaseState
{
    public PlayerAttacksState(PlayerStateMachine currentCtx, PlayerStateFactory stateFactory) : 
        base(currentCtx, stateFactory, "Attacks") { }
    public override void EnterState() {
       InitializeSubState(); 
    }

    public override void UpdateState() {
    }

    public override void FixedUpdateState() {
    }

    public override void ExitState() {
    }

    public override void CheckSwitchStates() {
    }

    public override void InitializeSubState() {
        if (Ctx.InputManager.LightAttack) {
            SetSubStates(Factory.LightAttack());
        }
    }
}
}
