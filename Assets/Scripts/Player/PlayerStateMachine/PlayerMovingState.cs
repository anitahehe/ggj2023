using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovingState : PlayerBaseState
{
	public PlayerMovingState(PlayerStateController context, PlayerStateFactory factory) : base(context, factory)
	{

	}
	public override void CheckSwitchStates()
	{
		if (ctx.IsDashPressed)
			SwitchState(Factory.Dash());
		else if (!ctx.IsMovePressed)
			SwitchState(Factory.Idle());
	}

	public override void EnterState()
	{
		Debug.Log("Entering move state");

		ctx.Animator.SetBool(ctx.IsMovingHash, true);
	}

	public override void ExitState()
	{
	}

	public override void InitializeSubState()
	{
	}

	public override void UpdateState()
	{
		Vector2 movement = new Vector2(ctx.CurrentMoveX, ctx.CurrentMoveZ) * ctx.moveSpeed;
		ctx.AppliedMoveX = movement.x;
		ctx.AppliedMoveZ = movement.y;
		CheckSwitchStates();
	}
}
