public abstract class PlayerBaseState 
{
	bool isRoot = false;
	PlayerStateFactory factory;
	PlayerStateController _ctx;
	PlayerBaseState currentSubState;
	PlayerBaseState currentSuperState;

	protected float targetAngle;

	protected bool IsRoot { set { isRoot = value; } }
	protected PlayerStateController ctx { get { return _ctx; } }
	protected PlayerStateFactory Factory { get { return factory; } }

	public PlayerBaseState(PlayerStateController controller, PlayerStateFactory factory)
	{
		this.factory = factory;
		this._ctx = controller;
	}

	public abstract void EnterState();

	public abstract void UpdateState();

	public abstract void ExitState();

	public abstract void CheckSwitchStates();

	public abstract void InitializeSubState();

	public virtual bool HandleGravity()
	{
		if (isRoot)
			return currentSubState.HandleGravity();
		else
			return false;
	}

	public void UpdateStates()
	{
		UpdateState();
		if (currentSubState != null)
			currentSubState.UpdateState();
	}

	protected void SwitchState(PlayerBaseState newState) 
	{
		// currrent state exits
		ExitState();

		// new state enters
		newState.EnterState();

		// switch current state of context
		if (isRoot)
			ctx.CurrentState = newState;
		else if (currentSuperState != null)
			currentSuperState.SetSubState(newState);
	}

	protected void SetSuperState(PlayerBaseState newSuperState)
	{
		currentSuperState = newSuperState;
	}

	protected void SetSubState(PlayerBaseState newSubState)
	{
		currentSubState = newSubState;
		newSubState.SetSuperState(this);
	}

}
