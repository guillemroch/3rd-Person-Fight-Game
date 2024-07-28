using System;
using UnityEngine;

namespace Player.StateMachine{
    
    [Serializable]
    public abstract class PlayerBaseState
    {
        private bool _isRootState;
        private PlayerStateMachine _ctx;
        private PlayerStateFactory _factory;
        [SerializeField] public string name;
        public PlayerBaseState _currentSuperState;
        public PlayerBaseState _currentSubState;
    
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

            if (_isRootState) {
                //Debug.Log("States: [" + _currentSuperState?.name + "] ||=> [" + name + "] ||=> [" + _currentSubState?.name +  "] ||=> [" + _currentSubState?._currentSubState?.name + "]");
            }
        }
        
        public void ExitStates() {
            ExitState();
            _currentSubState?.ExitStates();
        }

        protected void SwitchStates(PlayerBaseState newState) {

            if (CurrentSuperState == null && newState.IsRootState) {
               ExitStates();
               Ctx.CurrentState = newState;
               Ctx.CurrentState.EnterState();

            }else if (newState.IsRootState) {
               Ctx.CurrentState.SetSubStates(newState); 
            }else if (!IsRootState) {
                CurrentSuperState.SetSubStates(newState);
            }
            else {
                Debug.LogWarning("Somethings went wrong: " + this);
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
