using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    // Funktion, um dem Spieler Schaden zuzufügen
    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        Debug.Log("Spieler nimmt " + damageAmount + " Schaden. Aktuelle Gesundheit: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Spieler stirbt
    void Die()
    {
        Debug.Log("Spieler ist gestorben.");
        // Hier kannst du das Verhalten definieren, wenn der Spieler stirbt
    }
}
