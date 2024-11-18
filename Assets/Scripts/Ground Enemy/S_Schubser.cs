using System.Collections;
using UnityEngine;

public class S_Schubser : MonoBehaviour
{
    public float attackSpeed = 1f; // Zeitintervall zwischen Angriffen
    public float knockbackForce = 5f; // R�cksto�kraft
    public int damage = 10; // Schaden des Angriffs
    public int maxHP = 3; // HP des Gegners
    public float detectionRadius = 5f; // Radius, in dem der Spieler erkannt wird
    public float attackRange = 3f; // Reichweite des Angriffs

    private int currentHP;
    private Transform player;
    private bool isIdle = true;
    private bool isDefeated = false;
    private float nextAttackTime;

    private void Start()
    {
        Debug.Log("Schubser ist gestartet"); // Debug-Log zur �berpr�fung des Starts
        currentHP = maxHP;
        player = GameObject.FindWithTag("Player").transform; // Spieler muss das Tag "Player" haben

        // Setze Rigidbody2D auf Kinematic, um den Schubser statisch zu machen
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 0;
            rb.isKinematic = true; // Verhindert, dass der Schubser sich bewegt
        }
    }

    private void Update()
    {
        if (isDefeated) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (isIdle)
        {
            // Spieler innerhalb des Erkennungsradius
            if (distanceToPlayer <= detectionRadius)
            {
                isIdle = false; // Angriff vorbereiten, aber der Schubser bewegt sich nicht
            }
        }
        else
        {
            if (distanceToPlayer <= attackRange && Time.time >= nextAttackTime)
            {
                Attack();
            }
        }
    }

    private void Attack()
    {
        nextAttackTime = Time.time + attackSpeed;

        // Schaden am Spieler zuf�gen
        S_PlayerHealth playerHealth = player.GetComponent<S_PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }

        // R�cksto� auf den Spieler anwenden
        ApplyKnockback();
    }

    // Diese Methode k�mmert sich um den R�cksto�
    private void ApplyKnockback()
    {
        Vector2 knockbackDirection = (player.position - transform.position).normalized; // Richtung vom Schubser zum Spieler
        Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();

        if (playerRb != null)
        {
            // R�cksto�kraft auf den Spieler anwenden
            playerRb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
            Debug.Log("Knockback angewendet: " + knockbackDirection * knockbackForce);
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
        // Der Schubser bleibt an seinem Platz, auch wenn er besiegt ist
        yield return null;

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
    }
}
