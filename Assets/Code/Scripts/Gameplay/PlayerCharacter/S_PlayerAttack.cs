
using UnityEngine;


public class S_PlayerAttack : MonoBehaviour
{
    private const float VFX_SCALE = 1.75f;
    public int damage = 25;
    public float attackRange = 1.5f;
    public float attackCooldown = 0.5f;
    public float attackMinCooldownBeforeComboAttack = 0.1f;
    public float attackComboTime = 0.25f;
    private float attackComboTimer = 0;
    public LayerMask enemyLayer;
    public ParticleSystem attackVFX;
    private float attackTimer = 0;
    private Collider2D[] enemiesInRange;
    private bool comboActive = false;
    private Vector2 attackPos = Vector2.zero;
    [SerializeField] Animator animator;
    [SerializeField] AudioClip[] attackClip;
    [SerializeField] AudioClip[] impactSFX;
    private void Start()
    {
        attackTimer = attackCooldown;

    }

    void Update()
    {
        attackTimer += Time.deltaTime;
        if (comboActive)
        {
            attackComboTimer += Time.deltaTime;
        }
        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.JoystickButton2))
        {
            if (attackTimer >= attackCooldown)
            {
                attackVFX.transform.localScale = Vector3.one* attackRange * VFX_SCALE;
                comboActive = true;
                attackComboTimer = 0;
                attackTimer = 0;
                Attack();
            }
            else if (attackTimer <= attackComboTime+ attackMinCooldownBeforeComboAttack && attackTimer >= attackMinCooldownBeforeComboAttack && comboActive)
            {
                Vector3 secondComboScale = Vector3.one* VFX_SCALE * attackRange;
                secondComboScale.y = secondComboScale.y * -1;
                attackVFX.transform.localScale = secondComboScale;
                Attack();
                attackComboTimer = 0;
                comboActive = false;
            }
        }
    }

    private void CheckForEnemiesInRange()
    {
        attackPos = transform.position;
        attackPos.x = attackPos.x + (attackRange / 2 * transform.localScale.x)* VFX_SCALE;
        attackVFX.transform.position = attackPos;
        enemiesInRange = Physics2D.OverlapCircleAll(attackPos, attackRange, enemyLayer);
    }
    public float GetCooldownPercentage()
    {
        return Mathf.Clamp01(attackTimer / attackCooldown);
    }



    // Angriffs-Methode
    void Attack()
    {
        animator.SetTrigger("MeleeAttack");
        CheckForEnemiesInRange();
        attackVFX.Play();
        //AudioManager.instance.PlayRandomSoundFXClip(attackClip, transform, 1f);
        foreach (var enemyCollider in enemiesInRange)
        {
            IHurtable enemy = enemyCollider.GetComponent<IHurtable>();

            if (enemy != null)
            {
                enemy.Hurt(damage);
                //AudioManager.instance.PlayRandomSoundFXClip(impactSFX, transform, 1f);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos, attackRange);
    }
}
