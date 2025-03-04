using System.Collections.Generic;
using UnityEngine;

public class S_PlayerLongRangeAttack : MonoBehaviour
{
    [SerializeField] private S_Projectile projectilePrefab;
    [SerializeField] Vector3 firePoint;
    [SerializeField] Animator animator;
    public float projectileSpeed = 10f;
    public float projectileLifeTime = 10f;
    public int damage = 100;
    public float attackCooldown = 0.5f;
    private float attackTimer;
    List<S_Projectile> projectiles = new List<S_Projectile>();
    [SerializeField] AudioClip longrangesfx;

    void Update()
    {
        attackTimer += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.JoystickButton3))
        {
            if (attackTimer >= attackCooldown)
            {
                attackTimer = 0;
                LongRangeAttack();
                AudioManager.instance?.PlaySoundFXClip(longrangesfx, transform, 1f);
            }
        }
    }

    void LongRangeAttack()
    {
        animator.SetTrigger("RangedAttack");
        bool needMoreProjectiles = true;
        foreach (S_Projectile shot in projectiles)
        {
            if (!shot.gameObject.activeInHierarchy)
            {
                needMoreProjectiles = false;
                ResetShot(shot);
                break;

            }

        }
        if (needMoreProjectiles)
        {
            S_Projectile shot = Instantiate(projectilePrefab);
            ResetShot(shot);
            projectiles.Add(shot);

        }
    }

    private void ResetShot(S_Projectile shot)
    {
        shot.Init(damage, projectileLifeTime);
        shot.transform.position = this.transform.position + (firePoint * Mathf.Sign(this.transform.localScale.x));
        shot.gameObject.SetActive(true);
        shot.rigidBody2D.linearVelocityX = projectileSpeed * Mathf.Sign(this.transform.localScale.x);
    }

    public float GetCooldownPercentage()
    {
        return Mathf.Clamp01(attackTimer / attackCooldown);
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(this.transform.position + (firePoint * Mathf.Sign(this.transform.localScale.x)), 0.125f);
    }
}
