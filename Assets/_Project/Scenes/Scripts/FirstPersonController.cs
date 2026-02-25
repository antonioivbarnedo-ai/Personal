using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	[RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM
	[RequireComponent(typeof(PlayerInput))]
#endif
	public class FirstPersonController : MonoBehaviour
	{
		[Header("Player")]
		[Tooltip("Move speed of the character in m/s")]
		public float MoveSpeed = 4.0f;
		[Tooltip("Sprint speed of the character in m/s")]
		public float SprintSpeed = 6.0f;
		[Tooltip("Rotation speed of the character")]
		public float RotationSpeed = 1.0f;
		[Tooltip("Acceleration and deceleration")]
		public float SpeedChangeRate = 10.0f;

		[Space(10)]
		[Tooltip("The height the player can jump")]
		public float JumpHeight = 1.2f;
		[Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
		public float Gravity = -15.0f;
		[Tooltip("Multiplies gravity when falling to make it feel snappy")]
		public float FallMultiplier = 2.5f;

        [Header("Climbing System")]
        [Tooltip("How fast you climb up")]
        public float ClimbSpeed = 3.0f;
        [Tooltip("Are we currently climbing?")]
        public bool IsClimbing = false;

        [Header("Immersive Head Bob")]
        [Tooltip("Enable or disable the head bob effect")]
        public bool EnableHeadBob = true;
        [Tooltip("How fast the camera bobs (Walking frequency)")]
        public float BobFrequency = 5f;
        [Tooltip("How high the camera moves up/down")]
        public float BobAmplitude = 0.05f;
        [Tooltip("How much the camera sways left/right")]
        public float BobSwayFactor = 0.5f; // Usually half of amplitude looks good
        [Tooltip("Multiplies frequency when running")]
        public float RunBobSpeedMultiplier = 1.5f;

        [Space(10)]
		[Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
		public float JumpTimeout = 0.1f;
		[Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
		public float FallTimeout = 0.15f;

		[Header("Player Grounded")]
		[Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
		public bool Grounded = true;
		[Tooltip("Useful for rough ground")]
		public float GroundedOffset = -0.14f;
		[Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
		public float GroundedRadius = 0.5f;
		[Tooltip("What layers the character uses as ground")]
		public LayerMask GroundLayers;

		[Header("Cinemachine")]
		[Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
		public GameObject CinemachineCameraTarget;
		[Tooltip("How far in degrees can you move the camera up")]
		public float TopClamp = 90.0f;
		[Tooltip("How far in degrees can you move the camera down")]
		public float BottomClamp = -90.0f;

		// cinemachine
		private float _cinemachineTargetPitch;

		// player
		private float _speed;
		private float _rotationVelocity;
		private float _verticalVelocity;
		private float _terminalVelocity = 53.0f;

		// timeout deltatime
		private float _jumpTimeoutDelta;
		private float _fallTimeoutDelta;

        // Internal variables for head bob
        private float _bobTimer;
        private float _defaultYPos;

        //ariable to track how intense the bob should be.
        private float _bobIntensity = 0f;

#if ENABLE_INPUT_SYSTEM
		private PlayerInput _playerInput;
#endif
        private CharacterController _controller;
		private StarterAssetsInputs _input;
		private GameObject _mainCamera;

		private const float _threshold = 0.01f;

		private bool IsCurrentDeviceMouse
		{
			get
			{
				#if ENABLE_INPUT_SYSTEM
				return _playerInput.currentControlScheme == "KeyboardMouse";
				#else
				return false;
				#endif
			}
		}

		private void Awake()
		{
			// get a reference to our main camera
			if (_mainCamera == null)
			{
				_mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
			}
		}

		private void Start()
		{
			_controller = GetComponent<CharacterController>();
			_input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM
			_playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

			// reset our timeouts on start
			_jumpTimeoutDelta = JumpTimeout;
			_fallTimeoutDelta = FallTimeout;
            _defaultYPos = CinemachineCameraTarget.transform.localPosition.y;
        }

        private void Update()
        {
            HandleClimbing(); // Check for climbing first

            if (!IsClimbing)
            {
                // Only apply gravity if we are NOT climbing
                JumpAndGravity();
                GroundedCheck();
                Move();
                HeadBob();
            }
            else
            {
                // If we ARE climbing, we might still want to "Move" left/right (strafe)
                // But we don't want gravity.
                // You can leave Move() here if you want to strafe around the trunk, 
                // or remove it to lock the player to the trunk.
                Grounded = false; // Fake being in air so we don't snap to ground
            }
        }

        private void LateUpdate()
		{
			CameraRotation();
		}

		private void GroundedCheck()
		{
			// set sphere position, with offset
			Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
			Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
		}

		private void CameraRotation()
		{
			// if there is an input
			if (_input.look.sqrMagnitude >= _threshold)
			{
				//Don't multiply mouse input by Time.deltaTime
				float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;
				
				_cinemachineTargetPitch += _input.look.y * RotationSpeed * deltaTimeMultiplier;
				_rotationVelocity = _input.look.x * RotationSpeed * deltaTimeMultiplier;

				// clamp our pitch rotation
				_cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

				// Update Cinemachine camera target pitch
				CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);

				// rotate the player left and right
				transform.Rotate(Vector3.up * _rotationVelocity);
			}
		}

		private void Move()
		{
            // set target speed based on move speed, sprint speed and if sprint is pressed
            // Default to walking speed
            float targetSpeed = MoveSpeed;

            // Check: Is Shift held? AND Do we have Stamina?
            // We ask the PlayerStats singleton if we can "afford" the run
            if (_input.sprint && PlayerStats.Instance.UseStamina(PlayerStats.Instance.staminaDrainRate))
            {
                targetSpeed = SprintSpeed;
            }

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

			// a reference to the players current horizontal velocity
			float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

			float speedOffset = 0.1f;
			float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

			// accelerate or decelerate to target speed
			if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
			{
				// creates curved result rather than a linear one giving a more organic speed change
				// note T in Lerp is clamped, so we don't need to clamp our speed
				_speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);

				// round speed to 3 decimal places
				_speed = Mathf.Round(_speed * 1000f) / 1000f;
			}
			else
			{
				_speed = targetSpeed;
			}

			// normalise input direction
			Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

			// note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
			// if there is a move input rotate player when the player is moving
			if (_input.move != Vector2.zero)
			{
				// move
				inputDirection = transform.right * _input.move.x + transform.forward * _input.move.y;
			}

			// move the player
			_controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
		}

        private void JumpAndGravity()
        {
            if (Grounded)
            {
                // reset the fall timeout timer
                _fallTimeoutDelta = FallTimeout;

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // Jump
                if (_input.jump && _jumpTimeoutDelta <= 0.0f)
                {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                // reset the jump timeout timer
                _jumpTimeoutDelta = JumpTimeout;

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }

                // if we are not grounded, do not jump
                _input.jump = false;
            }

            // --- NEW SNAPPY GRAVITY LOGIC START ---
            // If we are falling (velocity is negative), apply EXTRA gravity
            if (_verticalVelocity < 0)
            {
                _verticalVelocity += Gravity * (FallMultiplier - 1) * Time.deltaTime;
            }
            // --- NEW SNAPPY GRAVITY LOGIC END ---

            // apply standard gravity over time
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
		{
			if (lfAngle < -360f) lfAngle += 360f;
			if (lfAngle > 360f) lfAngle -= 360f;
			return Mathf.Clamp(lfAngle, lfMin, lfMax);
		}

		private void OnDrawGizmosSelected()
		{
			Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
			Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

			if (Grounded) Gizmos.color = transparentGreen;
			else Gizmos.color = transparentRed;

			// when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
			Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
		}

        private void HeadBob()
        {
            if (!EnableHeadBob) return;

            // Check if we are moving and grounded
            // Use _speed so we don't bob if running into a wall
            bool isMoving = Grounded && _speed > 0.1f;

            // --- THE FIX: SMOOTH INTENSITY ---
            // Instead of snapping on/off, we fade the bob strength in and out.
            // If moving, target is 1. If stopped, target is 0.
            float targetIntensity = isMoving ? 1f : 0f;

            // "5f" controls how fast it ramps up. Lower = smoother start/stop.
            _bobIntensity = Mathf.MoveTowards(_bobIntensity, targetIntensity, Time.deltaTime * 5f);

            // Only calculate math if we have some intensity (optimization)
            if (_bobIntensity > 0.01f)
            {
                // Calculate Bob Speed
                float speedMultiplier = (_input.sprint) ? RunBobSpeedMultiplier : 1f;

                // Increment timer ONLY when we are actually moving.
                // If we stop, we PAUSE the timer (don't reset to 0), so the wave doesn't snap.
                if (isMoving)
                {
                    _bobTimer += Time.deltaTime * BobFrequency * speedMultiplier;
                }

                // 1. VERTICAL BOB (Sine Wave)
                // Multiply by _bobIntensity so it starts small (prevents half-step jitter)
                float yOffset = Mathf.Sin(_bobTimer) * BobAmplitude * _bobIntensity;

                // 2. HORIZONTAL SWAY (Cosine Wave)
                float xOffset = Mathf.Cos(_bobTimer / 2) * BobSwayFactor * BobAmplitude * _bobIntensity;

                // Apply offsets
                Vector3 newPos = CinemachineCameraTarget.transform.localPosition;
                newPos.y = _defaultYPos + yOffset;
                newPos.x = xOffset;
                CinemachineCameraTarget.transform.localPosition = newPos;
            }
            else
            {
                // When intensity is basically zero, strictly reset to center to prevent drift
                Vector3 currentPos = CinemachineCameraTarget.transform.localPosition;
                Vector3 targetPos = new Vector3(0, _defaultYPos, 0);

                CinemachineCameraTarget.transform.localPosition = Vector3.Lerp(currentPos, targetPos, Time.deltaTime * 5f);
            }
        }

        [Header("Interaction")]
        public GameObject InteractPrompt;
        public float ClimbStaminaCost = 10f; // Cost per second

        // Settings for "SphereCast" (Thicker detection)
        private float _detectRadius = 0.3f;
        private float _detectDistance = 1.0f;

        private void HandleClimbing()
        {
            // 1. SPHERECAST (The "Thick" Raycast)
            Vector3 origin = transform.position + Vector3.up * 1.0f; // Chest height
            bool hitSomething = Physics.SphereCast(origin, _detectRadius, transform.forward, out RaycastHit hitInfo, _detectDistance);

            // 2. FILTERING: Is it Climbable?
            // This answers your "Wood Bricks" question:
            // ONLY objects with the tag "Climbable" will work. 
            // So if you put this tag on your Wood Bricks but NOT the tree, you can only climb the bricks!
            bool isClimbableTag = hitSomething && hitInfo.collider.CompareTag("Climbable");

            // Check the ANGLE. 
            // I lowered this to 10 degrees. This means you can climb almost anything vertical 
            // or slightly sloped, as long as it's not a flat floor.
            float wallAngle = Vector3.Angle(hitInfo.normal, Vector3.up);
            bool isWall = wallAngle > 10f;

            bool canClimb = isClimbableTag && isWall;

            // 3. UI LOGIC (Screen Space / WBP Style)
            if (InteractPrompt != null)
            {
                // We simply turn it ON or OFF. We do NOT move it anymore.
                // Make sure your InteractPrompt is inside a Screen Space Canvas.
                if (canClimb && !IsClimbing)
                {
                    InteractPrompt.SetActive(true);
                }
                else
                {
                    InteractPrompt.SetActive(false);
                }
            }

            // 4. ENTER CLIMB MODE
            if (canClimb && !IsClimbing && UnityEngine.InputSystem.Keyboard.current.eKey.wasPressedThisFrame)
            {
                IsClimbing = true;
                _verticalVelocity = 0;
            }

            // 5. CLIMBING PHYSICS loop
            if (IsClimbing)
            {
                // A. EXIT CONDITIONS

                // 1. Jump Key pressed -> Jump off
                if (_input.jump)
                {
                    IsClimbing = false;
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity); // Jump!
                    _controller.Move(transform.forward * 0.5f); // Push forward slightly
                    return;
                }

                // 2. We hit a Flat Surface (Floor) -> Auto-walk onto it
                if (!isWall && hitSomething)
                {
                    IsClimbing = false;
                    _verticalVelocity = 2f; // Small hop up
                    return;
                }

                // 3. We ran out of object (Top of ladder/brick) -> Vault up
                if (!hitSomething)
                {
                    IsClimbing = false;
                    _verticalVelocity = 5f; // Vault up
                    _controller.Move(transform.forward * 0.5f);
                    return;
                }

                // 4. Ran out of Stamina? (Force Drain Check)
                // We use the lowercase 'stamina' variable directly to be safe
                if (_input.move.magnitude > 0 && PlayerStats.Instance.currentStamina <= 0)
                {
                    IsClimbing = false;
                    return; // Fall down
                }

                // B. MOVEMENT
                _verticalVelocity = 0f; // No Gravity

                float verticalInput = _input.move.y;
                float horizontalInput = _input.move.x;

                // --- CAMERA BOB & STAMINA DRAIN ---
                if (verticalInput != 0 || horizontalInput != 0)
                {
                    // --- STAMINA FIX ---
                    // We force the math here to ensure it drains
                    PlayerStats.Instance.currentStamina -= ClimbStaminaCost * Time.deltaTime;
                    // Clamp it so it doesn't go below 0
                    if (PlayerStats.Instance.currentStamina < 0) PlayerStats.Instance.currentStamina = 0;


                    // --- ANIMATION ---
                    float clunkSpeed = 6f;
                    float clunkAmount = 0.08f;

                    float bobY = Mathf.Sin(Time.time * clunkSpeed) * clunkAmount;
                    float bobX = Mathf.Cos(Time.time * (clunkSpeed / 2f)) * (clunkAmount / 2f);

                    CinemachineCameraTarget.transform.localPosition = new Vector3(bobX, _defaultYPos + bobY, 0f);
                }
                else
                {
                    // Smooth reset when stopped
                    CinemachineCameraTarget.transform.localPosition = Vector3.Lerp(
                        CinemachineCameraTarget.transform.localPosition,
                        new Vector3(0, _defaultYPos, 0),
                        Time.deltaTime * 5f
                    );
                }

                // PHYSICS MOVEMENT
                Vector3 moveDir = Vector3.ProjectOnPlane(transform.up, hitInfo.normal).normalized * verticalInput;
                Vector3 slideDir = Vector3.ProjectOnPlane(transform.right, hitInfo.normal).normalized * horizontalInput;

                _controller.Move((moveDir + slideDir) * ClimbSpeed * Time.deltaTime);

                // FORCE STICK
                _controller.Move(transform.forward * 1.0f * Time.deltaTime);
            }
        }
    }
}