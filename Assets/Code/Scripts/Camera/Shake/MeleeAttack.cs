using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    public float attackRange = 1.5f; // Reichweite des Angriffs
    public int attackDamage = 10; // Schaden des Angriffs
    public LayerMask enemyLayer; // Layer, der für Gegner verwendet wird

    void Update()
    {
        // Angriff auslösen mit X-Button des Controllers (Standard Unity Input: "Fire1")
        if (Input.GetButtonDown("Fire1"))
        {
            PerformAttack();
        }
    }

    void PerformAttack()
    {
        // Überlappende Gegner in der Angriffssphäre finden
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position + transform.forward, attackRange, enemyLayer);

        foreach (Collider enemy in hitEnemies)
        {
            // Gegner Schaden zufügen
            if (enemy.TryGetComponent(out EnemyHealth enemyHealth))
            {
                enemyHealth.TakeDamage(attackDamage);
                Debug.Log($"Getroffen: {enemy.name} für {attackDamage} Schaden");
            }
        }
    }

    // Debugging: Zeigt die Angriffssphäre in der Szene
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward, attackRange);
    }
}