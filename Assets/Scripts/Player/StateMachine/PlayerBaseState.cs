using UnityEngine;

namespace Player.StateMachine{
    public abstract class PlayerBaseState
    {
        private bool _isRootState = false;
        private PlayerStateMachine _ctx;
        private PlayerStateFactory _factory;
        private PlayerBaseState _currentSuperState;
        private PlayerBaseState _currentSubState;
    
        //Getters and Setters
        protected PlayerStateMachine Ctx { get { return _ctx; } }
        protected PlayerStateFactory Factory { get { return _factory; }}
        protected bool IsRootState { get { return _isRootState; } set => _isRootState = value; }
        protected PlayerBaseState CurrentSubState { get => _currentSubState; } //TODO: remove this getter (it's just for debug purposes)
        protected PlayerBaseState CurrentSuperState { get => _currentSuperState; } //TODO: remove this getter (it's just for debug purposes)

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
            _currentSubState?.UpdateStates();
        }
        
        public void ExitStates() {
            ExitState();
            _currentSubState?.ExitStates();
        }

        protected void SwitchStates(PlayerBaseState newState) {
            //current state exits state

            if (newState.IsRootState) {
                
                ExitState();
                
                if (_currentSubState != null) {
                    _currentSubState.ExitStates();
                }
                
                newState.EnterState();
                
                Debug.Log("Switching root state to " + newState.GetType());
                //switch current state of context
                Ctx.CurrentState = newState;
                
            }else {
                if (_currentSuperState != null) {
                //switch current sub state of super state
                _currentSuperState.SetSubStates(newState);
                } else if (_isRootState) {
                    //switch current state of context
                    SetSubStates(newState);
                }
                
            }
        
        }

        protected void SetSuperState(PlayerBaseState newSuperState) {
            _currentSuperState = newSuperState;
        }

        protected void SetSubStates(PlayerBaseState newSubState) {
            _currentSubState?.ExitStates();
            _currentSubState = newSubState;
            newSubState.SetSuperState(this);
            
            _currentSubState.EnterState();
        }
    
    }
}
