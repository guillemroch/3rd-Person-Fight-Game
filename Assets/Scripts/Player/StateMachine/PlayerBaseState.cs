using System;
using UnityEngine;

namespace Player.StateMachine{
    
    [Serializable]
    public abstract class PlayerBaseState
    {
        private bool _isRootState;
        private PlayerStateMachine _ctx;
        private PlayerStateFactory _factory;
        [SerializeField]
        public string name;
        public PlayerBaseState _currentSuperState;
        private PlayerBaseState _currentSubState;
    
        //Getters and Setters
        protected PlayerStateMachine Ctx { get { return _ctx; } }
        protected PlayerStateFactory Factory { get { return _factory; }}
        protected bool IsRootState { get { return _isRootState; } set => _isRootState = value; }
        protected PlayerBaseState CurrentSubState { get => _currentSubState; } 
        protected PlayerBaseState CurrentSuperState { get => _currentSuperState; } 

        public PlayerBaseState(PlayerStateMachine currentCtx, PlayerStateFactory stateFactory, string name) {
            _ctx = currentCtx;
            _factory = stateFactory;
            this.name = name;
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

            if (newState.IsRootState) {
                
                ExitState();
                
                if (_currentSubState != null) {
                    _currentSubState.ExitStates();
                }
                
                newState.EnterState();
                
                //Debug.Log("<color=orange>[ROOT]: " + newState.GetType() + "</color>");
                //switch current state of context
                Ctx.CurrentState = newState;
                
            }else {
                if (_currentSuperState != null) {
                //switch current sub state of super state
                //Debug.Log("<color=green>[SUB]: " + newState.GetType() + "</color>");
                _currentSuperState.SetSubStates(newState);
                } else if (_isRootState) {
                    //switch current state of context
                    SetSubStates(newState);
                    //Debug.Log("<color=yellow>[SUB]: " + newState.GetType() + "</color>");
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
