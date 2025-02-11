using UnityEngine;
using System.Collections;
using NUnit.Framework;
using System.Collections.Generic;

[System.Serializable]
public class PusherEnemy : BaseEnemy
{
    private const float HIT_BLINK_DURATION = 0.1f;
    [SerializeField] float _PushStrength;
    [SerializeField] float _Damage;
    public float attackCooldown;
    public float projectileSpeed;
    [SerializeField] EnemyPurify purifycationHandler;
    [SerializeField] Animator animator;
    [SerializeField] Color hitColor;
    [SerializeField] SpriteRenderer _sprite;
    [SerializeField] IsPlayerInTrigger _AttackTrigger;
    [SerializeField] PusherProjectile _ProjectilePrefab;
    [SerializeField] GameObject _NoPassCollider;
    [SerializeField] GameObject _PurifiedPusherPrefab;
    [SerializeField] AudioClip[] hitSFX;
    [SerializeField] AudioClip attackSFX;
    private List<PusherProjectile> projectiles = new List<PusherProjectile>();
    private (Rigidbody2D rigidbody2D, S_PlayerHealth health) _Player = (null, null);
    private float timer;
    private Vector2 lookDirection = new Vector2(0, -1); //Default position pusher looks at





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
            if (timer + attackCooldown <= Time.time)
            {
                timer = Time.time;
                Shoot();
                animator.SetBool("Attacking", true);
            }
        }
        else
        {
            animator.SetBool("Attacking", false);
        }
    }

    public override void Update()
    {
        base.Update();
    }

    public override void ReachedZeroHitpoints()
    {
        animator.SetBool("Attacking", false);
        purifycationHandler.healthAmountRestoredOnPurify = healthAmountRestoredOnPurify;
        purifycationHandler.SetStunned(this);
    }

    public override void Hurt(float damage)
    {
        base.Hurt(damage);
        StopCoroutine(HitFeedBack());
        _sprite.color = Color.white;
        StartCoroutine(HitFeedBack());
        animator.SetTrigger("Hurt");
        animator.SetBool("Attacking", false);

    }


    IEnumerator HitFeedBack()
    {
        if (_sprite != null)
        {
            _sprite.color = hitColor;
            AudioManager.instance?.PlayRandomSoundFXClip(hitSFX, transform, 1f);
            yield return new WaitForSeconds(HIT_BLINK_DURATION);
            _sprite.color = Color.white;
        }
    }

    private void Shoot()
    {
        bool needMoreProjectiles = true;
        foreach (PusherProjectile shot in projectiles)
        {
            if (!shot.gameObject.activeInHierarchy)
            {
                AudioManager.instance?.PlaySoundFXClip(attackSFX, transform, 1f);
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
            PusherProjectile shot = Instantiate(_ProjectilePrefab, transform);
            shot.Init(_Player, _PushStrength, _Damage);
            shot.gameObject.SetActive(true);
            projectiles.Add(shot);

        }
    }
    public override void OnPurify()
    {
        _NoPassCollider.SetActive(false);
        animator.SetBool("Purify", true);

    }


    public override void AfterPurify()
    {
        foreach (PusherProjectile shot in projectiles)
        {
            Destroy(shot);
        }
        projectiles.Clear();
        
        var tmp = Instantiate(_PurifiedPusherPrefab, animator.transform.position, this.transform.rotation, null);
        bool idleOne = Random.value >= 0.5f ? true : false;
        tmp.GetComponent<Animator>().SetBool("IdleOne", idleOne);
    }


    private Vector2 RotateVector2(Vector2 vector, float delta)
    {
        delta *= Mathf.Deg2Rad;
        return new Vector2(
            vector.x * Mathf.Cos(delta) - vector.y * Mathf.Sin(delta),
            vector.x * Mathf.Sin(delta) + vector.y * Mathf.Cos(delta)
    );
    }
}

