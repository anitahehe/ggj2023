using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.InputSystem;

public class PlayerStateController : MonoBehaviour
{
	// input 
	PlayerInput playerInput;
	InputActionMap actionMap;
	InputAction moveAction;
	InputAction jumpAction;
	InputAction dashAction;
	int isMovingHash;
	int movementHash;
	int isJumpingHash;
	int jumpVelocityHash;
	bool isMovePressed;
	bool requireNewJumpPress;
	bool isJumpPressed = false;

	// controllers
	Animator animator;
	CharacterController character;
	Camera mainCam;

	// movement
	Vector2 currentMoveInput;
	Vector3 currentMove;
	Vector3 appliedMove;
	float targetAngle;
	[HideInInspector] public float rotationRef;
	[PropertyTooltip("How fast the player moves.")]
	[TabGroup("Movement")] public float moveSpeed = 10f;
	[PropertyTooltip("How quickly the player rotates. Lower numbers mean faster rotation.")]
	[TabGroup("Movement")] public float rotationTime = 1f;
	[PropertyTooltip("How hard must the player hit the joystick to cause the run animation?")]
	[TabGroup("Movement")] public float runTriggerThreshold;

	// jumps
	[TabGroup("Movement")] public float maxJumpHeight = 1;
	[PropertyTooltip("Adjust length of time player stays in the air when jumping /nThis is assuming the fall multiplier is set to 1.")]
	[TabGroup("Movement")] public float maxAscendDuration = 0.5f;
	[PropertyTooltip("Adjust the multiplier for gravity while falling.")]
	[TabGroup("Movement")] public float fallMultiplier = 2f;
	[MaxValue(-1)]
	[TabGroup("Movement")] public float maxFallSpeed = -20f;
	float standardJumpVelocity;
	bool isJumping = false;

	// dashes
	[TabGroup("Dash")] public float dashDistance;
	[TabGroup("Dash")] public float dashDuration;
	bool isDashPressed = false;
	int isDashingHash;
	bool requirenewDashPress;

	// gravity
	float standardJumpGravity;

	// states
	PlayerBaseState currentState;
	PlayerStateFactory states;

	// getters and setters
	public PlayerBaseState CurrentState { get { return currentState; } set { currentState = value; } }
	public bool IsJumpPressed { get { return isJumpPressed; } set { isJumpPressed = value; } }
	public bool RequireNewJumpPress { get { return requireNewJumpPress; } set { requireNewJumpPress = value; } }
	public bool IsJumping { get { return isJumping; } set { isJumping = value; } }
	public bool IsMovePressed { get { return isMovePressed; } }
	public int IsMovingHash { get { return isMovingHash; } }
	public int MovementHash { get { return movementHash; } }
	public Vector2 CurrentMoveInput { get { return currentMoveInput; } }
	public Animator Animator { get { return animator; } }
	public CharacterController Character { get { return character; } }
	public int IsJumpingHash { get { return isJumpingHash; } }
	public int JumpVelocityHash { get { return jumpVelocityHash; } }
	public float CurrentMoveX { get { return currentMove.x; } set { currentMove.x = value; } }
	public float CurrentMoveZ { get { return currentMove.z; } set { currentMove.z = value; } }
	public float CurrentMoveY { get { return currentMove.y; } set { currentMove.y = value; } }
	public Vector3 AppliedMove { get { return appliedMove; } set { appliedMove = value; } }
	public Vector3 CurrentMove { get { return currentMove; } }
	public float AppliedMoveX { get { return appliedMove.x; } set { appliedMove.x = value; } }
	public float AppliedMoveZ { get { return appliedMove.z; } set { appliedMove.z = value; } }
	public float AppliedMoveY { get { return appliedMove.y; } set { appliedMove.y = value; } }
	public float StandardJumpVelocity { get { return standardJumpVelocity; } }
	public float StandardJumpGravity { get { return standardJumpGravity; } }
	public float FallMultiplier { get { return fallMultiplier; } }
	public float MaxFallSpeed { get { return maxFallSpeed; } }
	public float DashDistance {  get { return dashDistance; } }
	public float DashDuration {  get { return dashDuration; } }
	public bool IsDashPressed {  get { return isDashPressed; } }



	private void Awake()
	{
		// get components
		playerInput = GetComponent<PlayerInput>();
		character = GetComponent<CharacterController>();
		animator = GetComponentInChildren<Animator>();
		mainCam = Camera.main;

		// setup states
		states = new PlayerStateFactory(this);
		currentState = states.Grounded();
		currentState.EnterState();

		// get action maps & actions
		actionMap = playerInput.currentActionMap;
		moveAction = actionMap.FindAction("Move");
		jumpAction = actionMap.FindAction("Jump");
		dashAction = actionMap.FindAction("Dash");

		// assign hashes
		isMovingHash = Animator.StringToHash("isMoving");
		movementHash = Animator.StringToHash("moveVelocity");
		isJumpingHash = Animator.StringToHash("isJumping");
		isDashingHash = Animator.StringToHash("isDashing");
		jumpVelocityHash = Animator.StringToHash("yVelocity");

		// listen to input
		moveAction.started += OnMovementInput;
		moveAction.canceled += OnMovementInput;
		moveAction.performed += OnMovementInput;
		jumpAction.started += OnJumpInput;
		jumpAction.canceled += OnJumpInput;
		dashAction.started += OnDashInput;
		dashAction.canceled += OnDashInput;

		SetupJumpVariables();
	}

	private void Start()
	{
		character.Move(appliedMove * Time.deltaTime);
	}

	void SetupJumpVariables()
	{
		// calculate jump gravities and velocities
		float timeToApex = maxAscendDuration;
		standardJumpGravity = (-2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);
		standardJumpVelocity = (2 * maxJumpHeight) / timeToApex;
	}

	// method to get movement when input occurs
	void OnMovementInput(InputAction.CallbackContext context)
	{
		currentMoveInput = context.ReadValue<Vector2>();

		Vector3 forward = mainCam.transform.forward;
		Vector3 right = mainCam.transform.right;
		forward.y = 0f;
		right.y = 0f;

		Vector3 adjustedMove = (right.normalized * currentMoveInput.x + forward.normalized * currentMoveInput.y);

		currentMove.x = adjustedMove.x;
		currentMove.z = adjustedMove.z;

		isMovePressed = currentMoveInput != Vector2.zero;
	}

	void OnJumpInput(InputAction.CallbackContext context)
	{
		isJumpPressed = context.ReadValueAsButton();
		requireNewJumpPress = false;
	}

	void OnDashInput(InputAction.CallbackContext context)
	{
		isDashPressed = context.ReadValueAsButton();
		requirenewDashPress = false;
	}

	private void OnEnable()
	{
		actionMap.Enable();
	}

	private void OnDisable()
	{
		actionMap.Disable();
	}

	// Update is called once per frame
	void Update()
    {
        currentState.UpdateStates();
	}
}
