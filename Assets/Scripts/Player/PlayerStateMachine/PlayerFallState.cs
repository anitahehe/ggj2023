using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFallState : PlayerBaseState, IRootState
{
	public PlayerFallState(PlayerStateController context, PlayerStateFactory factory) : base(context, factory)
	{
		IsRoot = true;
	}

	public override void CheckSwitchStates()
	{
		if (ctx.Character.isGrounded)
			SwitchState(Factory.Grounded());
	}

	public override void EnterState()
	{
		InitializeSubState();
		ctx.Animator.SetBool(ctx.IsJumpingHash, true);
	}

	public override void ExitState()
	{
		ctx.Animator.SetBool(ctx.IsJumpingHash, false);
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
		HandleGravity();
		HandleMovementVariables();
		CheckSwitchStates();
	}

	public override bool HandleGravity()
	{
		if (base.HandleGravity())
			return true;

		float prevYVelocity = ctx.CurrentMoveY;
		ctx.CurrentMoveY = ctx.CurrentMoveY + (ctx.StandardJumpGravity * Time.deltaTime);
		ctx.AppliedMoveY = Mathf.Max((prevYVelocity + ctx.CurrentMoveY) * 0.5f, ctx.maxFallSpeed);

		// animate via velocity
		ctx.Animator.SetFloat(ctx.JumpVelocityHash, ctx.AppliedMoveY);

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
}
