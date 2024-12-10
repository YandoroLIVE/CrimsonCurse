using System.Collections;
using UnityEngine;

public class S_FlummiFluff : MonoBehaviour
{
    // Sprungparameter
    public float jumpDuration = 1.0f;   // Gesamtdauer des Sprungs
    public float jumpDistance = 2.0f;   // Horizontale Distanz des Sprungs
    public float jumpHeight = 2.0f;     // Maximale Höhe des Sprungs

    // Zielobjekt (der Spieler)
    public Transform player;

    // Parameter für die Fallgeschwindigkeit
    public float fallVelocity = 5.0f;

    // Layer für die Bodenprüfung (wir verwenden "Wall")
    public LayerMask groundLayer;

    // Animation Curve für den dynamischen Sprung
    public AnimationCurve jumpCurve;

    private Vector3 startPosition;
    private bool isIdle = true;
    private bool isAttacking = false;

    // Pause nach der Landung
    public float landPauseDuration = 0.5f;

    // Geschwindigkeit der Annäherung an den Spieler (Bewegung zwischen Idle und Attack)
    public float approachSpeed = 3.0f;

    // Zum Tracking der Bewegungsrichtung des FlummiFluffs (links oder rechts)
    private bool isMovingRight = true;

    private void Start()
    {
        // Setze die Layer für die Bodenprüfung auf den Layer "Wall"
        groundLayer = LayerMask.GetMask("Wall");

        // Standard-Animation Curve setzen, falls nicht definiert
        if (jumpCurve == null || jumpCurve.keys.Length == 0)
        {
            jumpCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.2f, 1.2f), new Keyframe(0.5f, 1.0f), new Keyframe(1, 0));
        }

        // Speichere die Anfangsposition des FlummiFluffs
        startPosition = transform.position;

        // Beginne im Idle-State
        StartCoroutine(IdleRoutine());
    }

    private IEnumerator IdleRoutine()
    {
        while (isIdle)
        {
            // Berechne die Richtung zum Spieler
            Vector3 targetPosition = player.position;
            bool playerIsOnRight = targetPosition.x > transform.position.x;

            // Wenn der Spieler auf der rechten Seite ist, Bounce nach rechts, dann links, usw.
            if (playerIsOnRight)
            {
                yield return StartCoroutine(JumpArc(targetPosition + new Vector3(jumpDistance, 0, 0)));
                yield return new WaitForSeconds(landPauseDuration);

                yield return StartCoroutine(JumpArc(targetPosition + new Vector3(-jumpDistance, 0, 0)));
                yield return new WaitForSeconds(landPauseDuration);
            }
            // Wenn der Spieler auf der linken Seite ist, Bounce nach links, dann rechts, usw.
            else
            {
                yield return StartCoroutine(JumpArc(targetPosition + new Vector3(-jumpDistance, 0, 0)));
                yield return new WaitForSeconds(landPauseDuration);

                yield return StartCoroutine(JumpArc(targetPosition + new Vector3(jumpDistance, 0, 0)));
                yield return new WaitForSeconds(landPauseDuration);
            }
        }
    }

    private IEnumerator AttackRoutine()
    {
        isIdle = false;
        isAttacking = true;

        // Bewegung zum Spieler (als Zwischenschritt zwischen Idle und Attack)
        yield return StartCoroutine(MoveToPlayer());

        // Solange der FlummiFluff angreift, springt er nach links und rechts
        while (isAttacking)
        {
            // Berechne die Richtung zum Spieler
            Vector3 targetPosition = player.position;
            bool playerIsOnRight = targetPosition.x > transform.position.x;

            // Wenn der Spieler auf der rechten Seite ist, Bounce nach rechts, dann links, usw.
            if (playerIsOnRight)
            {
                yield return StartCoroutine(JumpArc(targetPosition + new Vector3(jumpDistance, 0, 0)));
                yield return new WaitForSeconds(landPauseDuration);

                yield return StartCoroutine(JumpArc(targetPosition + new Vector3(-jumpDistance, 0, 0)));
                yield return new WaitForSeconds(landPauseDuration);
            }
            // Wenn der Spieler auf der linken Seite ist, Bounce nach links, dann rechts, usw.
            else
            {
                yield return StartCoroutine(JumpArc(targetPosition + new Vector3(-jumpDistance, 0, 0)));
                yield return new WaitForSeconds(landPauseDuration);

                yield return StartCoroutine(JumpArc(targetPosition + new Vector3(jumpDistance, 0, 0)));
                yield return new WaitForSeconds(landPauseDuration);
            }
        }
    }

    private IEnumerator MoveToPlayer()
    {
        // Annäherung an den Spieler (Bewegung vor dem Angriff)
        Vector3 targetPosition = player.position;
        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            // Bewege den FlummiFluff in Richtung des Spielers
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, approachSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private IEnumerator JumpArc(Vector3 targetPosition)
    {
        Vector3 start = transform.position;
        float timeElapsed = 0;
        bool groundDetected = false;

        while (timeElapsed < jumpDuration)
        {
            float t = timeElapsed / jumpDuration;
            float curveValue = jumpCurve.Evaluate(t);
            float height = curveValue * jumpHeight;

            Vector3 targetWithHeight = Vector3.Lerp(start, targetPosition, t) + new Vector3(0, height, 0);

            // Überprüfe, ob der FlummiFluff den Boden berührt
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

    private void Update()
    {
        // Fällt, wenn kein Boden unter dem FlummiFluff erkannt wird
        if (!IsGroundBelow())
        {
            transform.position += Vector3.down * Time.deltaTime * fallVelocity;
        }

        // Wenn der FlummiFluff nahe genug am Spieler ist, beginnt der Angriff
        if (Vector3.Distance(transform.position, player.position) < 5.0f && !isAttacking)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    private bool IsGroundBelow()
    {
        // Raycast nach unten, um den Boden zu überprüfen
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, Mathf.Infinity, groundLayer);
        return hit.collider != null;
    }

    private float GetGroundHeight()
    {
        // Raycast, um die Höhe des Bodens unter dem FlummiFluff zu ermitteln
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, Mathf.Infinity, groundLayer);
        return hit.point.y;
    }
}
