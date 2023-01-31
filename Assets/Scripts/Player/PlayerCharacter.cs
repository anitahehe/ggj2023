using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.InputSystem;

public class PlayerCharacter : MonoBehaviour
{
	// input 
	PlayerInput playerInput;
	InputActionMap actionMap;
	InputAction moveAction;
	InputAction jumpAction;
	int isMovingHash;
	int movementHash;
	int isJumpingHash;
	int jumpVelocityHash;
	bool isMovePressed;
	bool isJumpPressed = false;

	// controllers
	Animator animator;
	CharacterController controller;
	Camera mainCam;

	// movement
	Vector2 currentMoveInput;
	Vector3 currentMove;
	Vector3 appliedMove;
	float targetAngle;
	float rotationRef;	
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

	// gravity
	float gravity;

	private void Awake()
	{
		// get components
		playerInput = GetComponent<PlayerInput>();
		controller = GetComponent<CharacterController>();
		animator = GetComponentInChildren<Animator>();
		mainCam = Camera.main;

		// get action maps & actions
		actionMap = playerInput.currentActionMap;
		moveAction = actionMap.FindAction("Move");
		jumpAction = actionMap.FindAction("Jump");

		// assign hashes
		isMovingHash = Animator.StringToHash("isMoving");
		movementHash = Animator.StringToHash("moveVelocity");
		isJumpingHash = Animator.StringToHash("isJumping");
		jumpVelocityHash = Animator.StringToHash("yVelocity");

		// listen to input
		moveAction.started += OnMovementInput;
		moveAction.canceled += OnMovementInput;
		moveAction.performed += OnMovementInput;
		jumpAction.started += OnJumpInput;
		jumpAction.canceled += OnJumpInput;

		SetupJumpVariables();

	}

	void SetupJumpVariables()
	{
		// calculate jump gravities and velocities
		float timeToApex = maxAscendDuration;
		gravity = (-2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);
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

		Vector3 adjustedMove = (right * currentMoveInput.x + forward * currentMoveInput.y);

		currentMove.x = adjustedMove.x;
		currentMove.z = adjustedMove.z;

		isMovePressed = currentMoveInput != Vector2.zero;
	}

	void OnJumpInput(InputAction.CallbackContext context)
	{
		isJumpPressed = context.ReadValueAsButton();
	}
	
	private void OnEnable()
	{
		actionMap.Enable();
	}

	private void OnDisable()
	{
		actionMap.Disable();
	}

	private void Update()
	{
		HandleJump();

		// move character based on input variables
		appliedMove.x = currentMove.x;
		appliedMove.z = currentMove.z;
		controller.Move(appliedMove * moveSpeed * Time.deltaTime);
		
		HandleAnimation();

		HandleGravity();
	}

	void HandleAnimation()
	{
		bool isMoving = animator.GetBool(isMovingHash);
		float moveVelocity = animator.GetFloat(movementHash);

		// set animation bools
		if (isMovePressed)
		{
			if (!isMoving)
				animator.SetBool(isMovingHash, true);

			animator.SetFloat(movementHash, (appliedMove * moveSpeed).magnitude);
		}
		else if (isMoving)
		{
			animator.SetBool(isMovingHash, false);
		}


		// find new rotation if we are moving
		if (currentMove.magnitude >= 0.1f)
			targetAngle = Mathf.Atan2(currentMove.x, currentMove.z) * Mathf.Rad2Deg;


		float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref rotationRef, rotationTime);
		transform.rotation = Quaternion.Euler(0f, angle, 0f);
	}

	void HandleGravity()
	{
		bool isFalling = currentMove.y <= 0f || !isJumpPressed;

		// we add a little gravity so the player properly grounds
		if (controller.isGrounded)
		{
			animator.SetBool(isJumpingHash, false);
			float groundedGravity = -0.05f;
			currentMove.y = groundedGravity;
			appliedMove.y = groundedGravity;
		}
		// while falling, use fall multiplier
		else if (isFalling)
		{
			float prevYVelocity = currentMove.y;
			currentMove.y = currentMove.y + (gravity * fallMultiplier * Time.deltaTime);
			appliedMove.y = Mathf.Max((prevYVelocity + currentMove.y) * 0.5f, maxFallSpeed);
		}
		// while jumping, stick to normal velocity
		else
		{
			float prevYVelocity = currentMove.y;
			currentMove.y = currentMove.y + (gravity * Time.deltaTime);
			appliedMove.y = (prevYVelocity + currentMove.y) * 0.5f;
		}

		// animate via velocity
		animator.SetFloat(jumpVelocityHash, currentMove.y);
	}

	void HandleJump()
	{
		if (!isJumping && controller.isGrounded && isJumpPressed)
		{
			isJumping = true;
			animator.SetBool(isJumpingHash, true);
			currentMove.y = standardJumpVelocity;
			appliedMove.y = standardJumpVelocity;
		}
		// refresh jump if grounded
		else if (!isJumpPressed && isJumping && controller.isGrounded)
		{
			isJumping = false;
		}
	}


	//   CharacterController controller;
	//private Vector3 playerVelocity;

	//[SerializeField] float playerSpeed = 2.0f;
	//[SerializeField] float jumpHeight = 1.0f;
	//[SerializeField] float jumpDuration = 0.5f;
	//[SerializeField] float maxFallSpeed = 5f;
	//[SerializeField] float timeToReachMaxFall;
	//[SerializeField] float rotationTime = 0.1f;
	//[SerializeField] float groundForgiveness;
	//[SerializeField] AnimationCurve jumpCurve;
	//[SerializeField] AnimationCurve fallCurve;
	//[SerializeField] Animator modelAnim;
	//[SerializeField] float runAnimMod;

	//float turnSmoothVelocity;
	//float targetAngle;

	//public Transform lookTarget;

	//[HideInInspector] public Camera mainCam;
	//Vector2 moveInput;
	//Vector3 move;
	//Vector3 currentForward;
	//[ShowInInspector] [ReadOnly] bool grounded;
	//[ShowInInspector] [ReadOnly] bool jumpPressed;
	//[ShowInInspector][ReadOnly] bool jumping = false;
	//IEnumerator jumpRoutine;
	//IEnumerator fallRoutine;

	//// Start is called before the first frame update
	//void Start()
	//{ 
	//       controller = GetComponent<CharacterController>();
	//	mainCam = Camera.main;
	//   }

	//public void OnMove(InputValue value)
	//{
	//	moveInput = value.Get<Vector2>();
	//}

	//void OnJump()
	//{
	//	Debug.Log("we jopmpin");
	//	if (grounded)
	//		jumpPressed = true;
	//}

	//void Update()
	//{

	//}

	//IEnumerator AirRoutine(bool jump)
	//{
	//	jumping = jump;

	//	float timer = jump ? 0 : jumpDuration;

	//	while (timer < jumpDuration)
	//	{
	//		playerVelocity.y = jumpCurve.Evaluate(timer / jumpDuration) * jumpHeight;
	//		timer += Time.deltaTime;
	//		yield return null;
	//	}

	//	timer = 0;
	//	while (!grounded && timer < timeToReachMaxFall)
	//	{
	//		playerVelocity.y = fallCurve.Evaluate(timer / timeToReachMaxFall) * maxFallSpeed;
	//		timer += Time.deltaTime;
	//		yield return null;
	//	}

	//	while (!grounded)
	//	{
	//		playerVelocity.y = maxFallSpeed;
	//		yield return null;
	//	}

	//	jumpRoutine = null;
	//	fallRoutine = null;
	//}

	//void GroundCheck()
	//{
	//	grounded = Physics.Raycast(transform.position, Vector3.down, controller.bounds.extents.y + groundForgiveness);
	//	Color rayColor = grounded ? Color.green : Color.red;
	//	Debug.DrawRay(transform.position, Vector3.down * (controller.bounds.extents.y + groundForgiveness), rayColor);
	//}
}
