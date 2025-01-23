using UnityEngine;
using System.Collections;
using NUnit.Framework;
using System.Collections.Generic;

[System.Serializable]
public class PusherEnemy : BaseEnemy
{
    [SerializeField] float _PushStrength;
    [SerializeField] float _Damage;
    public float attackCooldown;
    public float projectileSpeed;
    [SerializeField] EnemyPurify purifycationHandler;
    [SerializeField] IsPlayerInTrigger _AttackTrigger;
    [SerializeField] PusherProjectile _ProjectilePrefab;
    private List<PusherProjectile> projectiles = new List<PusherProjectile>();
    private (Rigidbody2D rigidbody2D,S_PlayerHealth health) _Player = (null,null);
    private float timer;
    private Vector2 lookDirection = new Vector2(0, -1); //Default position pusher looks at


    private void Playeffects()
    {
        // play shooting animation
    }

   

    private void Awake()
    {
        Heal();        
        if (purifycationHandler == null) 
        {
            GetComponentInChildren<EnemyPurify>();
        }

        timer = Time.time;
    }

    public override void Move()
    {
        //pusher dos not move
    }

    public override void Attack() 
    {

        if (_AttackTrigger.IsPlayerInBox())
        {
            if (_Player.rigidbody2D == null || _Player.health == null)
            {
                Collider2D collision = _AttackTrigger.GetPlayer();
                _Player.rigidbody2D = collision.gameObject.GetComponent<Rigidbody2D>();
                _Player.health = collision.gameObject.GetComponent<S_PlayerHealth>();
            }
            if (timer+attackCooldown <= Time.time) 
            {
                timer = Time.time;
                Shoot();
                //Playeffects();
                //StartCoroutine(DelayPush());
            }
        }
    }

    public override void Update()
    {
        base.Update();
    }

    public override void ReachedZeroHitpoints()
    {
        purifycationHandler.healthAmountRestoredOnPurify = healthAmountRestoredOnPurify;
        purifycationHandler.SetStunned(this);
    }
    private void Shoot() 
    {
        bool needMoreProjectiles = true;
        foreach(PusherProjectile shot in projectiles) 
        {
            if (!shot.gameObject.activeInHierarchy) 
            {
                needMoreProjectiles = false;
                shot.transform.position = transform.position;
                shot.Init(_Player, _PushStrength, _Damage);
                float rotationAngle = transform.eulerAngles.z;
                shot.SetVelocity(RotateVector2(lookDirection, rotationAngle), projectileSpeed);
                shot.gameObject.SetActive(true);
                break;
                
            }
        
        }
        if (needMoreProjectiles) 
        {
            PusherProjectile shot = Instantiate(_ProjectilePrefab,transform);
            shot.Init(_Player, _PushStrength,_Damage);
            shot.gameObject.SetActive(true);
            projectiles.Add(shot);

        }
    }

    private Vector2 RotateVector2(Vector2 vector,float delta)
    {
        delta *= Mathf.Deg2Rad;
        return new Vector2(
            vector.x * Mathf.Cos(delta) - vector.y * Mathf.Sin(delta),
            vector.x * Mathf.Sin(delta) + vector.y * Mathf.Cos(delta)
    );
    }
}

