using System;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
using UnityEngine.UI;
#endif

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class ThirdPersonController : MonoBehaviour
    {
        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 2.0f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 5.335f;

        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;
        [Tooltip("The height the player can jump")]
        public float TransformationJumpHeight = 1.2f;

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;
        public float BatGlidingSpeed = -0.5f;
        public float BatFlyingSpeed = 1;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.50f;
        
        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        public float BatTimeout = 5f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;

        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 70.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -30.0f;

        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        public float CameraAngleOverride = 0.0f;

        public float LookSensibility = 0.0f;

        [Tooltip("For locking the camera position on all axis")]
        public bool LockCameraPosition = false;
        
        [SerializeField] GameObject _humanoidGameObject;
        [SerializeField] GameObject _batGameObject;
        [SerializeField] Image _staminaBar;
 
        // cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        // player

        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationSpeed;
        private float _horizontalSpeed;
        private float _verticalSpeed;
        private float _terminalVerticalSpeed = 53.0f;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;
        private float _batTimeoutDelta;

        // animation IDs
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
        private PlayerInput _playerInput;
#endif
        [SerializeField] private Animator _animator;
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;

        private const float _threshold = 0.01f;

        private bool _isBat;
        [SerializeField] private float _flyingStaminaMultiplier;

        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
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
                _mainCamera = Camera.main.gameObject;
            }
        }

        private void Start()
        {
            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
            
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
            _playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif
            TransformInHuman();
            AssignAnimationIDs();
            _input.OnJumpPressed += ProccesJumpInput;
            // reset our timeouts on start
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;
            _batTimeoutDelta = BatTimeout;
        }

        private void Update()
        {   
            ProcessVerticalSpeed();
            ProcessHorizontalSpeed();
            ProcessRotation();
            ApplyMove();
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        private void ProcessVerticalSpeed()
        {
            UpdateGroundedCheck();

            if (Grounded)
            {
                ProccessGrounded();
            }
            else
            {
                ProccesFall();
            }

            if (_isBat)
            {
                Glide();
                ProcessBatTimer();
            }
            else
            {
                ApplyGravity();
            }
        }

        private void ProcessHorizontalSpeed()
        {
            // set target speed based on move speed, sprint speed and if sprint is pressed
            float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                _horizontalSpeed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * SpeedChangeRate);

                // round speed to 3 decimal places
                _horizontalSpeed = Mathf.Round(_horizontalSpeed * 1000f) / 1000f;
            }
            else
            {
                _horizontalSpeed = targetSpeed;
            }

            ProcessAnimationBlend(targetSpeed, inputMagnitude);
        }

        private void ProcessAnimationBlend(float targetSpeed, float inputMagnitude)
        {
            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            // update animator if using character
            _animator?.SetFloat(_animIDSpeed, _animationBlend);
            _animator?.SetFloat(_animIDMotionSpeed, inputMagnitude);
        }

        private void ProcessRotation()
        {
            // normalise input direction
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (_input.move != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  _mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationSpeed,
                    RotationSmoothTime);

                // rotate to face input direction relative to camera position
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        }

        private void UpdateGroundedCheck()
        {   
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore) && _verticalSpeed <= 0;

            // update animator if using character
            _animator?.SetBool(_animIDGrounded, Grounded);
        }

        private void CameraRotation()
        {
            // if there is an input and camera position is not fixed
            if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
            {
                //Don't multiply mouse input by Time.deltaTime;
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier * LookSensibility;
                _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier * LookSensibility;
            }

            // clamp our rotations so our values are limited 360 degrees
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Cinemachine will follow this target
            CinemachineCameraTarget.transform.rotation =
                 Quaternion.Euler(CinemachineCameraTarget.transform.rotation.eulerAngles.x,
                _cinemachineTargetYaw, 0.0f);
        }

      
        private void Glide()
        {
            if(_input.flying)
            {
                _batTimeoutDelta = Mathf.Max(0.0f, _batTimeoutDelta - _flyingStaminaMultiplier * Time.deltaTime);
                _verticalSpeed = BatFlyingSpeed;
            }
            else
            {
                _batTimeoutDelta = Mathf.Max(0.0f, _batTimeoutDelta - Time.deltaTime);
            }
           

            ProcessBatTimer();
            _verticalSpeed = Math.Max(BatGlidingSpeed, _verticalSpeed + Gravity * Time.deltaTime);
        }

        private void ProcessBatTimer()
        {
            _staminaBar.fillAmount = Mathf.InverseLerp(0.0f, BatTimeout, _batTimeoutDelta);  
            if( _batTimeoutDelta <= 0.0f )
            {
                TransformInHuman();
            }
        }

        private void TransformInBat()
        {
            if (_batTimeoutDelta <= 0.0f)
                return;

            _staminaBar.gameObject.SetActive(true);
            _batGameObject.SetActive(true);
            _humanoidGameObject.SetActive(false);
            _isBat = true;
            _verticalSpeed = Mathf.Sqrt(TransformationJumpHeight * -2f * Gravity);
        }

        private void TransformInHuman()
        {
            _staminaBar.gameObject.SetActive(false);
            _batGameObject.SetActive(false);
            _humanoidGameObject.SetActive(true);
            _isBat = false;
        }

        private void ProccessGrounded()
        {
            TransformInHuman();

            // reset the fall timeout timer
            _fallTimeoutDelta = FallTimeout;
            _batTimeoutDelta = BatTimeout;

            // update animator if using character
            _animator?.SetBool(_animIDJump, false);
            _animator?.SetBool(_animIDFreeFall, false);

            // stop our velocity dropping infinitely when grounded         
            _verticalSpeed = Mathf.Max(0.0f, _verticalSpeed - 2f);

            // jump timeout
            _jumpTimeoutDelta = Mathf.Max(0.0f, _jumpTimeoutDelta - Time.deltaTime);
        }


        void ProccesJumpInput()
        {
            if(Grounded && !_isBat)
            {
                Jump();
            }
            else if(!Grounded && !_isBat)
            {
                TransformInBat();
            }
        }

        private void Jump()
        {
            if (_jumpTimeoutDelta > 0.0f)
                return;
            // the square root of H * -2 * G = how much velocity needed to reach desired height
            _verticalSpeed = Mathf.Sqrt(JumpHeight * -2f * Gravity);
            _animator?.SetBool(_animIDJump, true);
        }

        private void ProccesFall()
        {
            _jumpTimeoutDelta = JumpTimeout;
 
            _fallTimeoutDelta -= Mathf.Max(0.0f, _fallTimeoutDelta - Time.deltaTime);
            _animator?.SetBool(_animIDFreeFall, _fallTimeoutDelta <= 0.0f);
        }

        private void ApplyGravity()
        {
            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if(_verticalSpeed < _terminalVerticalSpeed)
            {
                _verticalSpeed +=  Gravity * Time.deltaTime;
            }
        }

        private void ApplyMove()
        {
            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
            // move the player
            _controller.Move(targetDirection.normalized * (_horizontalSpeed * Time.deltaTime) +
            Vector3.up * _verticalSpeed * Time.deltaTime);
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
            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
                GroundedRadius);
        }

        
    }


}