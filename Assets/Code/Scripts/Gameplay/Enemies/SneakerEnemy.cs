
using System.Collections;
using UnityEngine;

public class SneakerEnemy : BaseEnemy
{
    private const float POINTMERCYAREA = 0.25f;
    private const float ATTACKANIMATION_HIT_OFFSET = 0.5f;
    [SerializeField] private EnemyPurify purifyHandler;
    [SerializeField] private float hitRange;
    [SerializeField] private float contactRange = 1f;
    [SerializeField] private float contactDamage;
    [SerializeField] private float contactCooldown = 0.25f;
    private float contactTimer = 0;
    [SerializeField] private float damage;
    [SerializeField] private bool returnIfBeingLookedAtWhileAttacked;
    [SerializeField] private int wanderRange;
    [SerializeField] private float attackCooldown;
    private float attackTimer;
    [SerializeField] private float wanderMaxTime;
    [SerializeField] private float speed;
    [SerializeField] private float sneakSpeedFactor = 1f;
    [SerializeField] private float lookedAtSpeedFactor = 1f;
    [SerializeField] private LayerMask wallLayer;
    IsPlayerInTrigger attackRadius;
    private float idleTimer;
    private bool canAttack = true;
    Rigidbody2D rigi;
    (Transform transform, Rigidbody2D rigidbody2D, S_PlayerHealth health) _Player = (null, null, null);
    private Animator animator;
    Vector3 currentTargetPoint = Vector3.zero;
    Vector2 direction = Vector2.zero;
    Vector3 originPoint = Vector3.zero;
    float distanceToPlayer = float.MaxValue;

    public void Awake()
    {
        originPoint = transform.position;
        if (rigi == null)
        {
            rigi = GetComponent<Rigidbody2D>();
        }
        if (attackRadius == null)
        {
            attackRadius = GetComponentInChildren<IsPlayerInTrigger>();
        }
        currentTargetPoint = RandomTargetPoint();
        animator = GetComponent<Animator>();
    }

    public override void Move()
    {
        if (attackRadius.IsPlayerInBox())
        {
            if (_Player.rigidbody2D == null || _Player.transform == null || _Player.health == null)
            {
                GameObject collision = attackRadius.GetPlayer().gameObject;
                _Player.rigidbody2D = collision.GetComponent<Rigidbody2D>();
                _Player.transform = collision.transform;
                _Player.health = collision.GetComponent<S_PlayerHealth>();
            }



            currentTargetPoint = _Player.transform.position;
            direction = (currentTargetPoint - transform.position).normalized;
            if (distanceToPlayer <= hitRange && (IsBeingLookedAt() && !returnIfBeingLookedAtWhileAttacked))
            {
                canAttack = true;
                return; // is close enought to attack no need to get Closer
            }
            LookAtDirection();

            if (!IsBeingLookedAt())
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
        return Mathf.Sign(offset) == Mathf.Sign(_Player.transform.localScale.x);
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
        base.Hurt(damage);
        animator.SetTrigger("Hurt");
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(originPoint, Vector3.one * wanderRange * 2);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(currentTargetPoint, POINTMERCYAREA);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(direction.x, direction.y));
    }


}
