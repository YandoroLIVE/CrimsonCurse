using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    public float attackRange = 1.5f; // Reichweite des Angriffs
    public int attackDamage = 10; // Schaden des Angriffs
    public LayerMask enemyLayer; // Layer, der f�r Gegner verwendet wird

    void Update()
    {
        // Angriff ausl�sen mit X-Button des Controllers (Standard Unity Input: "Fire1")
        if (Input.GetButtonDown("Fire1"))
        {
            PerformAttack();
        }
    }

    void PerformAttack()
    {
        // �berlappende Gegner in der Angriffssph�re finden
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position + transform.forward, attackRange, enemyLayer);

        foreach (Collider enemy in hitEnemies)
        {
            // Gegner Schaden zuf�gen
            if (enemy.TryGetComponent(out EnemyHealth enemyHealth))
            {
                enemyHealth.TakeDamage(attackDamage);
                Debug.Log($"Getroffen: {enemy.name} f�r {attackDamage} Schaden");
            }
        }
    }

    // Debugging: Zeigt die Angriffssph�re in der Szene
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward, attackRange);
    }
}