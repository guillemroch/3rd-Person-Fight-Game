using System.Collections;
using System.Collections.Generic;
using Player.StateMachine;
using UnityEngine;

public class PlayerGroundedState : PlayerBaseState {
    
    //TODO: Change Run and Sprint substates to be reccurrent to animation speeds
    public PlayerGroundedState(PlayerStateMachine currentCtx, PlayerStateFactory stateFactory)
        : base(currentCtx, stateFactory) {
        IsRootState = true;
        InitializeSubState();
        
    }

    public override void EnterState() {
        Debug.Log("Enter Grounded Root State with substate: " + CurrentSubState?.GetType());
    }

    public override void UpdateState() {
        CheckSwitchStates();
        HandleRotation();
        HandleGroundDetection();
        HandleGravity();
    }

    public override void FixedUpdateState() { }

    public override void ExitState() {
    }

    public override void CheckSwitchStates() {
        if (Ctx.InputManager.IsJumpPressed || !Ctx.isGrounded) {
            SwitchStates(Factory.Air());
        } 
        if (Ctx.InputManager.HalfLashInput) {
            SwitchStates(Factory.Lash());
        }
    }

    public override void InitializeSubState() {
        
        if (Ctx.InputManager.movementInput == Vector2.zero) {
            SwitchStates(Factory.Idle());
        }else if (!Ctx.InputManager.IsSprintPressed) {
            SwitchStates(Factory.Walk());
        }else {
            SwitchStates(Factory.Run());
        }
    }

    public void HandleGroundDetection() {
        
        Vector3 rayCastOrigin = Ctx.transform.position - Ctx.rayCastHeightOffset * Ctx.gravityDirection;
        rayCastOrigin = Ctx.playerRigidbody.worldCenterOfMass;
        
        if (Physics.SphereCast(rayCastOrigin, Ctx.rayCastRadius, Ctx.gravityDirection, out RaycastHit hit ,Ctx.rayCastMaxDistance, Ctx.groundLayer))
        {
            Ctx.inAirTimer = 0;
            Ctx.isGrounded = true;
        }
        else
        {
            Ctx.isGrounded = false;
        }
    }
    
    private void HandleRotation()
    {
        
        Ctx.targetDirection = Vector3.zero; //Resets target direction
        
        //calculate orientation based on camera position
        Ctx.targetDirection = Ctx.cameraObject.forward * Ctx.inputManager.movementInput.y +
                              Ctx.cameraObject.right * Ctx.inputManager.movementInput.x;
        
        float moveDot = Vector3.Dot(Ctx.targetDirection, Ctx.gravityDirection);
        float magSquared = Ctx.gravityDirection.sqrMagnitude;
    
        Vector3 projection = (moveDot / magSquared) * Ctx.gravityDirection;
        Ctx.targetDirection += -projection;
        Ctx.targetDirection.Normalize();
        
        if (Ctx.targetDirection == Vector3.zero)
            Ctx.targetDirection = Ctx.playerTransform.forward;


        Quaternion targetRotation = Quaternion.LookRotation(Ctx.targetDirection, -Ctx.gravityDirection);

        Ctx.transform.rotation = Quaternion.Slerp(Ctx.playerTransform.rotation, targetRotation, Ctx.rotationSpeed * Time.deltaTime);
        
    }

    private void HandleGravity() {
        Ctx.playerRigidbody.AddForce(Ctx.groundedGravity * Ctx.gravityMultiplier * Ctx.gravityDirection, ForceMode.Acceleration);

    }
}
