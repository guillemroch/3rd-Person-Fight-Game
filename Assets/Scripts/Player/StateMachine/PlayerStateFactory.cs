namespace Player.StateMachine{
    public class PlayerStateFactory
    {
        private PlayerStateMachine _context;

        public PlayerStateFactory(PlayerStateMachine currentContext) {
            _context = currentContext;
        }
    
        //Ground States
        public PlayerBaseState Grounded() {
            return new PlayerGroundedState(_context,this );
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
        
    
    }
}
