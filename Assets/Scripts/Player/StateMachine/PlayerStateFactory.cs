using Player.StateMachine.States.Air;
using Player.StateMachine.States.Alive;
using Player.StateMachine.States.Attack;
using Player.StateMachine.States.Ground;
using Player.StateMachine.States.Infuse;
using Player.StateMachine.States.Lash;
using Player.StateMachine.States.Root;

namespace Player.StateMachine{
    public class PlayerStateFactory
    {
        private PlayerStateMachine _context;

        public PlayerStateFactory(PlayerStateMachine currentContext) {
            _context = currentContext;
        }
    
        //Root States
        public PlayerBaseState Alive() {
            return new PlayerAliveState(_context, this);
        }

        public PlayerBaseState Death() {
            return new PlayerDeathState(_context, this);
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
        public PlayerBaseState Dash() {
            return new PlayerDashState(_context, this);
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
        public PlayerBaseState Dive() {
            return new PlayerDiveState(_context,this );
        } 
        public PlayerBaseState JumpCharged() {
            return new PlayerJumpCharged(_context,this );
        }
        public PlayerBaseState JumpRunning() {
            return new PlayerJumpRunningState(_context,this );
        }
        public PlayerBaseState LandDamage() {
            return new PlayerLandDamageState(_context,this );
        }
        public PlayerBaseState LandHeavy() {
            return new PlayerLandHeavyState(_context,this );
        }
        
        //Lash States
        public PlayerBaseState Lashings() {
            return new PlayerLashingsState(_context,this );
        }
        public PlayerBaseState Halflash() {
            return new PlayerHalflashState(_context,this );
        }
        public PlayerBaseState Lash() {
            return new PlayerLashState(_context,this );
        }
        public PlayerBaseState LashDive() {
            return new PlayerLashDiveState(_context,this );
        }
        public PlayerBaseState LashDash() {
            return new PlayerLashDashState(_context,this );
        }
        
        //Infuse
        public PlayerBaseState Infuse() {
            return new PlayerInfuseState(_context,this );
        }
        public PlayerBaseState InteractIdleState() {
            return new PlayerInteractIdleState(_context,this );
        }
        public PlayerBaseState InteractWalk() {
            return new PlayerInteractingWalkState(_context,this );
        }
           
        //Attack
        public PlayerBaseState Attacks() {
            return new PlayerAttacksState(_context,this );
        }
        public PlayerBaseState LightAttack() {
            return new PlayerLightAttackState(_context,this );
        }
        public PlayerBaseState HeavyAttack() {
            return new PlayerHeavyAttackState(_context,this );
        }
        public PlayerBaseState AirLight() {
            return new PlayerAirLightAttackState(_context,this );
        }
        public PlayerBaseState AirHeavy() {
            return new PlayerAirHeavyAttackState(_context,this );
        }
        public PlayerBaseState BlockAttack() {
            return new PlayerBlockAttackState(_context,this );
        }
        public PlayerBaseState NormalBlock() {
            return new PlayerNormalBlockState(_context,this );
        }
        public PlayerBaseState StormlightBlock() {
            return new PlayerStormlightBlockState(_context,this );
        }
        public PlayerBaseState SpecialAttack() {
            return new PlayerSpecialAttackState(_context,this );
        }
        
           
           


        
        
        
        
    
    }
}
