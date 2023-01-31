using System.Collections.Generic;

enum PlayerStates
{
	Idle, Move, Grounded, Jump, Fall, Dash
}
public class PlayerStateFactory 
{
	PlayerStateController context;
	Dictionary<PlayerStates, PlayerBaseState> states = new Dictionary<PlayerStates, PlayerBaseState>();

	public PlayerStateFactory(PlayerStateController currentContext)
	{
		context = currentContext;
		states[PlayerStates.Idle] = new PlayerIdleState(context, this);
		states[PlayerStates.Move] = new PlayerMovingState(context, this);
		states[PlayerStates.Grounded] = new PlayerGroundedState(context, this);
		states[PlayerStates.Jump] = new PlayerJumpState(context, this);
		states[PlayerStates.Fall] = new PlayerFallState(context, this);
		states[PlayerStates.Dash] = new PlayerDashState(context, this);
	}

	public PlayerBaseState Idle()
	{
		return states[PlayerStates.Idle];
	}
	public PlayerBaseState Move()
	{
		return states[PlayerStates.Move];
	}

	public PlayerBaseState Jump()
	{
		return states[PlayerStates.Jump];
	}
	public PlayerBaseState Grounded()
	{
		return states[PlayerStates.Grounded];
	}

	public PlayerBaseState Fall()
	{
		return states[PlayerStates.Fall];
	}

	public PlayerBaseState Dash()
	{
		return states[PlayerStates.Dash];
	}
}
