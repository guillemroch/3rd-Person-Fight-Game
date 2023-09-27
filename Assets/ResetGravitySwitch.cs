using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Player Animator StateMachine class:
 * End the gravity switch when exiting the animation
 */
public class ResetGravitySwitch : StateMachineBehaviour
{

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       animator.SetBool("isGravitySwitching", false);
    }

}
