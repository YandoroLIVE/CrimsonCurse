
using UnityEngine;


public class S_PlayerAttack : MonoBehaviour
{
    public int damage = 25;
    public float attackRange = 1.5f;
    public float attackHeight = 1.5f;
    public float attackCooldown = 0.5f;
    public float attackMinCooldownBeforeComboAttack = 0.1f;
    public float attackComboTime = 0.25f;
    private float attackComboTimer = 0;
    public LayerMask enemyLayer;
    public ParticleSystem attackDamage;
    private float attackTimer = 0;
    private Collider2D[] enemiesInRange;
    private bool comboActive = false;
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
                attackDamage.transform.localScale = Vector3.one* attackHeight;
                comboActive = true;
                attackComboTimer = 0;
                attackTimer = 0;
                Attack();
            }
            else if (attackTimer <= attackComboTime+ attackMinCooldownBeforeComboAttack && attackTimer >= attackMinCooldownBeforeComboAttack && comboActive)
            {
                Vector3 secondComboScale = Vector3.one*1* attackHeight;
                secondComboScale.y = secondComboScale.y * -1;
                attackDamage.transform.localScale = secondComboScale;
                Attack();
                attackComboTimer = 0;
                comboActive = false;
                Debug.Log("Did combo attack");
            }
        }
    }

    private void CheckForEnemiesInRange()
    {
        Vector2 pos = transform.position;
        pos.x = pos.x + (attackRange / 2 * transform.localScale.x);
        enemiesInRange = Physics2D.OverlapBoxAll(pos, new Vector2(attackRange, attackHeight), 0, enemyLayer);
    }


    // Angriffs-Methode
    void Attack()
    {
        attackDamage.Play();
        CheckForEnemiesInRange();
        foreach (var enemyCollider in enemiesInRange)
        {
            IHurtable enemy = enemyCollider.GetComponent<IHurtable>();

            if (enemy != null)
            {
                enemy.Hurt(damage);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector2 pos = transform.position;
        pos.x = pos.x + (attackRange / 2 * transform.localScale.x);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(pos, new Vector2(attackRange, attackHeight));
    }
}
