
public abstract class PlayerBaseState
{
    private bool _isRootState = false;
    private PlayerStateMachine _ctx;
    private PlayerStateFactory _factory;
    private PlayerBaseState _currentSuperState;
    private PlayerBaseState _currentSubState;
    
    //Getters and Setters
    protected PlayerStateMachine Ctx => _ctx;
    protected PlayerStateFactory Factory => _factory;
    protected bool IsRootState { set { _isRootState = value; } }
    

    public PlayerBaseState(PlayerStateMachine currentCtx, PlayerStateFactory stateFactory) {
        _ctx = currentCtx;
        _factory = stateFactory;
    }

    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void FixedUpdateState();
    public abstract void ExitState();
    public abstract void CheckSwitchStates();
    public abstract void InitializeSubState();

    public void UpdateStates() {
        UpdateState();
        _currentSubState?.UpdateState();
    }

    protected void SwitchStates(PlayerBaseState newState) {
        //current state exits state
        ExitState();

        //new state enters state
        newState.EnterState();

        if (_isRootState) {
            //switch current state of context
            _ctx.CurrentState = newState;
        }else if (_currentSuperState != null) {
            //switch current sub state of super state
            _currentSuperState.SetSubStates(newState);
        }
        
    }

    protected void SetSuperState(PlayerBaseState newSuperState) {
        _currentSuperState = newSuperState;
    }

    protected void SetSubStates(PlayerBaseState newSubState) {
        _currentSubState = newSubState;
        newSubState.SetSuperState(this);
    }
    
}
