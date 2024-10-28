using System;
using Unity.VisualScripting;
using UnityEngine;

namespace HeroController
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class PlayerController : MonoBehaviour, IPlayerController
    {
        [SerializeField] private Transform characterSprite; // Referenz zum Child-GameObject mit dem Sprite
        [SerializeField] private ParticleSystem jumpParticle;
        [SerializeField] private ParticleSystem DashParticle;
        [SerializeField] private ParticleSystem wallLeft;
        [SerializeField] private ParticleSystem wallRight;
        // Importscriptable stats
        [SerializeField] private float dashSpeed = 15f;        // Dash-Geschwindigkeit
        [SerializeField] private float dashDuration = 0.2f;    // Dauer des Dashs in Sekunden
        [SerializeField] private float dashCooldown = 1f;      // Zeit, bevor ein neuer Dash m�glich ist

        private bool _canDash = true;                          // Kontrolliert, ob der Dash verf�gbar ist
        private bool _isDashing = false;                       // Kontrolliert, ob der Spieler gerade dashen
        private float _dashTimeLeft;                           // Verbleibende Zeit f�r den aktuellen Dash
        private Vector2 _dashDirection;


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
            HandleDash(); // Dash-Logik hinzuf�gen
        }

        private void GatherInput()
        {
            _frameInput = new FrameInput
            {
                Dash = Input.GetKeyDown(KeyCode.LeftShift),
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
            CheckCollisions();
            HandleDash(); // Dash-Logik hinzuf�gen
            HandleJump();
            HandleDirection();
            HandleGravity();

            ApplyMovement();
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
                _canDash = true;  // Reset des Dash nach Bodenber�hrung
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
            if (!_endedJumpEarly && !_grounded && !_frameInput.JumpHeld && _rb.linearVelocity.y > 0)
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
                    jumpParticle.Play();
                }

                _timeSinceJumpPressed = Mathf.Infinity;  // Reset the jump buffer once jump is consumed
            }
        }



        private void ExecuteJump()
        {
            jumpParticle.Play();
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
        }





        #endregion

        #region Horizontal

        private void HandleDirection()
        {
            // Handling der Bewegungsrichtung
            if (_frameInput.Move.x == 0)
            {
                var deceleration = _grounded ? _stats.GroundDeceleration : _stats.AirDeceleration;
                _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, 0, deceleration * Time.fixedDeltaTime);
            }
            else
            {
                _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, _frameInput.Move.x * _stats.MaxSpeed, _stats.Acceleration * Time.fixedDeltaTime);

                // Sprite flippen, basierend auf der Bewegungsrichtung
                if (_frameInput.Move.x > 0)
                {
                    characterSprite.localScale = new Vector3(0.3f, 0.3f, 0.3f); // Rechts
                }
                else if (_frameInput.Move.x < 0)
                {
                    characterSprite.localScale = new Vector3(-0.3f, 0.3f, 0.3f); // Links
                }
            }
        }


        #endregion
        #region Dash
        private void HandleDash()
        {
            if (_frameInput.Dash && _canDash && !_isDashing)
            {
                StartDash();  // Starte den Dash
            }

            if (_isDashing)
            {
                _dashTimeLeft -= Time.fixedDeltaTime;  // Reduziere die verbleibende Dash-Zeit

                if (_dashTimeLeft <= 0)
                {
                    EndDash();  // Beende den Dash, wenn die Zeit abgelaufen ist
                }
            }
        }
        private void StartDash()
        {
            _isDashing = true;
            _canDash = false;
            _dashTimeLeft = dashDuration;

            // Bestimme die Richtung des Dash basierend auf der aktuellen Eingabe
            _dashDirection = _frameInput.Move;
            if (_dashDirection == Vector2.zero)
            {
                _dashDirection = new Vector2(_wallDirectionX, 0);  // Falls keine Eingabe, dashen in Richtung der Wand
            }

            _dashDirection.Normalize();
            _frameVelocity = _dashDirection * dashSpeed;  // Setze die Geschwindigkeit des Dash
        }

        private void EndDash()
        {
            _isDashing = false;
            _canDash = true;
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




        private void ApplyMovement()
        {
            if (!_isDashing)
            {
                _rb.linearVelocity = _frameVelocity;  // Normale Bewegung
            }
            else
            {
                _rb.linearVelocity = _dashDirection * dashSpeed;  // Dash-Bewegung
            }
        }


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