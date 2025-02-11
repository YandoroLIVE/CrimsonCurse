
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class SneakerEnemy : BaseEnemy
{
    const float HIT_BLINK_DURATION = 0.1f;
    private const float POINTMERCYAREA = 0.25f;
    private const float ATTACKANIMATION_HIT_OFFSET = 0.5f;
    [SerializeField] private EnemyPurify purifyHandler;
    [SerializeField] private Color hitColor;
    [SerializeField] private SpriteRenderer _sprite;
    [SerializeField] private float hitRange;
    [SerializeField] private float contactRange = 1f;
    [SerializeField] private float contactDamage;
    [SerializeField] private float contactCooldown = 0.25f;
    private float contactTimer = 0;
    [SerializeField] private float damage;
    [SerializeField] private bool returnIfBeingLookedAtWhileAttacked;
    [SerializeField] private int wanderRange;
    [SerializeField] private float attackCooldown;
    [SerializeField] private float lookReactionDelayTime = 0.5f;
    private float attackTimer;
    [SerializeField] private float wanderMaxTime;
    [SerializeField] private float speed;
    [SerializeField] private float sneakSpeedFactor = 1f;
    [SerializeField] private float lookedAtSpeedFactor = 1f;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float aggroRange = 5;
    [SerializeField] AudioClip[] attackSFX;
    [SerializeField] AudioClip hurtSFX;
    //IsPlayerInTrigger attackRadius;
    private float idleTimer;
    private bool canAttack = true;
    private bool lookedAt = false;
    private bool reactingToLook = false;
    Rigidbody2D rigi;
    (Transform transform, S_PlayerHealth health) _Player = (null, null);
    private Animator animator;
    Vector3 currentTargetPoint = Vector3.zero;
    Vector2 direction = Vector2.zero;
    Vector3 originPoint = Vector3.zero;
    float distanceToPlayer = float.MaxValue;
    Vector2 debugDraw;

    public void Awake()
    {
        originPoint = transform.position;
        if (rigi == null)
        {
            rigi = GetComponent<Rigidbody2D>();
        }
        //if (attackRadius == null)
        //{
        //    attackRadius = GetComponentInChildren<IsPlayerInTrigger>();
        //}
        currentTargetPoint = RandomTargetPoint();
        animator = GetComponent<Animator>();
    }

    public override void Start()
    {
        base.Start();
        Heal();
        _Player.health = S_PlayerHealth.GetInstance();
        _Player.transform = _Player.health.transform;
    }

    public override void Move()
    {
        IsBeingLookedAt();
        if (CanSeePlayer())
        {
            currentTargetPoint = _Player.transform.position;
            direction = (currentTargetPoint - transform.position).normalized;
            if (distanceToPlayer <= hitRange && (lookedAt && !returnIfBeingLookedAtWhileAttacked))
            {
                canAttack = true;
                return; // is close enought to attack no need to get Closer
            }
            LookAtDirection();

            if (!lookedAt)
            {
                transform.position += ((Vector3)direction * speed * Time.deltaTime) * sneakSpeedFactor;
                canAttack = true;
            }

            else
            {
                canAttack = false;
                currentTargetPoint = originPoint;
                direction = (currentTargetPoint - transform.position).normalized;
                LookAtDirection();
                if (HasReachedPoint())
                {
                    LookAtPlayer();
                    return;
                }
                transform.position += ((Vector3)direction * speed * Time.deltaTime) * lookedAtSpeedFactor;
            }

        }

        //idle movement
        else if (idleTimer + wanderMaxTime < Time.time || HasReachedPoint())
        {
            idleTimer = Time.time;
            currentTargetPoint = RandomTargetPoint();
            
        }
        else
        {
            direction = (currentTargetPoint - transform.position).normalized;
            transform.position += (Vector3)direction * speed * Time.deltaTime;
            LookAtDirection();
        }

        //Move to Point


    }

    private bool CanSeePlayer()
    {
        bool canSee = true;
        Vector2 dir = _Player.transform.position - this.transform.position;
        dir = dir.normalized;
        float distance = Vector2.Distance(transform.position, _Player.transform.position);
        if (distance > aggroRange)
        {
            return canSee = false;
        }
        float range = distance > aggroRange ? aggroRange : distance;
        debugDraw = dir * range;
        var objectsHit = Physics2D.RaycastAll(this.transform.position, dir, range, wallLayer);
        foreach (var obj in objectsHit)
        {
            if (!obj.collider.isTrigger)
            {
                return canSee = false;
            }

        }
        objectsHit = Physics2D.RaycastAll(this.transform.position, dir, range, groundLayer);
        foreach (var obj in objectsHit)
        {
            if (!obj.collider.isTrigger)
            {
                return canSee = false;
            }

        }
        

        return canSee;
    }

    private void LookAtDirection()
    {
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * Mathf.Sign(-direction.x);
        transform.localScale = scale;
    }

    private void LookAtPlayer()
    {
        Vector3 scale;
        float xScale = _Player.transform.position.x - this.transform.position.x;
        scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * Mathf.Sign(-xScale);
        transform.localScale = scale;
    }

    IEnumerator Hit()
    {
        yield return new WaitForSeconds(ATTACKANIMATION_HIT_OFFSET);
        LookAtPlayer();
        if (distanceToPlayer <= hitRange)
        {
            _Player.health.TakeDamage((int)damage);
        }
    }

    public override void Attack()
    {
        if (distanceToPlayer <= hitRange && attackTimer >= attackCooldown && canAttack)
        {
            //attack
            StartCoroutine(Hit());
            animator.SetTrigger("Attack");
            attackTimer = 0;
            AudioManager.instance?.PlayRandomSoundFXClip(attackSFX, transform, 1f);
            LookAtPlayer();
        }
        if (distanceToPlayer <= contactRange && Time.deltaTime >= contactTimer)
        {
            contactTimer = Time.deltaTime + contactCooldown;

            _Player.health.TakeDamage((int)contactDamage);
        }
    }

    public override void ReachedZeroHitpoints()
    {
        rigi.gravityScale = 1;
        purifyHandler.healthAmountRestoredOnPurify = healthAmountRestoredOnPurify;
        purifyHandler.SetStunned(this);

    }

    public override void Update()
    {
        if (_Player.transform != null)
        {
            distanceToPlayer = Vector2.Distance(_Player.transform.position, transform.position);
        }
        attackTimer += Time.deltaTime;
        base.Update();

    }

    public override void Heal()
    {

        base.Heal();
        rigi.gravityScale = 0;
    }

    private Vector3 RandomTargetPoint()
    {
        int x = Random.Range(0, (wanderRange + 1) * 2) - wanderRange;
        int y = Random.Range(0, (wanderRange + 1) * 2) - wanderRange;
        currentTargetPoint = originPoint + new Vector3(x, y, this.transform.position.z);
        return currentTargetPoint;
    }

    private bool IsBeingLookedAt()
    {
        float offset = this.transform.position.x - _Player.transform.position.x;
        bool look = Mathf.Sign(offset) == Mathf.Sign(_Player.transform.localScale.x);
        if (look && !lookedAt && !reactingToLook)
        {
            StartCoroutine(DelayedLookedAt());
        }

        else if (!look && lookedAt)
        {
            lookedAt = false;
        }

        return look;
    }

    IEnumerator DelayedLookedAt()
    {
        reactingToLook = true;
        yield return new WaitForSeconds(lookReactionDelayTime);
        lookedAt = true;
        reactingToLook = false;
    }

    private bool HasReachedPoint()
    {
        Vector2 point = this.transform.position;
        Vector2 target = currentTargetPoint;
        return
            point.x < target.x + POINTMERCYAREA &&
            point.x > target.x - POINTMERCYAREA &&
            point.y < target.y + POINTMERCYAREA &&
            point.y > target.y - POINTMERCYAREA;
    }

    public override void Hurt(float damage)
    {

        StopCoroutine(HitFeedBack());
        _sprite.color = Color.white;
        StartCoroutine(HitFeedBack());
        base.Hurt(damage);
        animator.SetTrigger("Hurt");
    }


    IEnumerator HitFeedBack()
    {
        if (_sprite != null)
        {
            _sprite.color = hitColor;
            AudioManager.instance.PlaySoundFXClip(hurtSFX, transform, 1f);
            yield return new WaitForSeconds(HIT_BLINK_DURATION);
            _sprite.color = Color.white;
        }
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.green;
    //    Gizmos.DrawWireCube(originPoint, Vector3.one * wanderRange * 2);
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(currentTargetPoint, POINTMERCYAREA);
    //    Gizmos.color = Color.cyan;
    //    Gizmos.DrawWireSphere(this.transform.position, aggroRange);

    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawLine(this.transform.position, this.transform.position + (Vector3)debugDraw);
    //}


}
