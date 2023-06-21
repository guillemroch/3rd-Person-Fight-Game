using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorManager : MonoBehaviour
{
    private Animator animator;
    //Blend Tree variables
    private int horizontal;
    private int vertical;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        horizontal = Animator.StringToHash("Horizontal");
        vertical = Animator.StringToHash("Vertical");
        
    }

    public void UpdateAnimatorValues(Vector2 movementInput)
    {
        //Animation Snapping
        float snappedHorizontal = Snapping.Snap(movementInput.x, 0.5f);
        float snappedVertical = Snapping.Snap(movementInput.y, 0.5f);
        
        animator.SetFloat(horizontal, snappedHorizontal, 0.1f, Time.deltaTime);
        animator.SetFloat(vertical, snappedVertical, 0.1f, Time.deltaTime);
    }
}
