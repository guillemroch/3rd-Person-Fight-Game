using UnityEngine;

namespace Player.StateMachine.States.Air{
    public class PlayerLandState : PlayerBaseState
    {
        public PlayerLandState(PlayerStateMachine currentCtx, PlayerStateFactory stateFactory) : base(currentCtx, stateFactory, "Land") { }
        public override void EnterState() {

            //Ctx.AnimatorManager.PlayTargetAnimation("Land Blend Tree");
            //TODO: [Animation] -> set land blend tree value
            Ctx.AnimatorManager.animator.SetBool("End", false);
            //Ctx.AnimatorManager.PlayTargetAnimation("Land Blend Tree");
            Ctx.PlayerTransform.rotation *= Quaternion.FromToRotation(Ctx.PlayerTransform.up, -Ctx.GravityDirection);
        }

        public override void UpdateState() {
         
            CheckSwitchStates();
        }

        public override void FixedUpdateState() {
        }

        public override void ExitState() {
            
        }

        public override void CheckSwitchStates() {
           // Debug.Log(Ctx.AnimatorManager.animator.GetBool("End"));
            if (Ctx.AnimatorManager.animator.GetBool("End")) {
                SwitchStates(Factory.Grounded());
            }
        }

        public override void InitializeSubState() {
        }
      }
    
}
