using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WingAttack : MonoBehaviour
{
    [SerializeField] GameObject warnZone;

    [SerializeField] PusherProjectile _ProjectilePrefab;
    public float projectileSpeed = 10f;
    private Vector2 lookDirection = new Vector2(0, -1); //Default position

    private List<PusherProjectile> projectiles = new List<PusherProjectile>();
    public float _Damage;
    public float _Pushstrength;
    public float warnduration;
    public float attackCooldown;
    private float attackTimer = 0;
    private bool canAttack;
    public float attackInterval;
    public float attackDuration;
    private (Rigidbody2D rigidbody2D, S_PlayerHealth health) _Player;
    
    public void SetPlayer((Rigidbody2D, S_PlayerHealth) player) 
    {
        _Player = player;
    }

    private bool CanAttack() 
    { 
        if(Time.time >= attackTimer) 
        {
            canAttack = true;
            attackTimer = Time.time +attackCooldown;
            
        }
        return canAttack;
    }

    private void Awake()
    {
        warnZone.SetActive(false);
    }
    

    public void Cycle(float delta, float timer)
    {
        if (warnZone != null && timer >= attackInterval - warnduration && timer <= attackInterval)
        {
            warnZone.SetActive(true);
        }
        if (timer >= attackInterval && timer <= attackInterval+attackDuration && CanAttack())
        {
            canAttack = false;
            Shoot();
            warnZone.SetActive(false);
        }
    }
    public void OnEnable()
    {
        warnZone.SetActive(false);
        attackTimer = 0;
    }

    public void Shoot() 
    {
        bool needMoreProjectiles = true;
        foreach (PusherProjectile shot in projectiles)
        {
            if (!shot.gameObject.activeInHierarchy)
            {
                needMoreProjectiles = false;
                shot.transform.position = transform.position;
                shot.Init(_Player, _Pushstrength, _Damage);
                float rotationAngle = transform.eulerAngles.z;
                shot.SetVelocity(RotateVector2(lookDirection, rotationAngle), projectileSpeed);
                shot.gameObject.SetActive(true);
                break;

            }

        }
        if (needMoreProjectiles)
        {
            PusherProjectile shot = Instantiate(_ProjectilePrefab, transform);
            shot.Init(_Player, _Pushstrength, _Damage);
            shot.gameObject.SetActive(true);
            projectiles.Add(shot);

        }
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
