using System.Collections;
using UnityEngine;

public class S_FlummiFluff : MonoBehaviour
{
    public Transform player;                    // Referenz auf den Player
    public float jumpDuration = 1.0f;           // Sprungdauer
    public float jumpDistance = 2.0f;           // Sprungdistanz
    public float jumpHeight = 1.0f;             // Sprunghöhe
    public float moveSpeed = 3.0f;              // Bewegungs-Geschwindigkeit
    public float detectionRadius = 5.0f;        // Erkennungsradius für den Spieler
    public float landPauseDuration = 0.5f;      // Pause nach der Landung

    private Vector3 startPosition;              // Startposition des FlummiFluffs
    private bool isIdle = true;                 // Flag, ob der FlummiFluff im Idle-State ist
    private bool isAttacking = false;           // Flag, ob der FlummiFluff im Attack-State ist
    private bool isPlayerInRange = false;       // Flag, ob der Spieler im Erkennungsradius ist
    private float playerColliderHeight = 1.0f;  // Eine Mindesthöhe, um den Spieler zu überspringen

    private void Start()
    {
        if (player == null)
        {
            player = GameObject.FindWithTag("Player")?.transform;
        }

        if (player == null)
        {
            Debug.LogError("Player Transform nicht gefunden!");
            return;
        }

        startPosition = transform.position;
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
                if (isIdle)
                {
                    isIdle = false;
                    StopCoroutine(IdleJumpRoutine());
                    StartCoroutine(AttackPattern());
                }
            }
        }
        else if (Vector3.Distance(transform.position, player.position) > detectionRadius && isPlayerInRange)
        {
            if (isAttacking)
            {
                isPlayerInRange = false;
                isAttacking = false;
                StopCoroutine(AttackPattern());
                isIdle = true;
                StartCoroutine(IdleJumpRoutine());
            }
        }

        if (!IsGroundBelow())
        {
            transform.position += Vector3.down * Time.deltaTime * 5f;
        }
    }

    private IEnumerator IdleJumpRoutine()
    {
        while (isIdle)
        {
            Vector3 targetPosition = startPosition + new Vector3(jumpDistance, 0, 0);
            yield return StartCoroutine(JumpArc(targetPosition));
            yield return new WaitForSeconds(landPauseDuration);
            yield return StartCoroutine(JumpArc(startPosition));
            yield return new WaitForSeconds(landPauseDuration);
        }
    }

    private IEnumerator AttackPattern()
    {
        isAttacking = true;
        bool moveToLeft = true;

        while (isAttacking)
        {
            Vector3 nextTargetPosition;
            if (moveToLeft)
            {
                nextTargetPosition = player.position + new Vector3(-jumpDistance, 0, 0);
            }
            else
            {
                nextTargetPosition = player.position + new Vector3(jumpDistance, 0, 0);
            }

            // Dynamische Anpassung der Sprungdauer je nach Geschwindigkeit des Spielers
            float distanceToTarget = Vector3.Distance(transform.position, nextTargetPosition);
            float adjustedJumpDuration = Mathf.Clamp(distanceToTarget / moveSpeed, 1.0f, 2.0f);

            yield return StartCoroutine(JumpArc(nextTargetPosition, adjustedJumpDuration));

            yield return new WaitForSeconds(landPauseDuration);
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

        float horizontalDistance = Vector3.Distance(start, targetPosition);

        while (timeElapsed < jumpDuration)
        {
            float t = timeElapsed / jumpDuration;
            float curveValue = Mathf.Sin(t * Mathf.PI);  // Einfacher Sinus-Kurve zur Sprungberechnung
            float height = curveValue * jumpHeight;

            Vector3 targetWithHeight = Vector3.Lerp(start, targetPosition, t) + new Vector3(0, height, 0);

            // Korrektur, falls der FlummiFluff über dem Spieler landet
            if (IsPlayerColliderNearby(targetWithHeight))
            {
                targetWithHeight.y = Mathf.Max(targetWithHeight.y, transform.position.y + playerColliderHeight);
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

        transform.position = targetPosition;
    }

    private bool IsPlayerColliderNearby(Vector3 targetPosition)
    {
        return Physics2D.OverlapCircle(targetPosition, 0.5f, LayerMask.GetMask("Player"));
    }

    private bool IsGroundBelow()
    {
        return Physics2D.Raycast(transform.position, Vector2.down, Mathf.Infinity, LayerMask.GetMask("Ground"));
    }

    private float GetGroundHeight()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, Mathf.Infinity, LayerMask.GetMask("Ground"));
        return hit.point.y;
    }
}
