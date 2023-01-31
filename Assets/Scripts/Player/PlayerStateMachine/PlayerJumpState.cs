using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerBaseState, IRootState
{

	public PlayerJumpState(PlayerStateController context, PlayerStateFactory factory) : base(context, factory)
	{
		IsRoot = true;
	}

	public override void CheckSwitchStates()
	{
		if (ctx.Character.isGrounded)
		{
			SwitchState(Factory.Grounded());
		}
	}

	public override void EnterState()
	{
		InitializeSubState();
		HandleJump();
	}

	public override void ExitState()
	{
		ctx.Animator.SetBool(ctx.IsJumpingHash, false);

		if (ctx.IsJumpPressed)
		{
			ctx.RequireNewJumpPress = true;
		}
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

	void HandleJump()
	{
		ctx.IsJumping = true;
		ctx.Animator.SetBool(ctx.IsJumpingHash, true);
		ctx.CurrentMoveY = ctx.StandardJumpVelocity;
		ctx.AppliedMoveY = ctx.StandardJumpVelocity;
	}

	public override bool HandleGravity()
	{
		// if a substate handles gravity for us, stop here
		if (base.HandleGravity())
		{
			return true;
		}

		bool isFalling = ctx.CurrentMoveY <= 0f || !ctx.IsJumpPressed;

		// while falling, use fall multiplier
		if (isFalling)
		{
			float prevYVelocity = ctx.CurrentMoveY;
			ctx.CurrentMoveY = ctx.CurrentMoveY + (ctx.StandardJumpGravity * ctx.fallMultiplier * Time.deltaTime);
			ctx.AppliedMoveY = Mathf.Max((prevYVelocity + ctx.CurrentMoveY) * 0.5f, ctx.maxFallSpeed);
		}
		// while jumping, stick to normal velocity
		else
		{
			float prevYVelocity = ctx.CurrentMoveY;
			ctx.CurrentMoveY = ctx.CurrentMoveY + (ctx.StandardJumpGravity * Time.deltaTime);
			ctx.AppliedMoveY = (prevYVelocity + ctx.CurrentMoveY) * 0.5f;
		}

		// animate via velocity
		ctx.Animator.SetFloat(ctx.JumpVelocityHash, ctx.AppliedMoveY);

		// if we handled gravity, return true;
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
