using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class FlummiFluffEnemy : BaseEnemy
{
    private const float MINJUMPDIST = 0.25f;
    private const float POINTMERCYAREA = 0.25f; 
    private Vector2 origin;
    private Vector2 currentTarget = Vector2.zero;
    private Rigidbody2D rigid;
    private bool aggro = false;
    private bool attacked = false;
    private float jumpTimer = 0f;
    private float attackTimer = 0f;
    public float jumpCooldown = 2f;
    public float jumpSpeed = 3f;
    public float attackRange = 2f;
    public float attackCooldown = 5f;
    public float damage = 2f;
    [SerializeField] private EnemyPurify purifyHandler;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Vector2 jumpXIdleRange;
    [SerializeField] private IsPlayerInTrigger aggroRange;
    (Transform transform, Rigidbody2D rigidbody2D, S_PlayerHealth health) _Player = (null, null, null);
    public override void Start()
    {
        
        origin = transform.position;
        aggroRange = GetComponentInChildren<IsPlayerInTrigger>();
        rigid = GetComponent<Rigidbody2D>();
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
            //get player component the first time that the player enters the aggro range
            if (_Player.rigidbody2D == null || _Player.transform == null || _Player.health == null)
            {
                GameObject collision = aggroRange.GetPlayer().gameObject;
                _Player.rigidbody2D = collision.GetComponent<Rigidbody2D>();
                _Player.transform = collision.transform;
                _Player.health = collision.GetComponent<S_PlayerHealth>();
            }

            aggro = aggroRange.IsPlayerInBox();
        }
        if (attacked)
        {
            //Set point away from player as target
            currentTarget = _Player.transform.position - (_Player.transform.position - this.transform.position).normalized*5f;
            currentTarget.y = this.transform.position.y;
        }

        else if (aggro)
        {
            //Set player as target
            currentTarget = _Player.transform.position - (_Player.transform.position - this.transform.position).normalized;
            currentTarget.y = this.transform.position.y;
        }
        else
        {
            RandomIdlePoint();
        }
    }

    private void RandomIdlePoint()
    {
        float x = Random.Range(jumpXIdleRange.x, jumpXIdleRange.y + 1);
        //to do check that the x position isnt to close to the current position
        float y = this.transform.position.y;
        currentTarget = new Vector2(origin.x + x, y);
    }

    private void InitJump() 
    {
        if (attacked) 
        {
            attacked = false;
        }
        Vector2 startPos = this.transform.position;
        Vector2 goalPos = currentTarget;

        // Calculate distance and time
        float distancX = goalPos.x - startPos.x;
        float distancY = goalPos.y - startPos.y;
        float distance = Vector2.Distance(startPos, goalPos);
        if(distance <= MINJUMPDIST) 
        {
            return;
        }
        float speed = jumpSpeed;
        float timeOfFlight = distance / speed;
        float gravity = Mathf.Abs(Physics2D.gravity.y);
        float velocityY = (distancY + 0.5f * gravity * Mathf.Pow(timeOfFlight, 2)) / timeOfFlight;
        float velocityX = distancX / timeOfFlight;
        Vector2 velocity = new Vector2(velocityX, velocityY);
        rigid.linearVelocity = velocity;  
        jumpTimer = Time.time + jumpCooldown;  

    }

    
    public override void Update()
    {
        base.Update();
    }
    private bool IsGrounded() 
    {
        bool onGround= false;

        if(Physics2D.Raycast(transform.position,Vector2.down,1,groundLayer) != false) 
        {
            onGround = true;
        }
        return onGround;
    }

    public override void ReachedZeroHitpoints()
    {
        purifyHandler.SetStunned(this);
    }

    public override void Attack()
    {
        if(_Player.transform != null)
        {
            float distance = Vector2.Distance(this.transform.position, _Player.transform.position);
            if (distance <= attackRange && Time.time >= attackTimer) 
            {
                attackTimer = Time.time+ attackCooldown;
                _Player.health.TakeDamage((int)damage);
                attacked = true;
            }
            
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
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(new Vector3(currentTarget.x,currentTarget.y,this.transform.position.z), POINTMERCYAREA/2);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision != null) 
        {
            rigid.linearVelocity = Vector2.zero; // makes sure enemy dosent glide on the ground
        }
    }
}
