using System;
using Unity.VisualScripting;
using UnityEngine;

namespace HeroController
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class PlayerController : MonoBehaviour, IPlayerController
    {
        // Importscriptable stats
        [SerializeField] private float dashSpeed = 20f;
        [SerializeField] private float dashDuration = 0.2f;
        [SerializeField] private float dashCooldown = 1f;
        private bool _canDash = true;
        private bool _isDashing;
        private float _dashTime;
        private float _lastDashTime;

        [SerializeField] private LayerMask wallLayer; // LayerMask for walls
        [SerializeField] private float wallCheckDistance = 0.5f; // Distance to check for walls
        [SerializeField] private float wallSlideSpeed = 2f; // Speed when sliding down a wall
        [SerializeField] private float wallJumpForce = 10f; // Force applied when wall jumping
        [SerializeField] private Vector2 wallJumpDirection = new Vector2(1f, 1.5f); // Direction of the wall jump

        private bool _isTouchingWall;
        private bool _isWallSliding;
        private int _wallDirectionX; // 1 for right wall, -1 for left wall
        [SerializeField] private float jumpBufferTime = 0.1f; // Time window to buffer jumps
        private float _timeSinceJumpPressed = Mathf.Infinity; // Time since the jump button was pressed






        [SerializeField] private ScriptableStats _stats;
        private Rigidbody2D _rb;
        private CapsuleCollider2D _col;
        private FrameInput _frameInput;
        private Vector2 _frameVelocity;
        private bool _cachedQueryStartInColliders;

        #region Interface

        public Vector2 FrameInput => _frameInput.Move;
        public event Action<bool, float> GroundedChanged;
        public event Action Jumped;

        #endregion

        private float _time;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _col = GetComponent<CapsuleCollider2D>();

            _cachedQueryStartInColliders = Physics2D.queriesStartInColliders;
        }

        private void Update()
        {

            _time += Time.deltaTime;
            GatherInput();

            if (!_canDash && Time.time >= _lastDashTime + dashCooldown)
            {
                _canDash = true;
            }
        }

        private void GatherInput()
        {
            _frameInput = new FrameInput
            {
                JumpDown = Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.Space),
                JumpHeld = Input.GetButton("Jump") || Input.GetKey(KeyCode.Space),
                Dash = Input.GetKeyDown(KeyCode.LeftShift),  // Capture the dash input here
                Move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"))
            };

            if (_frameInput.JumpDown)
            {
                _timeSinceJumpPressed = 0f;  // Reset the jump buffer
            }

            _timeSinceJumpPressed += Time.deltaTime;  // Track time since jump was pressed
        }



        private void FixedUpdate()
        {
            CheckCollisions();  // Check for ground, ceiling, and walls

            if (_frameInput.Dash && _canDash)
            {
                HandleDash();  // Handle dash first, if dash is allowed
            }
            else
            {
                HandleJump();   // Then handle jumps (ground and wall)
                HandleDirection();  // Handle horizontal movement
                HandleGravity();  // Handle gravity
            }

            ApplyMovement();  // Apply the final velocity to the Rigidbody
        }



        #region Collisions

        private float _frameLeftGrounded = float.MinValue;
        private bool _grounded;

        private void CheckCollisions()
        {
            Physics2D.queriesStartInColliders = false;

            // Ground and Ceiling Detection
            bool groundHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.down, _stats.GrounderDistance, ~_stats.PlayerLayer);
            bool ceilingHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.up, _stats.GrounderDistance, ~_stats.PlayerLayer);

            // Raycast to check for walls on both sides
            bool wallHitRight = Physics2D.Raycast(transform.position, Vector2.right, wallCheckDistance, wallLayer);
            bool wallHitLeft = Physics2D.Raycast(transform.position, Vector2.left, wallCheckDistance, wallLayer);

            // Determine the wall direction: -1 for left, 1 for right, 0 for no wall
            if (wallHitRight)
            {
                _isTouchingWall = true;
                _wallDirectionX = 1;
            }
            else if (wallHitLeft)
            {
                _isTouchingWall = true;
                _wallDirectionX = -1;
            }
            else
            {
                _isTouchingWall = false;
                _wallDirectionX = 0;
            }

            // Wall sliding condition
            _isWallSliding = _isTouchingWall && !_grounded && _frameVelocity.y < 0;
            if (_isWallSliding)
            {
                _frameVelocity.y = -wallSlideSpeed;  // Apply wall sliding speed
            }

            // Ground Detection Logic
            if (!_grounded && groundHit)
            {
                _grounded = true;
                _coyoteUsable = true;
                _bufferedJumpUsable = true;
                _endedJumpEarly = false;
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




        #endregion


        #region Jumping

        private bool _jumpToConsume;
        private bool _bufferedJumpUsable;
        private bool _endedJumpEarly;
        private bool _coyoteUsable;
        private float _timeJumpWasPressed;

        private bool HasBufferedJump => _bufferedJumpUsable && _time < _timeJumpWasPressed + _stats.JumpBuffer;
        private bool CanUseCoyote => _coyoteUsable && !_grounded && _time < _frameLeftGrounded + _stats.CoyoteTime;



        private void HandleJump()
        {
            // Early jump end for variable jump heights
            if (!_endedJumpEarly && !_grounded && !_frameInput.JumpHeld && _rb.velocity.y > 0)
            {
                _endedJumpEarly = true;
            }

            // Check if the jump button has been pressed recently within the jump buffer time
            if (_timeSinceJumpPressed <= jumpBufferTime)
            {
                if (_grounded || CanUseCoyote)
                {
                    ExecuteJump();  // Grounded or coyote jump
                }
                else if (_isTouchingWall && !_grounded)
                {
                    ExecuteWallJump();  // Wall jump
                }

                _timeSinceJumpPressed = Mathf.Infinity;  // Reset the jump buffer once jump is consumed
            }
        }



        private void ExecuteJump()
        {
            _endedJumpEarly = false;
            _bufferedJumpUsable = false;
            _coyoteUsable = false;
            _frameVelocity.y = _stats.JumpPower;  // Standard jump force
            Jumped?.Invoke();
        }

        private void ExecuteWallJump()
        {
            _endedJumpEarly = false;
            _isWallSliding = false;  // Stop wall sliding during the wall jump

            // Apply wall jump velocity in the opposite direction of the wall
            float jumpDirectionX = -_wallDirectionX;  // Invert the wall direction to jump away
            _frameVelocity = new Vector2(jumpDirectionX * wallJumpDirection.x * wallJumpForce, wallJumpDirection.y * wallJumpForce);

            // Reset the jump buffer and allow dash reset after wall jump if applicable
            _canDash = true;
            Jumped?.Invoke();
        }





        #endregion

        #region Horizontal

        private void HandleDirection()
        {
            if (_frameInput.Move.x == 0)
            {
                var deceleration = _grounded ? _stats.GroundDeceleration : _stats.AirDeceleration;
                _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, 0, deceleration * Time.fixedDeltaTime);
            }
            else
            {
                _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, _frameInput.Move.x * _stats.MaxSpeed, _stats.Acceleration * Time.fixedDeltaTime);
            }
        }

        #endregion

        #region Gravity

        private void HandleGravity()
        {
            if (_grounded && _frameVelocity.y <= 0f)
            {
                _frameVelocity.y = _stats.GroundingForce;  // Ensure player is grounded properly
            }
            else if (_isWallSliding)
            {
                _frameVelocity.y = Mathf.Max(_frameVelocity.y, -wallSlideSpeed);  // Apply wall slide speed
            }
            else
            {
                var inAirGravity = _stats.FallAcceleration;
                if (_endedJumpEarly && _frameVelocity.y > 0)
                {
                    inAirGravity *= _stats.JumpEndEarlyGravityModifier;  // Apply extra gravity when ending a jump early
                }
                _frameVelocity.y = Mathf.MoveTowards(_frameVelocity.y, -_stats.MaxFallSpeed, inAirGravity * Time.fixedDeltaTime);
            }
        }



        #endregion

        #region Dash
        private void HandleDash()
        {
            if (Time.time < _dashTime + dashDuration)
            {
                // Continue dashing in the direction the player was moving
                _frameVelocity.x = _frameInput.Move.x * dashSpeed;
                _frameVelocity.y = 0; // Optionally make dash horizontal only
            }
            else
            {
                // End the dash
                _isDashing = false;
            }
        }
        private void StartDash()
        {
            _isDashing = true;
            _canDash = false;
            _dashTime = Time.time;
            _lastDashTime = Time.time;

            // Optionally, zero out the y-velocity to prevent dash from being influenced by gravity
            _frameVelocity.y = 0;
        }


        private void OnDrawGizmos()
        {
            // Draw raycast lines to visualize wall detection
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + Vector3.right * wallCheckDistance);  // Right wall
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + Vector3.left * wallCheckDistance);   // Left wall
        }


        #endregion


        private void ApplyMovement() => _rb.velocity = _frameVelocity;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_stats == null) Debug.LogWarning("Please assign a ScriptableStats asset to the Player Controller's Stats slot", this);
        }
#endif
    }

    public struct FrameInput
    {
        public bool Dash; // Add Dash input
        public bool JumpDown;
        public bool JumpHeld;
        public Vector2 Move;
    }

    public interface IPlayerController
    {
        public event Action<bool, float> GroundedChanged;

        public event Action Jumped;
        public Vector2 FrameInput { get; }
    }


}