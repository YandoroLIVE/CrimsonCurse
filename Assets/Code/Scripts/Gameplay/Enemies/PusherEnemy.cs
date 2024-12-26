using UnityEngine;
using System.Collections;
using NUnit.Framework;
using System.Collections.Generic;


public class PusherEnemy : BaseEnemy
{
    private const int PUSH_AMOUNT = 20;
    [SerializeField] ParticleSystem _AttackVFX;
    [SerializeField] Vector2 _PushDirection;
    [SerializeField] float _PushStrength;
    [SerializeField] float _PushDelay;
    [SerializeField] float _Damage;
    [SerializeField] EnemyPurify purifycationHandler;
    [SerializeField] IsPlayerInTrigger _AttackTrigger;
    private bool playerinRange = false;
    private (Rigidbody2D rigidbody2D,S_PlayerHealth health) _Player = (null,null);
    public float attackCooldown;
    private float timer;
    //private float standardDrag;
    private void Push(Rigidbody2D target)
    {
        target.linearDamping = 0;
        Vector2 pushforce = (_PushDirection.normalized * _PushStrength * Time.fixedDeltaTime);
        //Debug.Log(pushforce);
        target.AddForce(pushforce);
        
    }


    private void Playeffects()
    {
        _AttackVFX.Play();
        // play shooting animation
    }

    IEnumerator DelayPush()
    {
        for (int i = 0; i < PUSH_AMOUNT; i++) { 
            yield return new WaitForSeconds(_PushDelay);
            Push(_Player.rigidbody2D);
        }

        _Player.health.TakeDamage(((int)_Damage));
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

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    //Debug.Log("Collision");
    //    if (_Player.rigidbody2D == null || _Player.health == null)
    //    {

    //        _Player.rigidbody2D = collision.gameObject.GetComponent<Rigidbody2D>();
    //        _Player.health = collision.gameObject.GetComponent<S_PlayerHealth>();
    //        //standardDrag = _Player.linearDamping;
    //    }

    //    playerinRange = true;
        
    //}
    //public void OnTriggerExit2D(Collider2D collision)
    //{
    //    playerinRange = false;
    //}

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
                Debug.Log(collision);
                _Player.rigidbody2D = collision.gameObject.GetComponent<Rigidbody2D>();
                _Player.health = collision.gameObject.GetComponent<S_PlayerHealth>();
                //standardDrag = _Player.linearDamping;
            }
            if (timer+attackCooldown <= Time.time) 
            {
                timer = Time.time;
                Playeffects();
                StartCoroutine(DelayPush());
            }
        }
    }

    public override void Update()
    {
        base.Update();
    }

    public override void ReachedZeroHitpoints()
    {
        purifycationHandler.SetStunned(this);
    }
}
