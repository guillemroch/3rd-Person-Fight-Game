using System.Collections;
using System.Collections.Generic;
using Player.StateMachine;
using UnityEngine;

public class PlayerLandState : PlayerBaseState
{
    public PlayerLandState(PlayerStateMachine currentCtx, PlayerStateFactory stateFactory) : base(currentCtx, stateFactory) { }
    public override void EnterState() {
        Debug.Log("Entered Land Sub State with parent state: " + CurrentSuperState.GetType());

        Ctx.animatorManager.PlayTargetAnimation("Land", true);
    }

    public override void UpdateState() {
        CheckSwitchStates();
    }

    public override void FixedUpdateState() {
    }

    public override void ExitState() {

    }

    public override void CheckSwitchStates() {
        SwitchStates(Factory.Grounded());
    }

    public override void InitializeSubState() {
    }
}
