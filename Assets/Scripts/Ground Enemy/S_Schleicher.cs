using System.Collections;
using UnityEngine;

public class S_Schleicher : MonoBehaviour
{
    [Header("Erkennung")]
    public float detectionRadius = 5f; // Radius, in dem der Player erkannt wird
    public LayerMask playerLayer; // Layer des Players

    [Header("Bewegung")]
    public float moveSpeed = 2f; // Geschwindigkeit des Schleicher
    public float idleSpeed = 1f; // Geschwindigkeit für die Spiralförmige Bewegung

    [Header("Angriff")]
    public int attackDamage = 10; // Schaden pro Angriff
    public float attackSpeed = 1f; // Zeit zwischen Angriffen in Sekunden

    [Header("Schleicher Status")]
    public float maxHP = 50f; // Maximale HP des Schleicher
    private float currentHP;

    [Header("Referenzen")]
    public Transform playerTransform; // Referenz auf den Player (automatisch erkannt)

    private bool isPlayerInRange = false; // Ob der Player im Radius ist
    private bool isAttacking = false; // Ob der Schleicher gerade angreift
    private bool isIdle = true; // Zustand des Schleichers (Idle oder Angreifen)

    private Vector2 idleStartPos; // Startposition der Idle-Phase
    private float angle = 0f; // Winkel für die spiralförmige Bewegung
    private float maxIdleHeight = 2f; // Maximale Höhe der spiralförmigen Bewegung

    public LayerMask wallLayer; // Layer für Wände

    private void Start()
    {
        currentHP = maxHP; // Setze die aktuellen HP auf die maximale HP beim Start
        idleStartPos = transform.position; // Speichere die Startposition für die Idle-Phase
    }

    private void Update()
    {
        DetectPlayer();
        if (isIdle)
        {
            MoveInIdleState(); // Spiralförmige Bewegung im Idle-Zustand
        }
        else
        {
            MoveTowardsPlayer(); // Bewegung auf den Spieler zu
        }
    }

    private void DetectPlayer()
    {
        // Überprüft, ob der Spieler im Erkennungsradius ist
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius, playerLayer);
        if (hits.Length > 0)
        {
            playerTransform = hits[0].transform;
            isPlayerInRange = true;

            // Bestimmt die Bewegungsrichtung des Spielers basierend auf den Eingaben
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            Vector2 movementDirection = new Vector2(horizontal, vertical).normalized;

            // Berechnet die Richtung zum Schleicher
            Vector2 directionToEnemy = (transform.position - playerTransform.position).normalized;

            // Berechnet den Winkel zwischen der Bewegungsrichtung des Spielers und der Richtung zum Schleicher
            float angleBetween = Vector2.Angle(movementDirection, directionToEnemy);

            // Wenn der Winkel kleiner als ein bestimmter Schwellenwert ist, schaut der Spieler auf den Schleicher
            if (angleBetween < 45f)
            {
                isIdle = true; // Spieler schaut in Richtung des Schleichers
                if (isAttacking)
                {
                    StopCoroutine(ContinuousAttack()); // Stoppe den kontinuierlichen Angriff
                    isAttacking = false;
                }
            }
            else
            {
                isIdle = false; // Spieler schaut weg, Schleicher greift an
                if (!isAttacking)
                {
                    StartCoroutine(ContinuousAttack()); // Starte kontinuierlichen Angriff
                }
            }
        }
        else
        {
            isPlayerInRange = false;
            isIdle = true; // Spieler ist nicht im Radius, zurück in Idle-Position
            if (isAttacking)
            {
                StopCoroutine(ContinuousAttack()); // Stoppe den kontinuierlichen Angriff, wenn der Spieler rausgeht
                isAttacking = false;
            }
        }
    }

    private void MoveInIdleState()
    {
        // Spiralförmige Bewegung im Idle-Zustand
        angle += idleSpeed * Time.deltaTime; // Erhöht den Winkel für die Spirale
        float x = idleStartPos.x + Mathf.Cos(angle) * 2f; // X-Position entlang der Spirale
        float y = idleStartPos.y + Mathf.Sin(angle) * 0.5f + Mathf.PingPong(angle * 0.2f, maxIdleHeight); // Y-Position mit schwingender Bewegung

        transform.position = new Vector2(x, y); // Setze neue Position des Schleichers
    }

    private void MoveTowardsPlayer()
    {
        if (playerTransform == null) return;

        // Bewegung in Richtung Spieler
        Vector2 direction = (playerTransform.position - transform.position).normalized;

        // Prüfen, ob ein Hindernis (Wand) im Weg ist
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 1f, wallLayer);
        if (hit.collider == null) // Keine Wand im Weg
        {
            transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;
        }
        else
        {
            // Bewegung stoppen oder eine alternative Bewegung implementieren
            Debug.Log("Wall detected! Stopping movement.");
        }

        // Debugging: Raycast anzeigen
        Debug.DrawRay(transform.position, direction, Color.red, 0.1f);
    }

    private IEnumerator ContinuousAttack()
    {
        isAttacking = true;
        while (!isIdle) // Solange der Spieler den Schleicher nicht anschaut, greife an
        {
            if (playerTransform != null)
            {
                // Schaden dem Spieler zufügen (Korrekte Referenzierung zur S_PlayerHealth-Datei)
                S_PlayerHealth playerHealth = playerTransform.GetComponent<S_PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(attackDamage); // Übergibt int-Schaden
                }
            }
            yield return new WaitForSeconds(attackSpeed); // Warte die festgelegte Zeit und füge dann erneut Schaden zu
        }
        isAttacking = false;
    }

    public void TakeDamage(float damage)
    {
        currentHP -= damage;

        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Entferne den Schleicher aus der Szene
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        // Zeichnet den Erkennungsradius in der Szene, wenn der Schleicher ausgewählt ist
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
