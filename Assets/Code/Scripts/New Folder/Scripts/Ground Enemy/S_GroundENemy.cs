using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_GroundEnemy : MonoBehaviour
{
    public float speed = 2f; // Geschwindigkeit des Gegners
    public int maxHealth = 100;
    public int currentHealth;
    public int damageToPlayer = 20; // Schaden, den der Gegner dem Spieler zuf�gt
    public float detectionRadius = 5f; // Radius, in dem der Gegner den Spieler erkennt
    public float attackRadius = 1.5f; // Radius, in dem der Gegner Schaden verursacht

    private Transform target; // Das Ziel, normalerweise der Spieler
    private float attackCooldown = 1f; // Zeit zwischen den Angriffen
    private float lastAttackTime;

    void Start()
    {
        currentHealth = maxHealth;
        target = GameObject.FindGameObjectWithTag("Player").transform; // Finde den Spieler per Tag
    }

    void Update()
    {
        // Pr�fe, ob der Spieler innerhalb des Erkennungsradius ist
        if (target != null && Vector3.Distance(transform.position, target.position) <= detectionRadius)
        {
            // Pr�fe, ob der Gegner nah genug am Spieler ist, um Schaden zu verursachen
            if (Vector3.Distance(transform.position, target.position) > attackRadius)
            {
                //MoveTowardsPlayer(); // Gegner bewegt sich in Richtung des Spielers (Auskommentiert)
            }
            else
            {
                AttackPlayer(); // Gegner greift den Spieler an
            }
        }
    }

    // Funktion zum Zuf�gen von Schaden
    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        Debug.Log("Gegner nimmt " + damageAmount + " Schaden.");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Angreifen des Spielers
    void AttackPlayer()
    {
        Debug.Log("Greife den Spieler an"); // Debug-Log f�r Angriff
        // �berpr�fen, ob der Gegner bereit ist anzugreifen (Cooldown)
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            // Zugriff auf das PlayerHealth-Skript des Spielers und Schaden zuf�gen
            S_PlayerHealth playerHealth = target.GetComponent<S_PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageToPlayer);
                Debug.Log("Gegner f�gt dem Spieler " + damageToPlayer + " Schaden zu.");
            }

            lastAttackTime = Time.time; // Zeit des letzten Angriffs aktualisieren
        }
    }

    // Gegner stirbt
    void Die()
    {
        Debug.Log("Gegner ist gestorben.");

        // Spieler heilt sich beim T�ten des Gegners
        S_PlayerHealth playerHealth = target.GetComponent<S_PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.RegenerateHealthOnKill();
        }

        Destroy(gameObject); // Entferne den Gegner aus der Szene
    }
}
