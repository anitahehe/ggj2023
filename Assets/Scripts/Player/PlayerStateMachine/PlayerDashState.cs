using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor.Rendering;
using UnityEngine;

public class PlayerDashState : PlayerBaseState
{
	bool dashComplete = false;
	float timer;
	float currentX;
	float currentZ;

	public PlayerDashState(PlayerStateController context, PlayerStateFactory factory) : base(context, factory)
	{
	}

	public override void CheckSwitchStates()
	{
		if (!ctx.IsMovePressed)
			SwitchState(Factory.Idle());
		else
			SwitchState(Factory.Move());
	}

	public override void EnterState()
	{
		Debug.Log("Entering dash state");
		timer = 0;

		// get the input direction 
		currentX = ctx.CurrentMoveX;
		currentZ = ctx.CurrentMoveZ;

		// calculate a vector of the final position at the end of the dash
		Vector2 endPos = new Vector2(ctx.CurrentMoveX, ctx.CurrentMoveZ).normalized * ctx.DashDistance;

	}

	public override void ExitState()
	{

	}

	public override bool HandleGravity()
	{
		ctx.AppliedMoveY = 0f;
		return true;
	}

	public void HandleDash()
	{
		timer += Time.deltaTime;

		if (timer >= ctx.dashDuration)
		{
			dashComplete = true;
			CheckSwitchStates();
			return;
		}
	}

	public override void InitializeSubState()
	{

	}

	public override void UpdateState()
	{
		HandleDash();
	}
}
