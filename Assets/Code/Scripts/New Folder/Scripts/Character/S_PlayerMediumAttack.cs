using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_PlayerMediumAttack : MonoBehaviour
{
    public int damage = 40; // Schaden für den Medium Attack
    public float attackRange = 3f; // Reichweite für den Medium Attack
    public float attackHeight = 2f;
    public LayerMask enemyLayer; // Layer für den Gegner
    private Collider2D[] enemiesInRange; // Array für Gegner im Angriffsbereich
    public ParticleSystem attackDamage;

    void Update()
    {
        // Überprüfen, ob der Spieler die Medium-Angriffs-Taste drückt (hier auf "Q" festgelegt)
        if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.JoystickButton1))
        {
            MediumAttack(); // Spieler führt den Medium-Angriff aus
        }

        // Überprüfen, ob Gegner im Angriffsbereich sind
        CheckForEnemiesInRange();
    }

    // Methode zur Überprüfung von Gegnern im Angriffsbereich
    private void CheckForEnemiesInRange()
    {
        // Alle Gegner im Bereich abfragen
        Vector2 pos = transform.position;
        pos.x = pos.x + (attackRange / 2 * transform.localScale.x);
        enemiesInRange = Physics2D.OverlapBoxAll(pos, new Vector2(attackRange,attackHeight),0, enemyLayer);

        if (enemiesInRange.Length > 0)
        {
            Debug.Log("Gegner im Angriffsbereich für Medium-Angriff erkannt!");
        }
        else
        {
            Debug.Log("Kein Gegner im Angriffsbereich für Medium-Angriff.");
        }
    }

    // Medium-Angriff
    void MediumAttack()
    {
        Debug.Log("Medium-Angriff ausgelöst!");
        attackDamage.Play();
        // Gehe durch alle Gegner im Bereich und füge Schaden zu
        foreach (var enemyCollider in enemiesInRange)
        {
            S_GroundEnemy enemy = enemyCollider.GetComponent<S_GroundEnemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Debug.Log("Gegner nimmt " + damage + " Schaden im Medium-Angriff.");
                DisplayDamage(damage); // Zeigt den verursachten Schaden an
            }
        }
    }

    // Funktion zur Anzeige des verursachten Schadens
    void DisplayDamage(int damageAmount)
    {
        Debug.Log("Du hast " + damageAmount + " Schaden im Medium-Angriff verursacht!");
    }

    // Optional: Zeichne den Angriffsbereich zur Visualisierung
    private void OnDrawGizmosSelected()
    {
        Vector2 pos = transform.position;
        pos.x = pos.x + (attackRange / 2 * transform.localScale.x);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(pos, new Vector2(attackRange,attackHeight));
    }
}
