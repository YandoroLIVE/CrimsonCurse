using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace HeroController
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class PlayerController : MonoBehaviour, IPlayerController
    {
        [SerializeField] private float afktimeTillIdle = 2f;
        [SerializeField] private bool forrestLevel;
        [SerializeField] private bool spawnCamera = true;

        [SerializeField] private ParticleSystem landingVFX;
        [SerializeField] private ParticleSystem jumpingVFX;
        [SerializeField] private ParticleSystem dashToLeftVFX;
        [SerializeField] private ParticleSystem dashToRightVFX;
        [SerializeField] private ParticleSystem m_dustParticle;
        [SerializeField] private ParticleSystem m_LeafParticle;
        [SerializeField] private ParticleSystem m_IdleTailEntry;
        [SerializeField] private ParticleSystem m_IdleTailLoop;
        [SerializeField] private ParticleSystem m_IdleTailExit;
        [SerializeField] private GameObject cameraPrefab;

        [SerializeField] private float wallJumpMoveLockDuration = 1f;
        // Importscriptable stats
        [SerializeField] private float dashSpeed = 15f;        // Dash-Geschwindigkeit
        [SerializeField] private float dashDuration = 0.2f;    // Dauer des Dashs in Sekunden
        [SerializeField] private float dashCooldown = 1f;      // Zeit, bevor ein neuer Dash m�glich ist
        public bool pickedUpDash = false;
        public bool hasWallJump = false;
        private bool _canDash = true;                          // Kontrolliert, ob der Dash verf�gbar ist
        private bool _isDashing = false;                       // Kontrolliert, ob der Spieler gerade dashen
        private float _dashTimeLeft;                           // Verbleibende Zeit f�r den aktuellen Dash
        private Vector2 _dashDirection;
        private Coroutine _dashCooldownCoroutine;
        private float _timeSinceWallJump;
        private bool _isWallJumpLocked;


        [SerializeField] private LayerMask wallLayer; // LayerMask for walls
        [SerializeField] private float wallCheckDistance = 0.5f; // Distance to check for walls
        [SerializeField] private float wallSlideSpeed = 2f; // Speed when sliding down a wall
        [SerializeField] private float wallJumpForce = 10f; // Force applied when wall jumping
        [SerializeField] private Vector2 wallJumpDirection = new Vector2(1f, 1.5f); // Direction of the wall jump

        private bool _isTouchingWall;
        private bool _isWallSliding;
        private int _wallDirectionX; // 1 for right wall, -1 for left wall
        private int _lastWallJumpDirection = 0; // 1 für rechts, -1 für links, 0 für keinen Walljump

        [SerializeField] private float jumpBufferTime = 0.1f; // Time window to buffer jumps
        private float _timeSinceJumpPressed = Mathf.Infinity; // Time since the jump button was pressed



        //afk vars
        private float afkTime = 0f;
        private bool afk = false;
        private bool afkEntry = false;
        private bool afkExit = false;



        [SerializeField] private ScriptableStats _stats;
        private Rigidbody2D _rb;
        private CapsuleCollider2D _col;
        private FrameInput _frameInput;
        private Vector2 _frameVelocity;
        private bool _cachedQueryStartInColliders;
        private ParticleSystem runParticle;


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
            UpgradeHandler tmp = UpgradeHandler.GetInstance();   
            if(tmp != null) 
            {
                tmp.UpdateStatus();
            }
            if (spawnCamera) Instantiate(cameraPrefab);
            if (forrestLevel)
            {
                m_LeafParticle.gameObject.SetActive(true);
                m_LeafParticle.Play();
                runParticle = m_LeafParticle;
            }
            else
            {
                m_dustParticle.gameObject.SetActive(true);
                m_dustParticle.Play();
                runParticle = m_dustParticle;
            }
            _cachedQueryStartInColliders = Physics2D.queriesStartInColliders;
        }



        private void Update()
        {
            Debug.Log(_dashTimeLeft);

            _time += Time.deltaTime;
            GatherInput();
            HandleDash();
            AfkHandling();
        }

        private void GatherInput()
        {
            _frameInput = new FrameInput
            {
                Dash = Input.GetKeyDown(KeyCode.LeftShift ) || Input.GetKeyDown(KeyCode.JoystickButton1),
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
                    _isWallJumpLocked = false; // Hebe die Bewegungssperre auf
                }
            }

            CheckCollisions();
            HandleDash();
            HandleJump();

            if (!_isWallJumpLocked) HandleDirection(); // Bewegung nur erlauben, wenn die Sperre aufgehoben ist
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
                _frameVelocity.y = -_stats.wallSlideSpeed;  // Apply wall sliding speed
            }

            // Ground Detection Logic
            if (!_grounded && groundHit)
            {
                runParticle.Play();
                _grounded = true;
                _canDash = true;  // Reset des Dash nach Bodenberührung
                _coyoteUsable = true;
                _bufferedJumpUsable = true;
                _endedJumpEarly = false;
                landingVFX.Play();


                // Reset der zuletzt gesprungenen Wand
                _lastWallJumpDirection = 0;

                GroundedChanged?.Invoke(true, Mathf.Abs(_frameVelocity.y));
            }

            else if (_grounded && !groundHit)
            {
                runParticle.Stop();
                _grounded = false;
                _frameLeftGrounded = _time;
                GroundedChanged?.Invoke(false, 0);
            }

            Physics2D.queriesStartInColliders = _cachedQueryStartInColliders;
        }




        #endregion

        #region AFK
        IEnumerator SetAfk(float t, bool state)
        {
            yield return new WaitForSeconds(t);
            afk = state;
        }

        IEnumerator StartParticleSystemWithOffset(float t, ParticleSystem systemToPlay)
        {
            yield return new WaitForSeconds(t);
            systemToPlay.Play();
        }


        private void AfkHandling()
        {
            if (!Input.anyKey)
            {
                afkTime += Time.deltaTime;
                if (afkTime >= afktimeTillIdle)
                {
                    if (!afk)
                    {
                        if (!afkEntry)
                        {
                            afkEntry = true;
                            afkExit = false;
                            m_IdleTailEntry.Play();
                        }
                        StartCoroutine(SetAfk(m_IdleTailEntry.main.startLifetime.constantMax, true));
                    }

                    else
                    {
                        if (!m_IdleTailLoop.isPlaying)
                        {
                            m_IdleTailLoop.Play();
                        }
                    }
                }
            }
            else
            {
                if (afk)
                {
                    float t = m_IdleTailLoop.main.startLifetime.constantMax - m_IdleTailLoop.time;
                    StartCoroutine(SetAfk(t, false));
                    if (!afkExit)
                    {
                        m_IdleTailLoop.Stop();
                        afkEntry = false;
                        afkExit = true;
                        StartCoroutine(StartParticleSystemWithOffset(t, m_IdleTailExit));
                    }
                    afkTime = 0;
                }

                else
                {
                    afkTime = 0;
                }
            }
        }





        #endregion

        #region Jumping

        private bool _jumpToConsume;
        private bool _bufferedJumpUsable;
        private bool _endedJumpEarly;
        private bool _coyoteUsable;
        private float _timeJumpWasPressed;

        private bool HasBufferedJump => _bufferedJumpUsable && _time < _timeJumpWasPressed + _stats.jumpBufferTime;
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
                else if (_isTouchingWall && !_grounded && hasWallJump)
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
            jumpingVFX.Play();
            Jumped?.Invoke();
        }

        private void ExecuteWallJump()
        {
            if (_wallDirectionX == _lastWallJumpDirection) return; // Blockiere doppelten Walljump

            _endedJumpEarly = false;
            _isWallSliding = false;
            _lastWallJumpDirection = _wallDirectionX;

            // Setze die Bewegungssperre
            _isWallJumpLocked = true;
            _timeSinceWallJump = 0f;

            // Walljump-Bewegung
            float jumpDirectionX = -_wallDirectionX;
            _frameVelocity = new Vector2(jumpDirectionX * wallJumpDirection.x * wallJumpForce, wallJumpDirection.y * wallJumpForce);
        }







        #endregion

        #region Horizontal

        private void HandleDirection()
        {
            if (_isWallJumpLocked && Mathf.Sign(_frameInput.Move.x) == _lastWallJumpDirection)
            {
                return; // Blockiere Bewegung in Richtung der Wand, von der gesprungen wurde
            }

            if (_frameInput.Move.x == 0)
            {
                var deceleration = _grounded ? _stats.GroundDeceleration : _stats.AirDeceleration;
                _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, 0, deceleration * Time.fixedDeltaTime);
            }
            else
            {
                _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, _frameInput.Move.x * _stats.MaxSpeed, _stats.Acceleration * Time.fixedDeltaTime);

                // Drehe den Charakter basierend auf der Richtung
                Vector3 scale = transform.localScale;
                scale.x = Mathf.Sign(_frameInput.Move.x) * Mathf.Abs(scale.x); // Behalte die ursprüngliche Größe bei
                transform.localScale = scale;
            }
        }




        #endregion
        #region Dash

        public float GetCooldownPercentage()
        {
            if (_canDash) return 1f; // Cooldown ist vollständig abgelaufen
            return Mathf.Clamp01(1f - (_dashTimeLeft / _stats.dashCooldown));
        }
        private void HandleDash()
        {
            if (_frameInput.Dash && _canDash && !_isDashing && pickedUpDash)
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
            _dashTimeLeft = _stats.dashDuration;

            _dashDirection = _frameInput.Move.x != 0
                ? new Vector2(Mathf.Sign(_frameInput.Move.x), 0)
                : new Vector2(transform.localScale.x > 0 ? 1 : -1, 0);

            _dashDirection.Normalize();
            _frameVelocity = _dashDirection * _stats.dashSpeed;
        }

        private IEnumerator DashCooldownCoroutine()
        {
            yield return new WaitForSeconds(_stats.dashCooldown);
            _canDash = true;
        }


        private void EndDash()
        {
            _isDashing = false;
            if (_dashCooldownCoroutine != null)
            {
                StopCoroutine(_dashCooldownCoroutine);
            }
            _dashCooldownCoroutine = StartCoroutine(DashCooldownCoroutine());
        }

        private void PlayDashVFX()
        {
            dashToRightVFX.Play();
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
                _frameVelocity.y = Mathf.Max(_frameVelocity.y, -_stats.wallSlideSpeed);  // Apply wall slide speed
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
                _rb.linearVelocity = _dashDirection * _stats.dashSpeed;  // Dash-Bewegung
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