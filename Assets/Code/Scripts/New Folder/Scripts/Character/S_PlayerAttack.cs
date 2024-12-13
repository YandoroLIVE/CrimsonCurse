using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class S_PlayerAttack : MonoBehaviour
{
    public int damage = 25; // Schaden, den der Spieler anrichtet
    public float attackRange = 1.5f; // Reichweite des Angriffs
    public float attackHeight = 1.5f;
    public LayerMask enemyLayer; // Layer f�r den Gegner
    private Collider2D[] enemiesInRange; // Array f�r Gegner im Angriffsbereich
    public ParticleSystem attackDamage;
    void Update()
    {
        // �berpr�fen, ob der Spieler die Angriffs-Taste dr�ckt (hier auf die Taste "E" festgelegt)
        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.JoystickButton2))
        {
            Attack(); // Spieler greift an
        }

        // �berpr�fe, ob Gegner im Angriffsbereich sind
        CheckForEnemiesInRange();
    }

    // Methode zur �berpr�fung von Gegnern im Angriffsbereich
    private void CheckForEnemiesInRange()
    {
        // Alle Gegner im Bereich abfragen
        //enemiesInRange = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayer);
        Vector2 pos = transform.position;
        pos.x = pos.x + (attackRange / 2 * transform.localScale.x);
        enemiesInRange = Physics2D.OverlapBoxAll(pos, new Vector2(attackRange, attackHeight), 0, enemyLayer);
    }


    // Angriffs-Methode
    void Attack()
    {
        Debug.Log("Angriff ausgel�st!");
        attackDamage.Play();
        // Gehe durch alle Gegner im Bereich und f�ge Schaden zu
        foreach (var enemyCollider in enemiesInRange)
        {
            S_GroundEnemy enemy = enemyCollider.GetComponent<S_GroundEnemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Debug.Log("Gegner nimmt " + damage + " Schaden.");
                DisplayDamage(damage); // Zeigt den verursachten Schaden an
            }
        }
    }

    // Funktion zur Anzeige des verursachten Schadens
    void DisplayDamage(int damageAmount)
    {
        Debug.Log("Du hast " + damageAmount + " Schaden verursacht!");
    }

    // Optional: Zeichne den Angriffsbereich zur Visualisierung
    private void OnDrawGizmosSelected()
    {
        Vector2 pos = transform.position;
        pos.x = pos.x + (attackRange / 2 * transform.localScale.x);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(pos, new Vector2(attackRange, attackHeight));
    }
}
