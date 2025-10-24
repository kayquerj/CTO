using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HollowKnightLike.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class PlayerController2D : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 8f;
        [SerializeField] private float acceleration = 40f;
        [SerializeField] private float deceleration = 50f;
        [SerializeField] private float airControlMultiplier = 0.5f;

        [Header("Jump")]
        [SerializeField] private float jumpForce = 14f;
        [SerializeField] private float variableJumpGravityMultiplier = 2f;
        [SerializeField] private float coyoteTime = 0.15f;
        [SerializeField] private float jumpBufferTime = 0.15f;
        [SerializeField] private int extraAirJumps = 0;

        [Header("Dash")]
        [SerializeField] private float dashSpeed = 18f;
        [SerializeField] private float dashDuration = 0.2f;
        [SerializeField] private float dashCooldown = 0.6f;
        [SerializeField] private AnimationCurve dashSpeedCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

        [Header("Wall Interaction")]
        [SerializeField] private float wallSlideSpeed = 2f;
        [SerializeField] private Vector2 wallJumpImpulse = new Vector2(12f, 14f);
        [SerializeField] private float wallJumpControlDelay = 0.2f;

        [Header("Ground Detection")]
        [SerializeField] private Transform groundCheck;
        [SerializeField] private Vector2 groundCheckSize = new Vector2(0.6f, 0.1f);
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private float slopeCheckDistance = 0.5f;

        [Header("Wall Detection")]
        [SerializeField] private Transform frontCheck;
        [SerializeField] private Vector2 wallCheckSize = new Vector2(0.25f, 1.2f);

        [Header("Input")]
        [SerializeField] private InputActionAsset actionsAsset;
        [SerializeField] private string actionMapName = "Player";
        [SerializeField] private string moveActionName = "Player/Move";
        [SerializeField] private string jumpActionName = "Player/Jump";
        [SerializeField] private string dashActionName = "Player/Dash";
        [SerializeField] private string attackActionName = "Player/Attack";

        [Header("Events")] [SerializeField] private UnityEngine.Events.UnityEvent onAttack;

        private InputActionAsset _actionsInstance;
        private InputActionMap _actionMap;
        private InputAction _moveAction;
        private InputAction _jumpAction;
        private InputAction _dashAction;
        private InputAction _attackAction;

        private Rigidbody2D _rb;
        private Vector2 _inputDirection;
        private bool _isFacingRight = true;

        private float _coyoteCounter;
        private float _jumpBufferCounter;
        private int _airJumpCounter;

        private bool _isGrounded;
        private bool _isOnWall;
        private bool _isWallSliding;
        private bool _dashing;
        private float _dashCooldownTimer;
        private Coroutine _dashRoutine;
        private float _wallJumpLock;

        private Vector2 _groundNormal = Vector2.up;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            if (actionsAsset != null)
            {
                _actionsInstance = actionsAsset;
                _actionMap = _actionsInstance.FindActionMap(actionMapName, false);
                if (_actionMap == null && !string.IsNullOrEmpty(actionMapName))
                {
                    _actionMap = _actionsInstance.FindActionMap(actionMapName, true);
                }
            }
            CacheActions();
            _rb.gravityScale = 4.5f;
        }

        private void OnEnable()
        {
            CacheActions();

            if (_actionMap != null)
            {
                _actionMap.Enable();
            }
            else
            {
                _actionsInstance?.Enable();
            }

            EnableAction(_moveAction);
            EnableAction(_jumpAction);
            EnableAction(_dashAction);
            EnableAction(_attackAction);

            if (_jumpAction != null)
            {
                _jumpAction.performed += OnJumpPerformed;
                _jumpAction.canceled += OnJumpCanceled;
            }

            if (_dashAction != null)
            {
                _dashAction.performed += OnDashPerformed;
            }

            if (_attackAction != null)
            {
                _attackAction.performed += OnAttackPerformed;
            }
        }

        private void OnDisable()
        {
            if (_jumpAction != null)
            {
                _jumpAction.performed -= OnJumpPerformed;
                _jumpAction.canceled -= OnJumpCanceled;
            }

            if (_dashAction != null)
            {
                _dashAction.performed -= OnDashPerformed;
            }

            if (_attackAction != null)
            {
                _attackAction.performed -= OnAttackPerformed;
            }

            DisableAction(_moveAction);
            DisableAction(_jumpAction);
            DisableAction(_dashAction);
            DisableAction(_attackAction);

            if (_actionMap != null)
            {
                _actionMap.Disable();
            }
            else
            {
                _actionsInstance?.Disable();
            }
        }

        private void Update()
        {
            HandleInput();
            UpdateTimers();
            CheckEnvironment();

            if (_jumpBufferCounter > 0f)
            {
                TryConsumeJump();
            }

            HandleFacing();
        }

        private void FixedUpdate()
        {
            HandleMovement();
            HandleWallSlide();
        }

        private void CacheActions()
        {
            if (_actionsInstance == null)
            {
                return;
            }

            if (_actionMap == null && !string.IsNullOrEmpty(actionMapName))
            {
                _actionMap = _actionsInstance.FindActionMap(actionMapName, false);
                if (_actionMap == null)
                {
                    _actionMap = _actionsInstance.FindActionMap(actionMapName, true);
                }
            }

            _moveAction = FindAction(moveActionName);
            _jumpAction = FindAction(jumpActionName);
            _dashAction = FindAction(dashActionName);
            _attackAction = FindAction(attackActionName);
        }

        private InputAction FindAction(string actionName)
        {
            if (string.IsNullOrWhiteSpace(actionName) || _actionsInstance == null)
            {
                return null;
            }

            var action = _actionsInstance.FindAction(actionName, false);
            if (action != null)
            {
                return action;
            }

            if (_actionMap != null)
            {
                string localName = actionName;
                int slashIndex = actionName.LastIndexOf('/');
                if (slashIndex >= 0 && slashIndex < actionName.Length - 1)
                {
                    localName = actionName.Substring(slashIndex + 1);
                }

                return _actionMap.FindAction(localName, false);
            }

            return null;
        }

        private void HandleInput()
        {
            if (_moveAction != null)
            {
                var raw = _moveAction.ReadValue<Vector2>();
                _inputDirection = new Vector2(raw.x, 0f);
            }
            else
            {
                _inputDirection = Vector2.zero;
            }
        }

        private void UpdateTimers()
        {
            if (_isGrounded)
            {
                _coyoteCounter = coyoteTime;
                _airJumpCounter = extraAirJumps;
            }
            else
            {
                _coyoteCounter -= Time.deltaTime;
            }

            if (_jumpBufferCounter > 0f)
            {
                _jumpBufferCounter -= Time.deltaTime;
            }

            if (_dashCooldownTimer > 0f)
            {
                _dashCooldownTimer -= Time.deltaTime;
            }

            if (_wallJumpLock > 0f)
            {
                _wallJumpLock -= Time.deltaTime;
            }
        }

        private void CheckEnvironment()
        {
            _isGrounded = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, groundLayer);
            _isOnWall = Physics2D.OverlapBox(frontCheck.position, wallCheckSize, 0f, groundLayer);

            _groundNormal = Vector2.up;
            if (_isGrounded)
            {
                var origin = groundCheck.position;
                var hit = Physics2D.Raycast(origin, Vector2.down, slopeCheckDistance, groundLayer);
                if (hit.collider != null)
                {
                    _groundNormal = hit.normal.normalized;
                }
            }
        }

        private void HandleMovement()
        {
            if (_dashing)
            {
                return;
            }

            float targetSpeed = _inputDirection.x * moveSpeed;
            float accelerationRate = Mathf.Abs(targetSpeed) > 0.01f ? acceleration : deceleration;
            if (!_isGrounded)
            {
                accelerationRate *= airControlMultiplier;
            }

            float maxDelta = accelerationRate * Time.fixedDeltaTime;
            Vector2 velocity = _rb.velocity;

            if (_isGrounded)
            {
                Vector2 tangent = new Vector2(_groundNormal.y, -_groundNormal.x).normalized;
                float currentSpeed = Vector2.Dot(velocity, tangent);
                float speedDiff = targetSpeed - currentSpeed;
                float delta = Mathf.Clamp(speedDiff, -maxDelta, maxDelta);
                velocity += tangent * delta;
            }
            else
            {
                float speedDiff = targetSpeed - velocity.x;
                float delta = Mathf.Clamp(speedDiff, -maxDelta, maxDelta);
                velocity.x += delta;
            }

            velocity.x = Mathf.Clamp(velocity.x, -moveSpeed, moveSpeed);
            _rb.velocity = velocity;
        }

        private void HandleFacing()
        {
            if (_inputDirection.x > 0.1f && !_isFacingRight)
            {
                Flip();
            }
            else if (_inputDirection.x < -0.1f && _isFacingRight)
            {
                Flip();
            }
        }

        private void HandleWallSlide()
        {
            _isWallSliding = false;

            if (_isOnWall && !_isGrounded && _inputDirection.x != 0f && Mathf.Sign(_inputDirection.x) == (_isFacingRight ? 1f : -1f))
            {
                _isWallSliding = true;
                float verticalVelocity = Mathf.Clamp(_rb.velocity.y, -wallSlideSpeed, float.MaxValue);
                _rb.velocity = new Vector2(_rb.velocity.x, verticalVelocity);
            }
        }

        private void ApplyJump()
        {
            _rb.velocity = new Vector2(_rb.velocity.x, 0f);
            _rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        private void OnJumpPerformed(InputAction.CallbackContext context)
        {
            _jumpBufferCounter = jumpBufferTime;
            TryConsumeJump();
        }

        private void OnJumpCanceled(InputAction.CallbackContext context)
        {
            if (_rb.velocity.y > 0f)
            {
                _rb.velocity = new Vector2(_rb.velocity.x, _rb.velocity.y * 0.5f);
            }
        }

        private void TryConsumeJump()
        {
            if (_wallJumpLock > 0f)
            {
                return;
            }

            if (_isGrounded || _coyoteCounter > 0f)
            {
                ApplyJump();
                _coyoteCounter = 0f;
                _jumpBufferCounter = 0f;
                return;
            }

            if (_isWallSliding)
            {
                WallJump();
                return;
            }

            if (_airJumpCounter > 0)
            {
                ApplyJump();
                _airJumpCounter--;
                _jumpBufferCounter = 0f;
            }
        }

        private void WallJump()
        {
            int direction = _isFacingRight ? -1 : 1;
            Vector2 force = new Vector2(wallJumpImpulse.x * direction, wallJumpImpulse.y);
            _rb.velocity = Vector2.zero;
            _rb.AddForce(force, ForceMode2D.Impulse);
            _wallJumpLock = wallJumpControlDelay;
            FlipImmediate(direction > 0);
        }

        private void OnDashPerformed(InputAction.CallbackContext context)
        {
            if (_dashing || _dashCooldownTimer > 0f)
            {
                return;
            }

            _dashRoutine = StartCoroutine(DashCoroutine());
        }

        private IEnumerator DashCoroutine()
        {
            _dashing = true;
            _dashCooldownTimer = dashCooldown + dashDuration;
            float time = 0f;
            Vector2 direction = new Vector2(_isFacingRight ? 1f : -1f, 0f);
            Vector2 initialVelocity = _rb.velocity;

            while (time < dashDuration)
            {
                float eval = dashSpeedCurve.Evaluate(Mathf.Clamp01(time / dashDuration));
                _rb.velocity = direction * (dashSpeed * eval);
                time += Time.deltaTime;
                yield return null;
            }

            _rb.velocity = new Vector2(direction.x * moveSpeed * 0.5f, initialVelocity.y);
            _dashing = false;
        }

        private void OnAttackPerformed(InputAction.CallbackContext context)
        {
            onAttack?.Invoke();
        }

        private void Flip()
        {
            _isFacingRight = !_isFacingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1f;
            transform.localScale = scale;
        }

        private void FlipImmediate(bool faceRight)
        {
            _isFacingRight = faceRight;
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (_isFacingRight ? 1f : -1f);
            transform.localScale = scale;
        }

        private void EnableAction(InputAction action)
        {
            action?.Enable();
        }

        private void DisableAction(InputAction action)
        {
            action?.Disable();
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (groundCheck != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
            }

            if (frontCheck != null)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireCube(frontCheck.position, wallCheckSize);
            }
        }
#endif
    }
}
