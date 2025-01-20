using JetBrains.Annotations;
using UnityEngine;

public class CrystalProjectileEnemy : BaseEnemy
{
    private HeadPhase owner;
    private const float POINTMERCYAREA = 0.25f;
    [SerializeField] private float hitRange;
    [SerializeField] public float damage;
    [SerializeField] private int wanderRange;
    [SerializeField] private float attackCooldown;
    private float attackTimer;
    [SerializeField] private float wanderMaxTime;
    public float speed;
    [SerializeField] private LayerMask wallLayer;
    IsPlayerInTrigger attackRadius;
    private float idleTimer;
    Rigidbody2D rigi;
    public (Rigidbody2D rigidbody2D, S_PlayerHealth health) _Player;
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

    public void OnEnable()
    {
        this.transform.position = originPoint;
        Heal();
    }

    public void SetPlayer(Rigidbody2D rigidbody, S_PlayerHealth health) 
    {
        _Player.health = health;
        _Player.rigidbody2D = rigidbody;
    }

    public override void Move()
    {
        if (attackRadius.GetPlayer() != null)
        {
            if (attackRadius.IsPlayerInBox())
            {
                //target Player
                //Move closer
                RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 1f, wallLayer);
                if (hit.collider == null)
                {
                    currentTargetPoint = _Player.health.transform.position;
                }   
                direction = (currentTargetPoint - transform.position).normalized;
                transform.position += (Vector3)(direction * speed * Time.deltaTime);
                
                return;
            }
        }
        
        

        //idle movement
        if (idleTimer + wanderMaxTime < Time.time || HasReachedPoint())
        {
            idleTimer = Time.time;
            currentTargetPoint = RandomTargetPoint();

        }
        direction = (currentTargetPoint - transform.position).normalized;
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
        //Move to Point


    }

    public void SetOwner(HeadPhase headPhase) 
    {
        owner = headPhase;
    }

    public void SetOrigin(Vector3 newOrigin) 
    {
        originPoint = newOrigin;
    }

    public override void Attack()
    {
        if (distanceToPlayer <= hitRange && attackTimer >= attackCooldown)
        {
            //attack
            _Player.health.TakeDamage((int)damage);
            ReachedZeroHitpoints();
        }
    }

    public override void ReachedZeroHitpoints()
    {
        owner.CrystalDestroyed(this);
        gameObject.SetActive(false);

    }

    public override void Update()
    {
        base.Update();
        if (_Player != (null,null))
        {
            distanceToPlayer = (_Player.health.transform.position - this.transform.position).sqrMagnitude;
        }

        else 
        {
            _Player = owner.player;
        }
        attackTimer += Time.deltaTime;

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
        float offset = this.transform.position.x - _Player.health.transform.position.x;
        return Mathf.Sign(offset) == Mathf.Sign(_Player.health.transform.localScale.x);
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
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(currentTargetPoint, POINTMERCYAREA);
    }


}
