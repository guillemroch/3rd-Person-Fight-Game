using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorManager : MonoBehaviour
{
    public Animator animator;
    //Blend Tree variables
    private int horizontal;
    private int vertical;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        horizontal = Animator.StringToHash("Horizontal");
        vertical = Animator.StringToHash("Vertical");
        
    }

    public void PlayTargetAnimation(string targetAnimation, bool isInteracting)
    {
        animator.SetBool("isInteracting", isInteracting);
        animator.CrossFade(targetAnimation, 0.2f);
    }
    public void UpdateAnimatorValues(Vector2 movementInput, bool isSprinting)
    {
        //Animation Snapping
        //Horizontal
        float snappedHorizontal;
        if ( movementInput.x > 0 && movementInput.x < 0.55f)
        {
            snappedHorizontal = 0.5f;
            
        }else if ( movementInput.x >= 0.55f)
        {
            snappedHorizontal = 1f;
        }else if ( movementInput.x < 0 && movementInput.x > -0.55f)
        {
            snappedHorizontal = -0.5f;
            
        }else if ( movementInput.x <= -0.55f)
        {
            snappedHorizontal = -1f;
        }
        else
        {
            snappedHorizontal = 0;
        }

        //Vertical
        float snappedVertical;
        if ( movementInput.y > 0 && movementInput.y < 0.55f)
        {
            snappedVertical = 0.5f;
            
        }else if ( movementInput.y >= 0.55f)
        {
            snappedVertical = 1f;
        }else if ( movementInput.y < 0 && movementInput.y > -0.55f)
        {
            snappedVertical = -0.5f;
            
        }else if ( movementInput.y <= -0.55f)
        {
            snappedVertical = -1f;
        }else
        {
            snappedVertical = 0;
        }

        if (isSprinting)
        {
            snappedVertical = 2;
            
        }
        
        
        animator.SetFloat(horizontal, snappedHorizontal, 0.1f, Time.deltaTime);
        animator.SetFloat(vertical, snappedVertical, 0.1f, Time.deltaTime);
    }
}
