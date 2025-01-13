using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace HeroController
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class RevampedPlayerController : MonoBehaviour, IPlayerController
    {
        [Header("VFX")]
        [SerializeField] private ParticleSystem landingVFX;
        [SerializeField] private ParticleSystem jumpingVFX;
        [SerializeField] private ParticleSystem dashToLeftVFX;
        [SerializeField] private ParticleSystem dashToRightVFX;
        [SerializeField] private ParticleSystem walkParticlesVFX;
        [SerializeField] private ParticleSystem wallLeft;
        [SerializeField] private ParticleSystem wallRight;
        public bool hasWallJump = false;  // Add this
        public bool pickedUpDash = false;  // Add this

        [Header("Player Stats")]
        [SerializeField] private ScriptableStats _stats;
        [SerializeField] private float wallJumpMoveLockDuration = 1f;
        [SerializeField] private float wallCheckDistance = 0.5f;
        [SerializeField] private float wallSlideSpeed = 2f;
        [SerializeField] private float wallJumpForce = 10f;
        [SerializeField] private Vector2 wallJumpDirection = new Vector2(1f, 1.5f);
        [SerializeField] private float dashSpeed = 15f;
        [SerializeField] private float dashDuration = 0.2f;
        [SerializeField] private float dashCooldown = 1f;
        [SerializeField] private float jumpBufferTime = 0.1f;


        private Rigidbody2D _rb;
        private CapsuleCollider2D _col;
        private FrameInput _frameInput;
        private Vector2 _frameVelocity;
        private bool _cachedQueryStartInColliders;

        private bool _canDash = true;
        private bool _isDashing = false;
        private bool _isWallJumpLocked;
        private bool _isTouchingWall;
        private bool _isWallSliding;
        private bool _grounded;
        private bool _bufferedJumpUsable;
        private bool _endedJumpEarly;
        private bool _coyoteUsable;

        private float _dashTimeLeft;
        private float _timeSinceWallJump;
        private float _timeSinceJumpPressed = Mathf.Infinity;
        private float _frameLeftGrounded = float.MinValue;
        private int _wallDirectionX;
        private int _lastWallJumpDirection = 0;

        private Vector2 _dashDirection;
        private Coroutine _dashCooldownCoroutine;
        private float _time;
        private bool CanUseCoyote = true;

        #region Interface

        public Vector2 FrameInput => _frameInput.Move;
        public event Action<bool, float> GroundedChanged;
        public event Action Jumped;

        public float DashDuration = 0.2f;
        public float DashSpeed = 10f;
        public float DashCooldown = 1f;
        public float WallSlideSpeed = 2f;
        public LayerMask WallLayer;  // Assuming you use a LayerMask for walls
        public bool HasWallJump = true;
        public bool PickedUpDash = false;


        #endregion

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _col = GetComponent<CapsuleCollider2D>();
            UpgradeHandler tmp = UpgradeHandler.GetInstance();
            tmp?.UpdateStatus();
            _cachedQueryStartInColliders = Physics2D.queriesStartInColliders;
        }

        private void Update()
        {
            _time += Time.deltaTime;
            GatherInput();
            HandleDash();
        }

        private void GatherInput()
        {
            _frameInput = new FrameInput
            {
                Dash = Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.JoystickButton1),
                JumpDown = Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.Space),
                JumpHeld = Input.GetButton("Jump") || Input.GetKey(KeyCode.Space),
                Move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"))
            };

            if (_frameInput.JumpDown)
            {
                _timeSinceJumpPressed = 0f;
            }

            _timeSinceJumpPressed += Time.deltaTime;
        }

        private void FixedUpdate()
        {
            if (_isWallJumpLocked)
            {
                _timeSinceWallJump += Time.fixedDeltaTime;
                if (_timeSinceWallJump >= wallJumpMoveLockDuration)
                {
                    _isWallJumpLocked = false;
                }
            }

            CheckCollisions();
            HandleDash();
            HandleJump();
            if (!_isWallJumpLocked) HandleDirection();
            HandleGravity();
            ApplyMovement();
        }

        private void CheckCollisions()
        {
            Physics2D.queriesStartInColliders = false;

            // Ground and Ceiling Detection
            bool groundHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.down, _stats.GrounderDistance, ~_stats.PlayerLayer);
            bool ceilingHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.up, _stats.GrounderDistance, ~_stats.PlayerLayer);

            // Raycast to check for walls on both sides
            bool wallHitRight = Physics2D.Raycast(transform.position, Vector2.right, wallCheckDistance, WallLayer);
            bool wallHitLeft = Physics2D.Raycast(transform.position, Vector2.left, wallCheckDistance, WallLayer);

            // Wall Detection and Wall Sliding Logic
            _isTouchingWall = wallHitRight || wallHitLeft;
            _wallDirectionX = wallHitRight ? 1 : wallHitLeft ? -1 : 0;
            _isWallSliding = _isTouchingWall && !_grounded && _frameVelocity.y < 0;
            if (_isWallSliding) _frameVelocity.y = -WallSlideSpeed;

            // Ground Detection Logic
            if (!_grounded && groundHit)
            {
                _grounded = true;
                _canDash = true;
                _bufferedJumpUsable = true;
                _endedJumpEarly = false;
                landingVFX.Play();
                _lastWallJumpDirection = 0;
                GroundedChanged?.Invoke(true, Mathf.Abs(_frameVelocity.y));
            }
            else if (_grounded && !groundHit)
            {
                _grounded = false;
                _frameLeftGrounded = _time;
                GroundedChanged?.Invoke(false, 0);
            }

            Physics2D.queriesStartInColliders = _cachedQueryStartInColliders;
        }

        private void HandleJump()
        {
            if (!_endedJumpEarly && !_grounded && !_frameInput.JumpHeld && _rb.linearVelocity.y > 0)
            {
                _endedJumpEarly = true;
            }

            if (_timeSinceJumpPressed <= jumpBufferTime)
            {
                if (_grounded || CanUseCoyote)
                {
                    ExecuteJump();
                }
                else if (_isTouchingWall && !_grounded && HasWallJump)
                {
                    ExecuteWallJump();
                }
                _timeSinceJumpPressed = Mathf.Infinity;
            }
        }

        private void ExecuteJump()
        {
            _endedJumpEarly = false;
            _bufferedJumpUsable = false;
            _coyoteUsable = false;
            _frameVelocity.y = _stats.JumpPower;
            jumpingVFX.Play();
            Jumped?.Invoke();
        }

        private void ExecuteWallJump()
        {
            if (_wallDirectionX == _lastWallJumpDirection) return;

            _endedJumpEarly = false;
            _isWallSliding = false;
            _lastWallJumpDirection = _wallDirectionX;

            _isWallJumpLocked = true;
            _timeSinceWallJump = 0f;

            float jumpDirectionX = -_wallDirectionX;
            _frameVelocity = new Vector2(jumpDirectionX * wallJumpDirection.x * wallJumpForce, wallJumpDirection.y * wallJumpForce);
        }

        private void HandleDirection()
        {
            if (_isWallJumpLocked && Mathf.Sign(_frameInput.Move.x) == _lastWallJumpDirection) return;

            if (_frameInput.Move.x == 0)
            {
                var deceleration = _grounded ? _stats.GroundDeceleration : _stats.AirDeceleration;
                _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, 0, deceleration * Time.fixedDeltaTime);
            }
            else
            {
                _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, _frameInput.Move.x * _stats.MaxSpeed, _stats.Acceleration * Time.fixedDeltaTime);
                Vector3 scale = transform.localScale;
                scale.x = Mathf.Sign(_frameInput.Move.x) * Mathf.Abs(scale.x);
                transform.localScale = scale;
            }
        }

        private void HandleDash()
        {
            if (_frameInput.Dash && _canDash && !_isDashing && PickedUpDash)
            {
                StartDash();
                PlayDashVFX();
            }

            if (_isDashing)
            {
                _dashTimeLeft -= Time.fixedDeltaTime;
                if (_dashTimeLeft <= 0)
                {
                    EndDash();
                }
            }
        }

        private void StartDash()
        {
            _isDashing = true;
            _canDash = false;
            _dashTimeLeft = DashDuration;
            _dashDirection = new Vector2(_frameInput.Move.x != 0 ? Mathf.Sign(_frameInput.Move.x) : Mathf.Sign(transform.localScale.x), 0);
            _dashDirection.Normalize();
            _frameVelocity = _dashDirection * DashSpeed;
        }

        private void EndDash()
        {
            _isDashing = false;
            _dashCooldownCoroutine = StartCoroutine(DashCooldownCoroutine());
        }

        private IEnumerator DashCooldownCoroutine()
        {
            yield return new WaitForSeconds(DashCooldown);
            _canDash = true;
        }

        private void PlayDashVFX() => dashToRightVFX.Play();

        private void HandleGravity()
        {
            if (_grounded && _frameVelocity.y <= 0f)
                _frameVelocity.y = _stats.GroundingForce;
            else if (_isWallSliding)
                _frameVelocity.y = Mathf.Max(_frameVelocity.y, -WallSlideSpeed);
            else
            {
                float inAirGravity = _stats.FallAcceleration;
                if (_endedJumpEarly && _frameVelocity.y > 0)
                    inAirGravity *= _stats.JumpEndEarlyGravityModifier;
                _frameVelocity.y = Mathf.MoveTowards(_frameVelocity.y, -_stats.MaxFallSpeed, inAirGravity * Time.fixedDeltaTime);
            }
        }

        private void ApplyMovement()
        {
            _rb.linearVelocity = _isDashing ? _dashDirection * DashSpeed : _frameVelocity;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_stats == null)
                Debug.LogWarning("Please assign a ScriptableStats asset to the Player Controller's Stats slot", this);
        }
#endif
    }

    public struct FrameInput
    {
        public bool Dash;
        public bool JumpDown;
        public bool JumpHeld;
        public Vector2 Move;
    }

    public interface IPlayerController
    {
        event Action<bool, float> GroundedChanged;
        event Action Jumped;
        Vector2 FrameInput { get; }
    }
}
