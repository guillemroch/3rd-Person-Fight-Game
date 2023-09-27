using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Player Animator StateMachine class:
 * Sets the bool of Interacting when entering the state
 */
public class ResetBool : StateMachineBehaviour
{
    public string isInteractingBool;

    public bool isInteractingStatus;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       animator.SetBool(isInteractingBool, isInteractingStatus);
    }


}
