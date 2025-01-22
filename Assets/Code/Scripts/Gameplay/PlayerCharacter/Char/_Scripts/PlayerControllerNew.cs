using UnityEngine;

public class PlayerControllerNew : MonoBehaviour
{
    // Serialized Fields
    [SerializeField] private float speed;
    [SerializeField] private bool forrestLevel;
    [SerializeField] private bool spawnCamera = true;
    [SerializeField] private GameObject cameraPrefab;
    [SerializeField] private ParticleSystem m_dustParticle;
    [SerializeField] private ParticleSystem m_LeafParticle;
    
    [Header("Jumping")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float fallMultiplier;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private ParticleSystem jumpEffect;
    [SerializeField] private ParticleSystem impactEffect;

    [Header("Dashing")]
    [SerializeField] private float dashSpeed = 30f;
    [SerializeField] private float startDashTime = 0.1f;
    [SerializeField] private float dashCooldown = 0.2f;
    [SerializeField] private ParticleSystem dashEffect;

    [Header("Wall grab & jump")]
    [SerializeField] public Vector2 grabRightOffset = new Vector2(0.16f, 0f);
    [SerializeField] public Vector2 grabLeftOffset = new Vector2(-0.16f, 0f);
    [SerializeField] public float grabCheckRadius = 0.24f;
    [SerializeField] public float slideSpeed = 2.5f;
    [SerializeField] public Vector2 wallJumpForce = new Vector2(10.5f, 18f);
    [SerializeField] public Vector2 wallClimbForce = new Vector2(4f, 14f);

    // Public Fields
    [SerializeField] public bool isGrounded;
    [HideInInspector] public float moveInput;
    [HideInInspector] public bool canMove = true;
    [HideInInspector] public bool isDashing = false;
    [HideInInspector] public bool actuallyWallGrabbing = false;
    [HideInInspector] public bool isCurrentlyPlayable = false;
    public bool pickedUpDash = false;
    public bool hasWallJump = false;

    // Private Fields
    private Rigidbody2D m_rb;
    private bool m_facingRight = true;
    private ParticleSystem runParticle;
    private readonly float m_groundedRememberTime = 0.25f;
    private float m_groundedRemember;
    private float m_dashTime;
    private bool m_hasDashedInAir;
    private bool m_onWall;
    private bool m_onRightWall;
    private bool m_onLeftWall;
    private bool m_wallGrabbing;
    private readonly float m_wallStickTime = 1.25f;
    private float m_wallStick;
    private bool m_wallJumping;
    private float m_dashCooldown;


    private int m_onWallSide;
    private int m_playerSide = 1;

    // Input String Caching
    private static readonly string HorizontalInput = "Horizontal";
    private static readonly string JumpInput = "Jump";
    private static readonly string DashInput = "Dash";

    public static float HorizontalRaw() => Input.GetAxisRaw(HorizontalInput);
    public static bool Jump() => Input.GetKeyDown(KeyCode.Space);
    public static bool Dash() => Input.GetKeyDown(KeyCode.LeftShift);

    public void Awake()
    {
        if(spawnCamera)
        {
            Instantiate(cameraPrefab);
        }
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
        if (transform.CompareTag("Player"))
            isCurrentlyPlayable = true;

        m_dashTime = startDashTime;
        m_dashCooldown = dashCooldown;

        m_rb = GetComponent<Rigidbody2D>();
        m_dustParticle = GetComponentInChildren<ParticleSystem>();
    }


    private void FixedUpdate()
    {
        UpdateGroundedState();
        UpdateWallState();
        CalculateSides();
        HandleWallSlide();
        HandleMovement();
        HandleDash();
    }

    private void Update()
    {
        moveInput = HorizontalRaw();
        HandleJump();
        HandleDashInput();
    }

    private void UpdateGroundedState()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, groundCheckRadius, whatIsGround);
        bool wasGrounded = isGrounded;
        isGrounded = false;
        foreach (var collider in colliders)
        {
            if (collider.isTrigger == false)
            {
                isGrounded = true;
                if (!wasGrounded) 
                {
                    impactEffect.Play();
                    runParticle.Play();
                }
                break;
            }
        }
        if (!isGrounded) 
        {
            runParticle.Stop();
        }
        m_groundedRemember -= Time.deltaTime;
        if (isGrounded)
        {
            m_groundedRemember = m_groundedRememberTime;
            if (m_wallJumping)
            {
                m_wallJumping = false;  // Reset WallJump flag after landing
            }
        }
    }

    private void UpdateWallState()
    {
        var position = (Vector2)transform.position;
        Collider2D[] rightColliders = Physics2D.OverlapCircleAll(position + grabRightOffset, grabCheckRadius, whatIsGround);
        m_onRightWall = CheckIfNonTrigger(rightColliders);
        Collider2D[] leftColliders = Physics2D.OverlapCircleAll(position + grabLeftOffset, grabCheckRadius, whatIsGround);
        m_onLeftWall = CheckIfNonTrigger(leftColliders);
        m_onWall = m_onRightWall || m_onLeftWall;
    }
    private bool CheckIfNonTrigger(Collider2D[] colliders)
    {
        foreach (var collider in colliders)
        {
            if (!collider.isTrigger)
                return true;
        }
        return false;
    }


    private void CalculateSides()
    {
        m_onWallSide = m_onRightWall ? 1 : m_onLeftWall ? -1 : 0;
        m_playerSide = m_facingRight ? 1 : -1;
    }

    private void HandleWallSlide()
    {

        if (m_onWall && !isGrounded && m_rb.linearVelocity.y <= 0f && m_playerSide == m_onWallSide)
        {
            m_wallGrabbing = true;
            actuallyWallGrabbing = true;
            m_rb.linearVelocity = new Vector2(moveInput * speed, -slideSpeed);
            m_wallStick = m_wallStickTime;
        }
        else
        {
            m_wallStick -= Time.deltaTime;
            actuallyWallGrabbing = false;
            if (m_wallStick <= 0f) m_wallGrabbing = false;
        }

        if (m_wallGrabbing && isGrounded) m_wallGrabbing = false;
    }

    private void HandleMovement()
    {
        if (!isCurrentlyPlayable) return;

        if (m_wallJumping)
        {
            m_rb.linearVelocity = Vector2.Lerp(m_rb.linearVelocity, new Vector2(moveInput * speed, m_rb.linearVelocity.y), 1.5f * Time.fixedDeltaTime);
        }
        else if (canMove && !m_wallGrabbing)
        {
            // Setze die horizontale Geschwindigkeit auf 0, wenn keine Eingabe erfolgt
            if (moveInput == 0f)
            {
                m_rb.linearVelocity = new Vector2(0f, m_rb.linearVelocity.y);
            }
            else
            {
                m_rb.linearVelocity = new Vector2(moveInput * speed, m_rb.linearVelocity.y);
            }
        }
        else if (!canMove)
        {
            m_rb.linearVelocity = new Vector2(0f, m_rb.linearVelocity.y);
        }

        if (m_rb.linearVelocity.y < 0f)
        {
            m_rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
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
            m_dashCooldown = dashCooldown;
            m_dashTime = startDashTime;
            m_rb.linearVelocity = Vector2.zero;
        }
        else
        {
            m_dashTime -= Time.deltaTime;
            m_rb.linearVelocity = (m_facingRight ? Vector2.right : Vector2.left) * dashSpeed;
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
            if (!isGrounded) m_hasDashedInAir = true;
        }

        if (m_hasDashedInAir && isGrounded) m_hasDashedInAir = false;
    }

    private void HandleJump()
    {
        if (Jump() && isGrounded)
        {
            jumpEffect.Play();
            m_rb.linearVelocity = new Vector2(m_rb.linearVelocity.x, jumpForce);
        }
        else if (hasWallJump && Jump() && m_wallGrabbing && moveInput != m_onWallSide)
        {
            PerformWallJump(wallJumpForce);
        }
        else if (hasWallJump && Jump() && m_wallGrabbing && moveInput == m_onWallSide)
        {
            PerformWallJump(wallClimbForce);
        }
    }


    private void PerformWallJump(Vector2 force)
    {
        m_wallGrabbing = false;
        m_wallJumping = true;
        if (m_playerSide == m_onWallSide) Flip();
        m_rb.AddForce(new Vector2(-m_onWallSide * force.x, force.y), ForceMode2D.Impulse);
    }
    private void Flip()
    {
        m_facingRight = !m_facingRight;
        var scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + grabRightOffset, grabCheckRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + grabLeftOffset, grabCheckRadius);
    }
}