using System.Collections;
using UnityEngine;

public class FlummiFluffEnemy : BaseEnemy
{
    private const float POINTMERCYAREA = 2f;
    private const float JUMPANIMATIONEND_OFFSET = 0.1f;
    private const float GROUNDCHECK_LENGTH = 1.25f;
    private const float JUMP_DISTANCE_OFFSET_TO_PLAYER = 5f;
    private const float JUMP_TO_PLAYER_OFFSET = 0.75f;
    private const float ATTACKANIMATION_HIT_OFFSET = 0.5f;

    private Vector2 origin;
    private bool foundIdlepoint = false;
    private Vector2 currentTarget = Vector2.zero;
    private Rigidbody2D rigid;
    private bool aggro = false;
    private bool attacked = false;
    private bool jumpToLeft = false;
    private float jumpTimer = 0f;
    private float attackTimer = 0f;
    public float pointLeft = 0;
    public float pointRight = 0;
    public float jumpCooldown = 2f;
    public float jumpTime = 2f;
    public float contactDamage;
    public float contactDamageRange = 1f;
    public float contactCooldown = 0.25f;
    private float contactTimer = 0f;
    private bool isJumping = true;
    public bool isInCave = true;

    private float distance = float.MaxValue;
    public float attackRange = 2f;
    public float attackCooldown = 5f;
    public float damage = 2f;
    public Animator animator;
    [SerializeField] private EnemyPurify purifyHandler;
    private ParticleSystem jumpVFX;
    [SerializeField] private ParticleSystem caveJumpVFX;
    [SerializeField] private ParticleSystem forrestJumpVFX;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private IsPlayerInTrigger aggroRange;
    (Transform transform, S_PlayerHealth health) _Player = (null, null);
    public override void Start()
    {
        Heal();
        origin = transform.position;
        if(aggroRange == null)
        { 
            aggroRange = GetComponentInChildren<IsPlayerInTrigger>(); 
        }
        if(rigid == null)
        { 
            rigid = GetComponent<Rigidbody2D>();
        }
        jumpTimer = 0;
        if (isInCave) 
        {
            jumpVFX = caveJumpVFX;
        }
        else 
        {
            jumpVFX = forrestJumpVFX;
        }
        _Player.health = S_PlayerHealth.GetInstance();
        _Player.transform = _Player.health.transform;
    }
    public override void Move()
    {
        DeterminTargetPoint();
        if (IsGrounded() && Time.time >= jumpTimer)
        {
            InitJump();
        }

    }

    private void DeterminTargetPoint()
    {
         
        if (aggroRange.GetPlayer() != null)
        {
            aggro = aggroRange.IsPlayerInBox();
        }
        if (attacked)
        {
            //Set point away from player as target
            currentTarget = _Player.transform.position - (_Player.transform.position - this.transform.position).normalized* JUMP_DISTANCE_OFFSET_TO_PLAYER;
            currentTarget.y = this.transform.position.y;
        }

        else if (aggro)
        {
            //Set player as target
            
            currentTarget = _Player.transform.position - (_Player.transform.position - this.transform.position).normalized*(attackRange- JUMP_TO_PLAYER_OFFSET);
            currentTarget.y = this.transform.position.y;
        }
        else if(!foundIdlepoint)
        {
            foundIdlepoint = true;
            ChooseIdlePoint();
        }
    }

    IEnumerator ActivateJumpEndAnimation() 
    {
        yield return new WaitForSeconds(jumpTime- JUMPANIMATIONEND_OFFSET);
        animator.SetTrigger("EndJump");
        isJumping = false;
    }

    private void ChooseIdlePoint()
    {
        if (jumpToLeft) 
        {
            jumpToLeft = false;
            currentTarget = origin;
            currentTarget.x -= pointLeft;
        }
        else 
        {
            jumpToLeft = true;
            currentTarget = origin;
            currentTarget.x += pointRight;
        }
    }

    private void InitJump() 
    {
        if (attacked) 
        {
            attacked = false;
        }
        else if (distance < attackRange)
        {
            return; //is close enough to player dosent need to move
        }
        if (foundIdlepoint)
        {
            foundIdlepoint = false;
        }

        Vector2 startPos = this.transform.position;
        Vector2 goalPos = currentTarget;


        float velocityX = ((goalPos.x - startPos.x)/jumpTime);

        float velocityY = 0.5f*-Physics2D.gravity.y*jumpTime; // Grüße gehen raus an horea



        float xScale = Mathf.Abs(animator.transform.localScale.x) * Mathf.Sign(-velocityX);
        animator.transform.localScale = new Vector3(xScale, animator.transform.localScale.y, animator.transform.localScale.z);
        Vector2 velocity = new Vector2(velocityX, velocityY);
        Debug.Log(Mathf.Pow(velocityY*(jumpTime/2) + -1/2* Physics2D.gravity.y*(jumpTime/2),2));
        rigid.linearVelocity = velocity;
        animator.SetTrigger("Jump");
        jumpVFX.Play();
        isJumping = true;
        StartCoroutine(ActivateJumpEndAnimation());
        jumpTimer = Time.time + jumpCooldown;  

    }

    
    public override void Update()
    {
        base.Update();
    }
    private bool IsGrounded() 
    {
        bool onGround= false;
        RaycastHit2D[] rays = Physics2D.RaycastAll(transform.position, Vector2.down, GROUNDCHECK_LENGTH * transform.localScale.y, groundLayer);
        foreach(RaycastHit2D ray in rays) 
        {
            if (ray != false && !ray.collider.isTrigger)
            {
                onGround = true;
                break;
            }
        }
       

        if (!onGround) 
        {
            jumpTimer = Time.time + jumpCooldown;
        }
        return onGround;
    }

    public override void ReachedZeroHitpoints()
    {
        purifyHandler.SetStunned(this);
    }


    IEnumerator Hit()
    {
        yield return new WaitForSeconds(ATTACKANIMATION_HIT_OFFSET);
        if (distance <= attackRange)
        {
            _Player.health.TakeDamage((int)damage);
        }
    }

    public override void Attack()
    {
        if(_Player.transform != null)
        {
            distance = Vector2.Distance(this.transform.position, _Player.transform.position);
            if (distance <= attackRange && Time.time >= attackTimer) 
            {
                attackTimer = Time.time+ attackCooldown;
                StartCoroutine(Hit());
                animator.SetTrigger("Attack");
                
            }
            if (distance <= contactDamageRange && _Player.health != null && Time.time >= contactTimer && !IsStunned())
            {
                contactTimer = Time.time + contactCooldown;
                _Player.health.TakeDamage((int)contactDamage);
            }

        }
    }
    public override void Hurt(float damage)
    {
        base.Hurt(damage);
        attacked = true;
        if (!isJumping)
        {
            animator.SetTrigger("Hurt");
        }
    }

    private bool HasReachedPoint()
    {
        Vector2 point = this.transform.position;
        Vector2 target = currentTarget;
        return
            point.x < target.x + POINTMERCYAREA &&
            point.x > target.x - POINTMERCYAREA;
    }

    private void OnDrawGizmos()
    {
        if(origin == Vector2.zero) 
        {
            origin = transform.position;
        }   
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(new Vector3(currentTarget.x,currentTarget.y,this.transform.position.z), POINTMERCYAREA/2);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(new Vector3(origin.x + pointRight,origin.y,this.transform.position.z), POINTMERCYAREA/2);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(new Vector3(origin.x - pointLeft, origin.y, this.transform.position.z), POINTMERCYAREA/2);

        Gizmos.DrawLine(transform.position, transform.position + (new Vector3(0,-1,1)* GROUNDCHECK_LENGTH * transform.localScale.y));
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision != null) 
        {
            rigid.linearVelocity = Vector2.zero; // makes sure enemy dosent glide on the ground
            if (jumpVFX != null) 
            {
                jumpVFX.Play();
            }
        }
    }


    

    

}
