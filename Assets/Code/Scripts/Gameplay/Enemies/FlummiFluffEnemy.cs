using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class FlummiFluffEnemy : BaseEnemy
{
    private const float POINTMERCYAREA = 2f;
    private const float HIT_BLINK_DURATION = 0.1f;
    private const float JUMPANIMATIONEND_OFFSET = 0.1f;
    private const float GROUNDCHECK_LENGTH = 1.25f;
    private const float JUMP_DISTANCE_OFFSET_TO_PLAYER = 5f;
    private const float JUMP_TO_PLAYER_OFFSET = 0.75f;
    private const float ATTACKANIMATION_HIT_OFFSET = 0.5f;
    private const string JUMP_ANIMATION_NAME= "FlummiFluffJump";
    private const float JUMP_ANIMATIONSPEED_MULTIPLIER = 0.75f;
    private const float JUMP_VELOCITY_START_DELAY = 0.136f;

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

    private float jumpAnimationSpeed;
    private float distance = float.MaxValue;
    public float attackRange = 2f;
    public float attackCooldown = 5f;
    public float damage = 2f;
    public Animator animator;
    [SerializeField] private EnemyPurify purifyHandler;
    [SerializeField] private Color hitColor;
    [SerializeField] private SpriteRenderer _sprite;
    [SerializeField] public float jumpHeight = 1f;
    private ParticleSystem jumpVFX;
    [SerializeField] private ParticleSystem caveJumpVFX;
    [SerializeField] private ParticleSystem forrestJumpVFX;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private IsPlayerInTrigger aggroRange;
    (Transform transform, S_PlayerHealth health) _Player = (null, null);
    [SerializeField] AudioClip jumpImpactSFX;
    [SerializeField] AudioClip hurtSFX;
    [SerializeField] AudioClip[] attackSFX;
    [SerializeField] AudioClip jumpSFX;
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
        rigid.gravityScale = jumpHeight;
        jumpAnimationSpeed = CalculateAnimationspeedForJump();
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

    public float CalculateAnimationspeedForJump() 
    {
        float speed = 0;
        float length = 0;
        foreach (var clip in animator.runtimeAnimatorController.animationClips)
        {
            if(clip.name == JUMP_ANIMATION_NAME) 
            {
                length = clip.length;
                break;
            }
        }
        speed = length / jumpTime;
        return speed * JUMP_ANIMATIONSPEED_MULTIPLIER;
    }

    IEnumerator ActivateJumpEndAnimation() 
    {
        yield return new WaitForSeconds(jumpTime);
        //animator.StopPlayback();
        animator.SetTrigger("EndJump");
        //isJumping = false;
        animator.speed = 1;
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


        float velocityX = ((goalPos.x - startPos.x) / jumpTime);

        float velocityY = 0.5f * -Physics2D.gravity.y * jumpTime * jumpHeight; // Grüße gehen raus an horea


        AudioManager.instance?.PlaySoundFXClip(jumpSFX, transform, .2f);
        float xScale = Mathf.Abs(animator.transform.localScale.x) * Mathf.Sign(-velocityX);
        animator.transform.localScale = new Vector3(xScale, animator.transform.localScale.y, animator.transform.localScale.z);
        Vector2 velocity = new Vector2(velocityX, velocityY);
        StartCoroutine(SetVelocityDelayed(velocity));
        animator.SetTrigger("Jump");
        jumpVFX.Play();
        isJumping = true;
        animator.speed = jumpAnimationSpeed;
        StartCoroutine(ActivateJumpEndAnimation());
        jumpTimer = Time.time + jumpCooldown;

    }

    IEnumerator SetVelocityDelayed(Vector2 velocity)
    {
        yield return new WaitForSeconds(JUMP_VELOCITY_START_DELAY);
        rigid.linearVelocity = velocity;
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
        purifyHandler.healthAmountRestoredOnPurify = healthAmountRestoredOnPurify;
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
                AudioManager.instance?.PlayRandomSoundFXClip(attackSFX, transform, .5f);


            }
            if (distance <= contactDamageRange && _Player.health != null && Time.time >= contactTimer && !IsStunned())
            {
                contactTimer = Time.time + contactCooldown;
                _Player.health.TakeDamage((int)contactDamage);
            }

        }
    }
    public override void Hurt(float damage, int attackID)
    {
        base.Hurt(damage, attackID);
        StopCoroutine(HitFeedBack());
        _sprite.color = Color.white;
        StartCoroutine(HitFeedBack());
        attacked = true;
        AudioManager.instance?.PlaySoundFXClip(hurtSFX, transform, .5f);
        if (!isJumping)
        {
            animator.SetTrigger("Hurt");
        }
    }



    IEnumerator HitFeedBack()
    {
        if (_sprite != null)
        {
            _sprite.color = hitColor;
            //AudioManager.instance.PlayRandomSoundFXClip(hitSFX, transform, 1f);
            yield return new WaitForSeconds(HIT_BLINK_DURATION);
            _sprite.color = Color.white;
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

    //private void OnDrawGizmos()
    //{
    //    if(origin == Vector2.zero) 
    //    {
    //        origin = transform.position;
    //    }   
    //    Gizmos.color = Color.green;
    //    Gizmos.DrawWireSphere(new Vector3(currentTarget.x,currentTarget.y,this.transform.position.z), POINTMERCYAREA/2);

    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(new Vector3(origin.x + pointRight,origin.y,this.transform.position.z), POINTMERCYAREA/2);

    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawWireSphere(new Vector3(origin.x - pointLeft, origin.y, this.transform.position.z), POINTMERCYAREA/2);

    //    Gizmos.DrawLine(transform.position, transform.position + (new Vector3(0,-1,1)* GROUNDCHECK_LENGTH * transform.localScale.y));
    //}
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
