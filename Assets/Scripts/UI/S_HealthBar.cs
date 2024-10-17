using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public S_PlayerHealth playerHealth;  // Referenz zum PlayerHealth
    public S_GroundEnemy enemyHealth;    // Referenz zum EnemyHealth

    public Slider playerHealthBar;        // Slider für den Spieler
    public Slider enemyHealthBar;         // Slider für den Gegner

    void Start()
    {
        // Überprüfen, ob die Referenzen gesetzt sind
        if (playerHealth == null)
        {
            Debug.LogError("PlayerHealth ist nicht zugewiesen! Bitte im Inspektor zuweisen.");
        }
        else
        {
            playerHealthBar.maxValue = playerHealth.maxHealth;
            playerHealthBar.value = playerHealth.currentHealth;
        }

        if (enemyHealth == null)
        {
            Debug.LogError("EnemyHealth ist nicht zugewiesen! Bitte im Inspektor zuweisen.");
        }
        else
        {
            enemyHealthBar.maxValue = enemyHealth.maxHealth;
            enemyHealthBar.value = enemyHealth.currentHealth;
        }
    }

    void Update()
    {
        UpdateHealthBars();
    }

    void UpdateHealthBars()
    {
        if (playerHealth != null)
        {
            playerHealthBar.value = playerHealth.currentHealth;  // Aktuellen Wert setzen
        }

        if (enemyHealth != null)
        {
            enemyHealthBar.value = enemyHealth.currentHealth;  // Aktuellen Wert setzen
        }
    }
}
