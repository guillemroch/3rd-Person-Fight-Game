using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Player.StateMachine.States.Attack{

    public class PlayerLightAttackState : PlayerBaseState{
        public PlayerLightAttackState(PlayerStateMachine currentCtx, PlayerStateFactory stateFactory) :
            base(currentCtx, stateFactory, "LightAttack") { }

        
        public override void EnterState() {
            Ctx.Spear.SetActive(true);
            Ctx.InputManager.ResetLightAttackInput();
            
            Ctx.AnimatorManager.PlayTargetAnimation("Spear Attack 4");
            Ctx.AnimatorManager.animator.SetLayerWeight(Ctx.AnimatorManager.animator.GetLayerIndex("Spear"), 1);
            
        }

        public override void UpdateState() {
            CheckSwitchStates();
        }

        public override void FixedUpdateState() {
        }

        public override void ExitState() {
            Ctx.AnimatorManager.animator.SetBool("End", false);
            Ctx.Spear.SetActive(false);
        }

        public override void CheckSwitchStates() {
            if (Ctx.AnimatorManager.animator.GetBool("End")) {
                SwitchStates(Factory.Grounded());
            } 
        }

        public override void InitializeSubState() {
        }
    }
}
