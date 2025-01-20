using System.Collections;
using UnityEngine;

public class WingAttack : MonoBehaviour
{
    private const int PUSH_AMOUNT = 20;
    [SerializeField] ParticleSystem _AttackVFX;
    [SerializeField] Vector2 _PushDirection;
    [SerializeField] float _PushStrength;
    [SerializeField] float _PushDelay;
    [SerializeField] IsPlayerInTrigger _PlayerDetector;
    [SerializeField] GameObject warnZone;
    public float _Damage;
    public float warnduration;
    public float attackInterval;
    public float attackDuration;
    public float timer;
    private (Rigidbody2D rigidbody2D, S_PlayerHealth health) _Player;
    private bool attacking;
    private float attackTimer;
    
    public void SetPlayer((Rigidbody2D, S_PlayerHealth) player) 
    {
        _Player = player;
    }
    private void Push(Rigidbody2D target)
    {
        //target.linearDamping = 0;
        Vector2 pushforce = (_PushDirection.normalized * _PushStrength);
        target.AddForce(pushforce);

    }

    IEnumerator DelayPush(Rigidbody2D target) 
    {
        for (int i = 0; i < PUSH_AMOUNT; i++)
        {
            yield return new WaitForSeconds(_PushDelay);
            Push(target);
        }
        _Player.health.TakeDamage(((int)_Damage));
    }

    private void Playeffects()
    {
        _AttackVFX.Play();
    }

    private void Awake()
    {
        warnZone.SetActive(false);
        timer = attackInterval;
    }

    public void Attack()
    {
        if (_Player.rigidbody2D == null || _Player.health == null)
        {
            Collider2D collision = _PlayerDetector.GetPlayer();
            _Player.rigidbody2D = collision.gameObject.GetComponent<Rigidbody2D>();
            _Player.health = collision.gameObject.GetComponent<S_PlayerHealth>();
        }
        if(transform.position.x <= _Player.rigidbody2D.transform.position.x) 
        {
            _PushDirection.x = 1;
        }
        else _PushDirection.x = -1;
        StartCoroutine(DelayPush(_Player.rigidbody2D));
        
    }

    public void Cycle(float delta)
    {
        if (warnZone != null && timer >= attackInterval - warnduration)
        {
            warnZone.SetActive(true);
        }
        if (timer >= attackInterval)
        {
            attacking = true;
            Playeffects();
            timer = 0;
            if (_PlayerDetector.IsPlayerInBox())
            {
                Attack();
            }
            warnZone.SetActive(false);
        }
        else if (attacking)
        {
            if (_PlayerDetector.IsPlayerInBox()) 
            {
                Attack();
            }
            
            attackTimer += delta;
            
            if (attackTimer >= attackDuration)
            {
                attackTimer = 0;
                attacking = false;
            }
        }
        else 
        {
            timer += delta;
        }
    }
    public void OnEnable()
    {
        warnZone.SetActive(false);
        timer = 0;
        attackTimer = 0;
    }
}
