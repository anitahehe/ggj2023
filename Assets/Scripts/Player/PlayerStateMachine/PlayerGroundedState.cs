using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.TextCore.Text;

public class PlayerGroundedState : PlayerBaseState, IRootState
{
	public PlayerGroundedState(PlayerStateController context, PlayerStateFactory factory) : base (context, factory)
	{
		IsRoot = true;
	}

	public override void CheckSwitchStates()
	{
		if (ctx.IsJumpPressed && !ctx.RequireNewJumpPress)
		{
			SwitchState(Factory.Jump());
		}
		else if (!ctx.Character.isGrounded)
			SwitchState(Factory.Fall());
	}

	public override void EnterState()
	{
		InitializeSubState();
		HandleGravity();
	}

	public override bool HandleGravity()
	{
		if (base.HandleGravity())
			return true;

		// -0.05f is so the character controller doesn't bork grounding logic
		ctx.CurrentMoveY = -0.5f;
		ctx.AppliedMoveY = -0.5f;
		return true;
	}

	public void HandleMovementVariables()
	{
		// move character based on input variables
		ctx.Character.Move(ctx.AppliedMove * Time.deltaTime);

		// send horizontal movement values to animator
		ctx.Animator.SetFloat(ctx.MovementHash, ctx.AppliedMove.magnitude);

		// find new rotation if we are moving
		if (ctx.CurrentMove.magnitude >= 0.1f)
			targetAngle = Mathf.Atan2(ctx.CurrentMove.x, ctx.CurrentMove.z) * Mathf.Rad2Deg;

		float angle = Mathf.SmoothDampAngle(ctx.transform.eulerAngles.y, targetAngle, ref ctx.rotationRef, ctx.rotationTime);
		ctx.transform.rotation = Quaternion.Euler(0f, angle, 0f);
	}

	public override void ExitState()
	{	
	}

	public override void InitializeSubState()
	{
		if (!ctx.IsMovePressed)
		{
			SetSubState(Factory.Idle());
		}
		else
		{
			SetSubState(Factory.Move());
		}
	}

	public override void UpdateState()
	{
		CheckSwitchStates();
		HandleMovementVariables();
	}
}
