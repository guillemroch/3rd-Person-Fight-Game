using System.Collections;
using System.Collections.Generic;
using Player.StateMachine;
using UnityEngine;

public class PlayerRunState : PlayerBaseState
{
    public PlayerRunState(PlayerStateMachine currentCtx, PlayerStateFactory stateFactory) : base(currentCtx, stateFactory) { }
    public override void EnterState() {
        //Setup starting things for animation or logic
        Debug.Log("Entered Run Sub State with parent state: " + CurrentSuperState.GetType());


    }

    public override void UpdateState() {
        CheckSwitchStates();
        
        HandleMovement();
    }

    public override void FixedUpdateState() {
    }

    public override void ExitState() {
    }

    public override void CheckSwitchStates() {
        if (Ctx.InputManager.movementInput == Vector2.zero) {
            SwitchStates(Factory.Idle());
        }
        else if (!Ctx.InputManager.IsSprintPressed) {
            SwitchStates(Factory.Walk());
        }
    }

    public override void InitializeSubState() {
    }
    
    private void HandleMovement() {
        Ctx.moveDirection = Ctx.cameraObject.forward * Ctx.inputManager.movementInput.y + Ctx.cameraObject.right * Ctx.inputManager.movementInput.x;

        float moveDot = Vector3.Dot(Ctx.moveDirection, Ctx.gravityDirection);
        float magSquared = Ctx.gravityDirection.sqrMagnitude;
    
        Vector3 projection = (moveDot / magSquared) * Ctx.gravityDirection;
        Ctx.moveDirection += -projection;
        Ctx.moveDirection.Normalize();
        
        Ctx.playerRigidbody.AddForce(Ctx.moveDirection * Ctx.sprintingSpeed, ForceMode.Force);
    }
    
 
}
