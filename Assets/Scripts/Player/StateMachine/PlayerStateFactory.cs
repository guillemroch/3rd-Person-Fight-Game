using Player.StateMachine.States.Air;
using Player.StateMachine.States.Ground;
using Player.StateMachine.States.Lash;
using Player.StateMachine.States.Normal;
using Player.StateMachine.States.Stormlight;

namespace Player.StateMachine{
    public class PlayerStateFactory
    {
        private PlayerStateMachine _context;

        public PlayerStateFactory(PlayerStateMachine currentContext) {
            _context = currentContext;
        }
    
        //Root States
        public PlayerBaseState Normal() {
            return new PlayerNormalState(_context, this);
        }

        public PlayerBaseState Stormlight() {
            return new PlayerStormlightState(_context, this);
        }
        //Ground States
        public PlayerBaseState Grounded() {
            return new PlayerGroundedState(_context,this);
        }
        public PlayerBaseState Idle() {
            return new PlayerIdleState(_context, this);
        }
        
        public PlayerBaseState Run() {
            return new PlayerRunState(_context,this );
        }
        public PlayerBaseState Walk() {
            return new PlayerWalkState(_context,this );
        }

        public PlayerBaseState Sprint() {
            return new PlayerSprintState(_context,this );
        }
        
        //Air States
        public PlayerBaseState Air() {
            return new PlayerAirState(_context,this );
        }
        public PlayerBaseState Jump() {
            return new PlayerJumpState(_context,this );
        }
        public PlayerBaseState Fall() {
            return new PlayerFallState(_context,this );
        }
        public PlayerBaseState Land() {
            return new PlayerLandState(_context,this );
        }
        
        //Lash States
        public PlayerBaseState Lashing() {
            return new PlayerLashingState(_context,this );
        }
        public PlayerBaseState Halflash() {
            return new PlayerHalflashState(_context,this );
        }

        public PlayerBaseState Lash() {
            return new PlayerLashState(_context,this );
        }
        
        
        
        
        
    
    }
}
