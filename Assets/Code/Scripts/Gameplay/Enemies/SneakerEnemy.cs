using Mono.Cecil.Cil;
using Unity.Hierarchy;
using UnityEngine;

public class SneakerEnemy : BaseEnemy
{
    private const float POINTMERCYAREA = 0.25f;
    [SerializeField] private EnemyPurify purifyHandler;
    [SerializeField] private float hitRange;
    [SerializeField] private float damage;
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
    Rigidbody2D rigi;
    (Transform transform, Rigidbody2D rigidbody2D, S_PlayerHealth health) _Player = (null, null, null);
    Vector3 currentTargetPoint = Vector3.zero;
    Vector2 direction = Vector2.zero;
    Vector3 originPoint = Vector3.zero;
    float distanceToPlayer = float.MaxValue;

    public void Awake()
    {
        originPoint = transform.position;
        rigi = GetComponent<Rigidbody2D>();
        attackRadius = GetComponentInChildren<IsPlayerInTrigger>();
        currentTargetPoint = RandomTargetPoint();
    }

    public override void Move()
    {
        if (attackRadius.GetPlayer() != null)
        {
            if (_Player.rigidbody2D == null || _Player.transform == null || _Player.health == null)
            {
                GameObject collision = attackRadius.GetPlayer().gameObject;
                _Player.rigidbody2D = collision.GetComponent<Rigidbody2D>();
                _Player.transform = collision.transform;
                _Player.health = collision.GetComponent<S_PlayerHealth>();
            }

            if (attackRadius.IsPlayerInBox())
            {
                //target Player
                if (!IsBeingLookedAt())
                {
                    //Move closer   

                    RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 1f, wallLayer);
                    if (hit.collider == null)
                    {
                        currentTargetPoint = _Player.transform.position;
                        //if (HasReachedPoint()) { currentTargetPoint = this.transform.position; }

                    }
                    else
                    {
                        currentTargetPoint = originPoint;

                    }

                    direction = (currentTargetPoint - transform.position).normalized;
                    transform.position += (Vector3)direction * speed * sneakSpeedFactor * Time.deltaTime;
                }
                else
                {
                    //Move Back
                    currentTargetPoint = originPoint;
                    direction = (currentTargetPoint - transform.position).normalized;
                    transform.position += (Vector3)direction * speed * lookedAtSpeedFactor * Time.deltaTime;
                }

            }
        }

        //idle movement
        if (idleTimer + wanderMaxTime < Time.time || HasReachedPoint())
        {
            idleTimer = Time.time;
            currentTargetPoint = RandomTargetPoint();

        }
        direction = (currentTargetPoint - transform.position).normalized;
        transform.position += (Vector3)direction * speed * lookedAtSpeedFactor * Time.deltaTime;
        //Move to Point


    }



    public override void Attack()
    {
        if (distanceToPlayer <= hitRange && attackTimer >= attackCooldown)
        {
            //attack
            _Player.health.TakeDamage((int)damage);
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
            distanceToPlayer = (_Player.transform.position - this.transform.position).sqrMagnitude;
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
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(originPoint, Vector3.one * wanderRange * 2);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(currentTargetPoint, POINTMERCYAREA);
    }


}
