using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
	public PlayerIdleState(PlayerStateController context, PlayerStateFactory factory) : base(context, factory)
	{
	}

	public override void CheckSwitchStates()
    {
		if (ctx.IsDashPressed)
			SwitchState(Factory.Dash());
		else if (ctx.IsMovePressed)
            SwitchState(Factory.Move());
    }

    public override void EnterState()
    {
        Debug.Log("Entering idle state");
        ctx.Animator.SetBool(ctx.IsMovingHash, false);
        ctx.Animator.SetFloat(ctx.MovementHash, 0f);
        ctx.AppliedMoveX = 0f;
        ctx.AppliedMoveZ = 0f;
    }

    public override void ExitState()
    {
    }

    public override void InitializeSubState()
    {

    }

    public override void UpdateState()
    {
        CheckSwitchStates();
    }
}
