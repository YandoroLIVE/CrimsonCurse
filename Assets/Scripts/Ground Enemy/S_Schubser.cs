using System.Collections;
using UnityEngine;

public class S_Schubser : MonoBehaviour
{
    public float movementSpeed = 2f;
    public float attackSpeed = 1f; // Zeitintervall zwischen Angriffen
    public float defenceSpeed = 1f; // Geschwindigkeit beim Zurückkehren in den Idle-Zustand
    public float idleTime = 2f; // Zeitverzögerung, bevor der Schubser aus dem Idle-Zustand wieder aktiv wird
    public float detectionRadius = 5f; // Radius, in dem der Spieler erkannt wird
    public float attackRange = 3f; // Reichweite des Angriffs
    public float knockbackForce = 5f; // Rückstoßkraft
    public int damage = 10; // Schaden des Angriffs
    public int maxHP = 3; // HP des Gegners
    public float approachDistance = 2f; // Distanz, die der Schubser vom Spawnpunkt in Richtung des Spielers zurücklegt

    private int currentHP;
    private Transform player;
    private Vector2 spawnPoint;
    private bool isIdle = true;
    private bool isDefeated = false;
    private bool isReturningToIdle = false;
    private float nextAttackTime;
    private float idleTimer;

    private void Start()
    {
        Debug.Log("Schubser ist gestartet"); // Debug-Log zur Überprüfung des Starts
        currentHP = maxHP;
        player = GameObject.FindWithTag("Player").transform; // Spieler muss das Tag "Player" haben

        // Setze Rigidbody2D auf Kinematic, um den Schubser statisch zu machen
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 0;
            rb.isKinematic = true;
        }

        // Speichert den Spawnpunkt des Schubsers
        spawnPoint = transform.position;
        idleTimer = idleTime; // Setze den Timer für den Idle-Zustand
    }

    private void Update()
    {
        if (isDefeated) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (isIdle)
        {
            // Spieler innerhalb des Erkennungsradius
            if (distanceToPlayer <= detectionRadius && idleTimer <= 0)
            {
                isIdle = false; // Bewegt sich kurz in Richtung des Spielers
            }
            else
            {
                idleTimer -= Time.deltaTime;
            }
        }
        else
        {
            if (distanceToPlayer <= attackRange && Time.time >= nextAttackTime)
            {
                Attack();
            }
            else
            {
                MoveTowardsPlayer();
            }
        }
    }

    private void MoveTowardsPlayer()
    {
        if (Vector2.Distance(transform.position, spawnPoint) < approachDistance)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            transform.position = Vector2.MoveTowards(transform.position, transform.position + (Vector3)direction, movementSpeed * Time.deltaTime);
        }
    }

    private void Attack()
    {
        nextAttackTime = Time.time + attackSpeed;

        // Berechne den Rückstoß
        Vector2 knockbackDirection = (player.position - transform.position).normalized;
        Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();

        if (playerRb != null)
        {
            // Rückstoßkraft auf den Spieler anwenden
            playerRb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
        }

        // Schaden am Spieler zufügen
        S_PlayerHealth playerHealth = player.GetComponent<S_PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }
    }

    public void TakeDamage(int amount)
    {
        if (isDefeated) return;

        currentHP -= amount;

        // Ausgabe des Schadens in der Konsole
        Debug.Log($"Schubser hat Schaden genommen: {amount} Schaden. Verbleibende HP: {currentHP}");

        if (currentHP <= 0)
        {
            isDefeated = true;
            StartCoroutine(DefeatedState());
        }
    }

    private IEnumerator DefeatedState()
    {
        // Zurück in den Idle-Zustand bewegen
        isReturningToIdle = true;
        while (Vector2.Distance(transform.position, spawnPoint) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, spawnPoint, defenceSpeed * Time.deltaTime);
            yield return null;
        }
        isReturningToIdle = false;

        // Bleibt für den eingestellten Idle-Intervall im Idle-Zustand
        idleTimer = idleTime;
        isIdle = true;
    }

    private void OnDrawGizmosSelected()
    {
        // Erkennungsradius visualisieren
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Angriffsradius visualisieren
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Ansatzdistanz visualisieren
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(spawnPoint, approachDistance);
    }
}
