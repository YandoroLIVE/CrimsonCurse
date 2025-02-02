using HeroController;
using Sirenix.OdinInspector.Editor.Internal;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerControllerNew : MonoBehaviour
{
    static PlayerControllerNew inst;

    // Serialized Fields
    [SerializeField] private float afktimeTillIdle = 2f;
    [SerializeField] private bool forrestLevel;
    [SerializeField] private bool spawnCamera = true;

    [Header("Run Stats")]
    [SerializeField, Range(1f, 100f)] private float maxRunSpeed = 20f;
    [SerializeField, Range(0.25f, 50f)] private float groundAcceleration = 5f;
    [SerializeField, Range(0.25f, 50f)] private float groundDeceleration = 20f;
    [SerializeField, Range(0.25f, 50f)] private float airAcceleration = 5f;
    [SerializeField, Range(0.25f, 50f)] private float airDeceleration = 5f;

    [Header("Groundcheck Stats")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private float groundDetectionRayLength = 0.02f;
    [SerializeField] private float wallDetectionRayLength = 0.02f;
    [SerializeField] private float headDetectionRayLength = 0.02f;
    [SerializeField, Range(0, 1)] private float headWidth = 0.75f;

    [Header("Dashing")]
    [SerializeField] private float dashSpeed = 30f;
    [SerializeField] private float startDashTime = 0.1f;
    [SerializeField] private float dashCooldown = 0.2f;

    [Header("Jump")]
    [SerializeField] float jumpHeight = 4f;
    [SerializeField, Range(1, 1.1f)] private float jumpHeightCompensationFactor = 1.054f;
    [SerializeField] float timeTillJumpApex = 0.35f;
    [SerializeField, Range(0.01f, 5f)] private float gravityOnReleaseMultiplier = 2f;
    [SerializeField] float maxFallSpeed = 25f;
    [SerializeField, Range(0.02f, 0.3f)] private float timeForUpwardsCancel = 0.027f;
    [SerializeField, Range(0.05f, 1f)] private float apexThreshold = 0.97f;
    [SerializeField, Range(0.01f, 1f)] private float apexHangTime = 0.075f;
    [SerializeField, Range(0f, 1f)] private float jumpBufferTime = 0.125f;
    [SerializeField, Range(0f, 1f)] private float jumpCoyoteTime = 0.1f;

    [Header("Wall")]
    [SerializeField] float hangTime;
    [SerializeField] float stuckTime;
    [SerializeField] float wallGrabCooldown = 0.1f;
    [SerializeField, Range(0, 1)] float hangSpeedFactor;

    // Public Fields
    [HideInInspector] public bool grounded;
    [HideInInspector] public bool onWall;
    [HideInInspector] public float moveInput;
    [HideInInspector] public bool canMove = true;
    [HideInInspector] public bool isDashing = false;
    public bool pickedUpDash = false;
    public bool hasWallJump = false;
    private bool jumped = false;


    [Header("Refrences")]
    [SerializeField] private Collider2D _feetColl;
    [SerializeField] private Collider2D _bodyColl;
    [SerializeField] private ParticleSystem jumpEffect;
    [SerializeField] private ParticleSystem impactEffect;
    [SerializeField] private ParticleSystem dashEffect;
    [SerializeField] private ParticleSystem m_dustParticle;
    [SerializeField] private ParticleSystem m_LeafParticle;
    [SerializeField] private ParticleSystem m_IdleTailEntry;
    [SerializeField] private ParticleSystem m_IdleTailLoop;
    [SerializeField] private ParticleSystem m_IdleTailExit;
    [SerializeField] private GameObject cameraPrefab;

    //misc vars
    private bool debug = false;
    private Vector2 _moveVelocity;
    private Rigidbody2D m_rb;
    private bool m_facingRight = true;
    private ParticleSystem runParticle;
    static private bool blockedInput;

    //dash vars
    private bool m_hasDashedInAir = false;
    private float m_dashCooldown;
    private float m_dashTime;

    //jump vars
    private bool _bumpedHead;
    private RaycastHit2D[] _groundHit;
    private RaycastHit2D[] _headHit;
    private RaycastHit2D[] _wallLeftHit;
    private RaycastHit2D[] _wallRightHit;
    private int lastWallID = 0; //0 = none, -1 = left, 1 = right
    private float gravity;
    private float initJumpVelocity;
    private float adjustedJumpHeight;
    private float yVelocity;
    private bool inJump;
    private bool falling;
    private bool fastFalling;
    private float fastFallingTime;
    private float fastFallReleaseSpeed;

    // wall vars
    private float wallTimer = 0;
    private float stuckTimer = 0;
    private bool canWallJump = false;
    private bool canWallGrab = true;
    //jump apex vars
    private float apexPoint;
    private float timePastApexThreshold;
    private bool isPastApexThreshold;

    //jump buffer vars
    private float jumpBufferTimer;
    private bool jumpRelesedDuringBuffer;
    private float coyoteTimer;

    //afk vars
    private float afkTime = 0f;
    private bool afk = false;
    private bool afkEntry = false;
    private bool afkExit = false;


    // Input String Caching
    private static readonly string HorizontalInput = "Horizontal";

    public static float HorizontalRaw() => Input.GetAxisRaw(HorizontalInput);
    public static bool GetJumpInput() => Input.GetKeyDown(KeyCode.Space);
    public static bool GetJumpReleased() => Input.GetKeyUp(KeyCode.Space);
    public static bool Dash() => Input.GetKeyDown(KeyCode.LeftShift);

    public void Awake()
    {
#if UNITY_EDITOR
        debug = true;
#else
        debug = false;
#endif
        if (inst != null)
        {
            Destroy(this.gameObject);
        }

        else
        {
            inst = this;
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
    }
    private void Start()
    {
        m_rb = GetComponent<Rigidbody2D>();
        m_dashCooldown = 0;
        m_dashTime = 0;
    }


    private void FixedUpdate()
    {
        CollisionChecks();
        HandleDash();
        Jump();
        if (grounded)
        {
            HandleMovement(groundAcceleration, groundDeceleration, new Vector2(HorizontalRaw(), 0));
        }
        else
        {
            HandleMovement(airAcceleration, airDeceleration, new Vector2(HorizontalRaw(), 0));
        }

    }

    private void Update()
    {

        Debug.Log(canWallJump);
        CalculateValues();
        CountTimers();
        if (!blockedInput)
        {
            AfkHandling();
            moveInput = HorizontalRaw();
            HandleDashInput();
            JumpChecks();
            OnWallChecks();
        }
    }
    private void CollisionChecks()
    {
        IsGrounded();
        BumpedHead();
        IsOnWall();
    }



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

    public static void DisableInput(float duration)
    {
        blockedInput = true;
        inst.StartCoroutine(inst.GiveInputBack(duration));
    }

    private void IsGrounded()
    {
        Vector2 boxCastOrigin = new Vector2(_feetColl.bounds.center.x, _feetColl.bounds.min.y);
        Vector2 boxCastSize = new Vector2(_feetColl.bounds.size.x, groundDetectionRayLength);
        _groundHit = Physics2D.BoxCastAll(boxCastOrigin, boxCastSize, 0f, Vector2.down, groundDetectionRayLength, whatIsGround);
        bool wasGrounded = grounded;
        grounded = false;
        foreach (var collision in _groundHit)
        {
            var collider = collision.collider;
            if (collider.isTrigger == false)
            {
                grounded = true;
                if (!wasGrounded)
                {
                    impactEffect.Play();
                    runParticle.Play();
                }
                break;
            }
        }
        if (!grounded)
        {
            runParticle.Stop();
        }
        if (debug)
        {
            Color color;
            if (grounded)
            {
                color = Color.green;
            }
            else { color = Color.red; }
            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y), Vector2.down * groundDetectionRayLength, color);
            Debug.DrawRay(new Vector2(boxCastOrigin.x + boxCastSize.x / 2, boxCastOrigin.y), Vector2.down * groundDetectionRayLength, color);
            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y - groundDetectionRayLength), Vector2.right * boxCastSize.x, color);
        }
    }


    private void HandleMovement(float acceleration, float deceleration, Vector2 movementInput)
    {
        if (!isDashing)
        {
            if (movementInput != Vector2.zero)
            {
                Vector2 targetVelocity = new Vector2(movementInput.x, 0f) * maxRunSpeed;

                _moveVelocity = Vector2.Lerp(_moveVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
                m_rb.linearVelocityX = _moveVelocity.x;

            }
            else
            {
                _moveVelocity = Vector2.Lerp(_moveVelocity, Vector2.zero, deceleration * Time.fixedDeltaTime);
                m_rb.linearVelocityX = _moveVelocity.x;
            }
        }
        if (!m_facingRight && moveInput > 0f) Flip();
        else if (m_facingRight && moveInput < 0f) Flip();
    }

    private void HandleDash()
    {
        if (!isDashing) return;

        if (m_dashTime <= 0f)
        {
            isDashing = false;
            falling = true;
            fastFalling = false;
            fastFallingTime = 0f;
            m_dashCooldown = dashCooldown;
            m_dashTime = startDashTime;
        }
        else
        {
            m_dashTime -= Time.deltaTime;
            inJump = false;
            falling = false;
            fastFalling = false;
            fastFallingTime = 0f;
            m_rb.linearVelocity = (HorizontalRaw() > 0 ? Vector2.right : Vector2.left) * dashSpeed;
        }
    }

    private void HandleDashInput()
    {
        m_dashCooldown -= Time.deltaTime;
        if (!pickedUpDash) return;

        if (!isDashing && !m_hasDashedInAir && m_dashCooldown <= 0f && Dash())
        {
            isDashing = true;
            dashEffect.Play();
            if (!grounded) m_hasDashedInAir = true;
        }

        if (m_hasDashedInAir && grounded) m_hasDashedInAir = false;
    }






    IEnumerator GiveInputBack(float duration)
    {
        yield return new WaitForSeconds(duration);
        blockedInput = false;
    }
    private void Flip()
    {
        m_facingRight = !m_facingRight;
        var scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void CalculateValues()
    {
        adjustedJumpHeight = jumpHeight * jumpHeightCompensationFactor;
        gravity = -(2f * adjustedJumpHeight) / Mathf.Pow(timeTillJumpApex, 2f);
        initJumpVelocity = Mathf.Abs(gravity) * timeTillJumpApex;
    }

    private void JumpChecks()
    {
        if (GetJumpInput())
        {
            jumpBufferTimer = jumpBufferTime;
            jumpRelesedDuringBuffer = false;
        }
        if (GetJumpReleased())
        {
            if (jumpBufferTimer > 0f)
            {
                jumpRelesedDuringBuffer = true;
            }

            if (inJump && yVelocity > 0f)
            {
                if (isPastApexThreshold)
                {
                    isPastApexThreshold = false;
                    fastFalling = true;
                    fastFallingTime = timeForUpwardsCancel;
                    yVelocity = 0f;
                }

                else
                {
                    fastFalling = true;
                    fastFallReleaseSpeed = yVelocity;
                }
            }
        }


        if (jumpBufferTimer > 0f && !inJump && (grounded || coyoteTimer > 0f || canWallJump))
        {
            
            
            if (canWallJump) 
            {
                canWallJump = false;
                StartCoroutine(WallGrabCooldown());
                //float wallJumpXOffset = 10;
                //m_rb.MovePosition(new Vector2(m_rb.position.x + (- lastWallID * wallDetectionRayLength* wallJumpXOffset), m_rb.position.y));
            }
            InitJump();
            
            if (jumpRelesedDuringBuffer)
            {
                fastFalling = true;
                fastFallReleaseSpeed = yVelocity;
            }
        }

        if ((inJump || falling) && (grounded || onWall) && yVelocity <= 0f)
        {
            inJump = false;
            falling = false;
            fastFalling = false;
            fastFallingTime = 0f;
            isPastApexThreshold = false;
            lastWallID = 0;
            wallTimer = hangTime;
            yVelocity = Physics2D.gravity.y;
        }

        if(onWall && yVelocity > 0f && !inJump) 
        {
            yVelocity = -0.01f;
        }

    }

    IEnumerator WallGrabCooldown() 
    {
        canWallGrab = false;
        yield return new WaitForSeconds(wallGrabCooldown);
        canWallGrab = true;
    }

    private void InitJump()
    {
        if (!inJump)
        {
            inJump = true;
        }
        jumpBufferTimer = 0f;
        CalculateValues();
        yVelocity = initJumpVelocity;
    }

    private void Jump()
    {
        if (inJump)
        {
            if (_bumpedHead)
            {
                fastFalling = true;
            }
            if (yVelocity >= 0f)
            {
                apexPoint = Mathf.InverseLerp(initJumpVelocity, 0f, yVelocity);
                if (apexPoint > apexThreshold)
                {
                    if (!isPastApexThreshold)
                    {
                        isPastApexThreshold = true;
                        timePastApexThreshold = 0;
                    }

                    if (isPastApexThreshold)
                    {
                        timePastApexThreshold += Time.fixedDeltaTime;
                        if (timePastApexThreshold < apexHangTime)
                        {
                            yVelocity = 0f;
                        }
                        else
                        {
                            yVelocity = -0.01f; // init a fall
                        }
                    }
                }
                else
                {
                    yVelocity += gravity * Time.fixedDeltaTime;
                    if (isPastApexThreshold)
                    {
                        isPastApexThreshold = false;
                    }
                }
            }
            else if (!fastFalling)
            {
                yVelocity += gravity * gravityOnReleaseMultiplier * Time.fixedDeltaTime;
            }

            else if (yVelocity < 0f)
            {
                if (!falling)
                {
                    falling = true;
                }
            }
        }

        if (fastFalling)
        {
            if (fastFallingTime >= timeForUpwardsCancel)
            {
                yVelocity += gravity * gravityOnReleaseMultiplier * Time.fixedDeltaTime;
            }

            else if (fastFallingTime < timeForUpwardsCancel)
            {
                yVelocity = Mathf.Lerp(fastFallReleaseSpeed, 0f, (fastFallingTime / timeForUpwardsCancel));
            }
            fastFallingTime += Time.fixedDeltaTime;
        }

        if (!grounded && !inJump && !onWall)
        {
            if (!falling)
            {
                falling = true;
            }

            yVelocity += gravity * Time.fixedDeltaTime;
        }

        float maxPositiveVelocity = 50f;

        yVelocity = Mathf.Clamp(yVelocity, -maxFallSpeed, maxPositiveVelocity);

        m_rb.linearVelocityY = yVelocity;

    }

    private void CountTimers()
    {
        jumpBufferTimer -= Time.deltaTime;
        if (!grounded && !canWallJump)
        {
            coyoteTimer -= Time.deltaTime;
        }
        else
        {
            coyoteTimer = jumpCoyoteTime;
        }
    }

    private void IsOnWall()
    {
        if (!hasWallJump) return;
        if (grounded)
        {
            onWall = false;
            canWallJump= false;
            return;
        }
        else
        {
            int tmp = CheckWalls();
            if (tmp != 0 && canWallGrab)
            {
                if (!onWall) 
                {
                    yVelocity = -0.01f;
                }
                onWall = true;
                falling = false;
                fastFalling = false;
                inJump = false;
                if (tmp != lastWallID)
                {
                    canWallJump = true;
                    lastWallID = tmp;
                    wallTimer = hangTime;
                    yVelocity = -0.01f;
                }
                yVelocity += Physics2D.gravity.y * hangSpeedFactor *Time.fixedDeltaTime;
                
            }
            else
            {
                onWall = false;
                falling = true;
            }

        }


    }

    private void OnWallChecks()
    {
        if (onWall)
        {
            if(Mathf.Sign(HorizontalRaw()) != lastWallID) 
            {
                stuckTimer -= Time.deltaTime;
            }
            else 
            {
                stuckTimer = stuckTime;
                moveInput = 0;
            }
            wallTimer -= Time.deltaTime;
            if (wallTimer <= 0 || grounded)
            {
                onWall = false;
            }

        }
    }


    private int CheckWalls()
    {
        if (CheckLeftWall()) return -1;
        else if (CheckRightWall()) return 1;
        else return 0;
    }

    private bool CheckLeftWall()
    {
        bool hitwall = false;
        Vector2 boxCastOrigin = new Vector2(_bodyColl.bounds.min.x, _bodyColl.bounds.center.y);
        RaycastHit2D[] hits = Physics2D.RaycastAll(boxCastOrigin, Vector2.left, wallDetectionRayLength, whatIsGround);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null && !hit.collider.isTrigger)
            {
                hitwall = true;
                break;
            }
        }

        if (debug)
        {
            Color rayColor;
            if (hitwall)
            {
                rayColor = Color.cyan;
            }
            else
            {
                rayColor = Color.magenta;
            }

            Debug.DrawRay(boxCastOrigin, Vector2.left, rayColor, wallDetectionRayLength);
        }
        return hitwall;
    }
    private bool CheckRightWall()
    {
        bool hitwall = false;
        Vector2 boxCastOrigin = new Vector2(_bodyColl.bounds.max.x, _bodyColl.bounds.center.y);
        RaycastHit2D[] hits = Physics2D.RaycastAll(boxCastOrigin, Vector2.right, wallDetectionRayLength, whatIsGround);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null && !hit.collider.isTrigger)
            {
                hitwall = true;
                break;
            }
        }
        if (debug)
        {
            Color rayColor;
            if (hitwall)
            {
                rayColor = Color.green;
            }
            else
            {
                rayColor = Color.yellow;
            }

            Debug.DrawRay(boxCastOrigin, Vector2.right, rayColor, wallDetectionRayLength);
        }
        return hitwall;
    }

    private void BumpedHead()
    {
        Vector2 boxCastOrigin = new Vector2(_feetColl.bounds.center.x, _bodyColl.bounds.max.y);
        Vector2 boxCastSize = new Vector2(_feetColl.bounds.size.x * headWidth, headDetectionRayLength);
        _headHit = Physics2D.BoxCastAll(boxCastOrigin, boxCastSize, 0f, Vector2.up, headDetectionRayLength, whatIsGround);
        _bumpedHead = false;
        foreach (RaycastHit2D cast in _headHit)
        {
            if (cast.collider != null && !cast.collider.isTrigger)
            {
                _bumpedHead = true;
            }
        }

        if (debug)
        {
            Color rayColor;
            if (_bumpedHead)
            {
                rayColor = Color.green;
            }
            else
            {
                rayColor = Color.red;
            }

            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2 * headWidth, boxCastOrigin.y), Vector2.up * headDetectionRayLength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x + boxCastSize.x / 2 * headWidth, boxCastOrigin.y), Vector2.up * headDetectionRayLength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2 * headWidth, boxCastOrigin.y * headDetectionRayLength), Vector2.right * boxCastSize.x * headWidth, rayColor);
        }
    }
}
