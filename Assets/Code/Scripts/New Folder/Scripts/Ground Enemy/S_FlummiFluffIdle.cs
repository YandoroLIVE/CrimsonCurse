using System.Collections;
using UnityEngine;

public class S_FlummiFluff : MonoBehaviour
{
    public Transform player;                   // Referenz auf den Player
    public float jumpDuration = 1.0f;          // Sprungdauer
    public float jumpHeightFactor = 1.0f;      // Faktor f�r die Sprungh�he (dynamisch)
    public float detectionRadius = 5.0f;       // Erkennungsradius f�r den Spieler
    public float attackRangeMax = 7.0f;        // Maximale Angriffsreichweite
    public float landPauseDuration = 0.3f;     // Pause nach der Landung
    public AnimationCurve jumpCurve;           // Animationskurve f�r den Sprung
    public LayerMask groundLayer;              // Layer f�r den Boden (jetzt "Wall")
    public Vector3 idleLeftOffset = Vector3.left * 2.0f; // Idle-Sprung nach links
    public Vector3 idleRightOffset = Vector3.right * 2.0f; // Idle-Sprung nach rechts

    private Rigidbody2D rb;                    // Rigidbody2D f�r die physikalische Bewegung
    private bool isIdle = true;                // Flag f�r den Idle-Zustand
    private bool triggeredOnPlayer = false;    // Flag, um den direkten Sprung nach Kontakt zu steuern
    private Vector3 lastJumpTarget;            // Speichert die letzte Sprungzielposition

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (player == null)
        {
            player = GameObject.FindWithTag("Player")?.transform;
        }

        if (player == null)
        {
            Debug.LogError("Player Transform nicht gefunden!");
            return;
        }

        if (jumpCurve == null || jumpCurve.keys.Length == 0)
        {
            jumpCurve = AnimationCurve.EaseInOut(0, 0, 1, 0.8f);
        }

        StartCoroutine(IdleJumpPattern());
    }

    private IEnumerator IdleJumpPattern()
    {
        Vector3 startPosition = transform.position;

        while (isIdle)
        {
            // Springe abwechselnd nach links und rechts
            Vector3 leftTarget = startPosition + idleLeftOffset;
            Vector3 rightTarget = startPosition + idleRightOffset;

            yield return StartCoroutine(JumpArc(leftTarget));
            yield return new WaitForSeconds(landPauseDuration);

            yield return StartCoroutine(JumpArc(rightTarget));
            yield return new WaitForSeconds(landPauseDuration);

            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer <= detectionRadius && distanceToPlayer <= attackRangeMax)
            {
                isIdle = false;
                StartCoroutine(AttackPattern());
                yield break;
            }
        }
    }

    private IEnumerator AttackPattern()
    {
        while (true)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            // Wenn Spieler au�erhalb der Reichweite ist, zum Idle-Modus zur�ckkehren
            if (distanceToPlayer > detectionRadius || distanceToPlayer > attackRangeMax)
            {
                isIdle = true;
                StartCoroutine(IdleJumpPattern());
                yield break;
            }

            // Sprung auf den Spieler
             // Reset, bevor der Sprung beginnt
            lastJumpTarget = player.position; // Merke Zielposition
            yield return StartCoroutine(JumpArc(player.position));

            // Wenn der Trigger auf den Player gehittet wurde, sofort weiter springen
            if (triggeredOnPlayer)
            {
                // Berechne eine neue Richtung f�r den n�chsten Sprung
                Vector3 direction = (player.position - transform.position).normalized;

                // Berechne den Abstand und passe die Sprungkraft an
                float distanceToPlayerAttack = Vector3.Distance(transform.position, player.position); // Umbenannt f�r die Attack-Logik
                float jumpStrength = Mathf.Lerp(3.0f, 7.0f, 1.0f - Mathf.Clamp01(distanceToPlayerAttack / detectionRadius)); // Mehr Kraft, je n�her der FlummiFluff kommt

                //direction.x = distanceToPlayerAttack <= 3 ? -direction.x : direction.x;
                //Debug.Log("executed");
                Vector3 targetPosition = transform.position + direction * jumpStrength; // Etwas weiter in dieselbe Richtung
                Debug.Log(direction * jumpStrength);
                // Finde die Bodenposition, falls m�glich
                RaycastHit2D groundHit = Physics2D.Raycast(targetPosition, Vector2.down, Mathf.Infinity, groundLayer);
                if (groundHit.collider != null)
                {
                    targetPosition = groundHit.point; // Ziel ist der Boden
                }
                triggeredOnPlayer = false;
                // Sofortiger Sprung in die neue Richtung
                yield return StartCoroutine(JumpArc(targetPosition));
                continue; // Direkt zum n�chsten Sprung

            }

            yield return new WaitForSeconds(landPauseDuration);
        }
    }

    private IEnumerator JumpArc(Vector3 targetPosition)
    {
        Vector3 start = transform.position;
        float horizontalDistance = Vector3.Distance(new Vector3(start.x, 0, 0), new Vector3(targetPosition.x, 0, 0));
        float timeElapsed = 0;

        float dynamicJumpHeight = horizontalDistance * jumpHeightFactor;

        rb.gravityScale = 0; // Schwerkraft w�hrend des Sprungs ausschalten

        while (timeElapsed < jumpDuration)
        {
            timeElapsed += Time.deltaTime;

            float t = timeElapsed / jumpDuration;

            float height = jumpCurve.Evaluate(t) * dynamicJumpHeight;

            Vector3 newPosition = Vector3.Lerp(start, targetPosition, t);
            newPosition.y += height;

            rb.MovePosition(newPosition);

            yield return null;
        }

        // Am Ende des Sprungs den Zielpunkt erreichen
        rb.MovePosition(targetPosition);

        // Schwerkraft wieder einschalten
        rb.gravityScale = 1;

        // Zus�tzliche �berpr�fung, ob der Gegner den Boden ber�hrt
        RaycastHit2D groundHit = Physics2D.Raycast(transform.position, Vector2.down, 1f, groundLayer);
        if (groundHit.collider != null)
        {
            Debug.Log("FlummiFluff hat den Boden ber�hrt.");
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other.gameObject.tag);
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("FlummiFluff ist auf dem Spieler gelandet!");
            triggeredOnPlayer = true; // Setze das Flag, um das Delay zu �berspringen
        }
    }
}
