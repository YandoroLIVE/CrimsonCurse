using System.Collections;
using UnityEngine;

public class S_FlummiFluff : MonoBehaviour
{
    public Transform player;                    // Referenz auf den Player
    public float jumpDuration = 1.0f;           // Sprungdauer
    public float jumpDistance = 2.0f;           // Sprungdistanz
    public float jumpHeight = 1.0f;             // Sprunghöhe
    public float jumpVelocity = 10.0f;          // Anfangsgeschwindigkeit beim Absprung
    public float fallVelocity = 5.0f;           // Fallgeschwindigkeit
    public LayerMask groundLayer;               // Layer für den Boden
    public AnimationCurve jumpCurve;            // Animationskurve für den Sprung
    public float moveSpeed = 3.0f;              // Bewegungs-Geschwindigkeit
    public float detectionRadius = 5.0f;        // Erkennungsradius für den Spieler

    private Vector3 startPosition;              // Startposition des FlummiFluffs
    private bool isIdle = true;                 // Flag, ob der FlummiFluff im Idle-State ist
    private bool isAttacking = false;           // Flag, ob der FlummiFluff im Attack-State ist
    private bool isPlayerInRange = false;       // Flag, ob der Spieler im Erkennungsradius ist

    public float landPauseDuration = 0.5f;      // Pause nach der Landung

    private void Start()
    {
        // Überprüfe, ob der Player zugewiesen wurde
        if (player == null)
        {
            player = GameObject.FindWithTag("Player")?.transform;
        }

        // Überprüfe, ob der Player gefunden wurde
        if (player == null)
        {
            Debug.LogError("Player Transform nicht gefunden!");
            return;
        }

        groundLayer = LayerMask.GetMask("Wall");

        // Falls die Animationskurve nicht im Editor gesetzt wurde, eine Standardkurve definieren
        if (jumpCurve == null || jumpCurve.keys.Length == 0)
        {
            jumpCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.2f, 1.2f), new Keyframe(0.5f, 1.0f), new Keyframe(1, 0));
        }

        startPosition = transform.position;

        // Starte den Idle-Sprungzyklus
        StartCoroutine(IdleJumpRoutine());
    }

    private void Update()
    {
        // Überprüfe, ob der Spieler im Detection Radius ist
        if (Vector3.Distance(transform.position, player.position) <= detectionRadius && !isAttacking)
        {
            if (!isPlayerInRange)
            {
                isPlayerInRange = true;
                // Wenn der Spieler erkannt wird und der FlummiFluff im Idle-Zustand ist, wechseln wir in den Attack-State
                if (isIdle)
                {
                    isIdle = false;
                    StopCoroutine(IdleJumpRoutine()); // Stoppe den Idle-Sprungzyklus
                    StartCoroutine(AttackPattern());   // Starte den Angriff
                }
            }
        }
        else if (Vector3.Distance(transform.position, player.position) > detectionRadius && isPlayerInRange)
        {
            if (isAttacking)
            {
                isPlayerInRange = false;
                isAttacking = false;
                StopCoroutine(AttackPattern()); // Stoppe den Angriff
                isIdle = true;
                StartCoroutine(IdleJumpRoutine()); // Zurück zum Idle-State
            }
        }

        // Während der Rückkehr zum Boden die Fallgeschwindigkeit anwenden, wenn der Gegner nicht auf dem Boden ist
        if (!IsGroundBelow())
        {
            transform.position += Vector3.down * Time.deltaTime * fallVelocity;
        }
    }

    private IEnumerator IdleJumpRoutine()
    {
        while (isIdle)
        {
            // Zielposition für den Sprung nach rechts berechnen
            Vector3 targetPosition = startPosition + new Vector3(jumpDistance, 0, 0);

            // Sprung in einem Bogen zur Zielposition
            yield return StartCoroutine(JumpArc(targetPosition));

            // Kurze Pause nach der Landung
            yield return new WaitForSeconds(landPauseDuration);

            // Sprung in einem Bogen zurück zur Startposition
            yield return StartCoroutine(JumpArc(startPosition));

            // Kurze Pause nach der Landung
            yield return new WaitForSeconds(landPauseDuration);
        }
    }

    private IEnumerator AttackPattern()
    {
        isAttacking = true;

        Vector3 nextTargetPosition;
        bool moveToLeft = true;

        // Endlosschleife für den Angriff
        while (isAttacking)
        {
            // Bestimme die Zielposition je nach Seite des Spielers
            if (moveToLeft)
            {
                nextTargetPosition = player.position + new Vector3(-jumpDistance, 0, 0); // Ziel links vom Spieler
            }
            else
            {
                nextTargetPosition = player.position + new Vector3(jumpDistance, 0, 0); // Ziel rechts vom Spieler
            }

            // Dynamische Anpassung der Sprungdauer je nach Entfernung zum Ziel
            float distanceToTarget = Vector3.Distance(transform.position, nextTargetPosition);
            float adjustedJumpDuration = Mathf.Clamp(distanceToTarget / moveSpeed, 1.0f, 2.0f); // Die Dauer des Sprungs wird an die Distanz angepasst

            // Sprung zur nächsten Zielposition, aber vorher Kollisionsprüfung
            yield return StartCoroutine(JumpArc(nextTargetPosition, adjustedJumpDuration));

            // Kurze Pause nach der Landung
            yield return new WaitForSeconds(landPauseDuration);

            // Wechsel die Richtung für den nächsten Sprung
            moveToLeft = !moveToLeft;
        }
    }

    private IEnumerator JumpArc(Vector3 targetPosition, float jumpDuration = 1.0f)
    {
        Vector3 start = transform.position;
        float timeElapsed = 0;
        bool groundDetected = false;
        float initialHeight = start.y;
        float peakHeight = initialHeight + jumpHeight;

        // Berechne die horizontale Bewegung des FlummiFluffs
        float horizontalDistance = Vector3.Distance(start, targetPosition);

        // Simuliere eine parabolische Bewegung
        while (timeElapsed < jumpDuration)
        {
            float t = timeElapsed / jumpDuration;
            float curveValue = jumpCurve.Evaluate(t);
            float height = curveValue * jumpHeight;  // Höhe des Sprungs wird von der Animationskurve bestimmt

            // Berechne die horizontale Bewegung (zwischen Start- und Zielposition)
            Vector3 targetWithHeight = Vector3.Lerp(start, targetPosition, t) + new Vector3(0, height, 0);

            // Kollisionsabfrage: Verhindere das Hängenbleiben im Player Collider
            if (IsPlayerColliderNearby(targetWithHeight))
            {
                // Wenn Kollision erkannt wird, passe die Zielposition nach oben an
                targetWithHeight.y = Mathf.Max(targetWithHeight.y, transform.position.y + 1); // Stelle sicher, dass er drüber springt
            }

            if (IsGroundBelow() && !groundDetected)
            {
                targetWithHeight.y = Mathf.Min(targetWithHeight.y, GetGroundHeight());
                groundDetected = true;
            }

            transform.position = targetWithHeight;
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // Stelle sicher, dass der FlummiFluff exakt an der Zielposition landet
        transform.position = targetPosition;
    }

    // Kollisionsabfrage: Überprüft, ob der FlummiFluff mit dem Player kollidiert
    private bool IsPlayerColliderNearby(Vector3 targetPosition)
    {
        Collider2D playerCollider = player.GetComponent<Collider2D>();
        if (playerCollider != null)
        {
            // Prüfen, ob der FlummiFluff sich dem Player-Collider nähert
            return playerCollider.bounds.Contains(targetPosition);
        }
        return false;
    }

    private bool IsGroundBelow()
    {
        return Physics2D.Raycast(transform.position, Vector2.down, Mathf.Infinity, groundLayer);
    }

    private float GetGroundHeight()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, Mathf.Infinity, groundLayer);
        return hit.point.y;
    }
}
