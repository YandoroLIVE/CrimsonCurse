using System.Collections;
using UnityEngine;

public class S_FlummiFluff : MonoBehaviour
{
    public Transform player;                    // Referenz auf den Player
    public float jumpDuration = 1.0f;           // Sprungdauer
    public float jumpHeightFactor = 1.0f;       // Faktor für die Sprunghöhe
    public float detectionRadius = 5.0f;        // Erkennungsradius für den Spieler
    public float attackRangeMax = 7.0f;         // Maximale Angriffsreichweite
    public AnimationCurve jumpCurve;            // Animationskurve für den Sprung
    public LayerMask groundLayer;               // Layer für den Boden (z. B. "Wall")
    public float idleJumpDistance = 2.0f;       // Distanz für Idle-Sprünge
    public float groundCheckRadius = 0.2f;      // Radius für die Bodenprüfung
    public Transform groundCheck;               // Transform, das den Bodenprüfpunkt darstellt

    private Rigidbody2D rb;                     // Rigidbody2D für die physikalische Bewegung
    private Vector3 currentTarget;              // Aktuelles Sprungziel
    private Vector3 startPosition;              // Startposition des aktuellen Sprungs
    private float jumpStartTime;                // Zeit, zu der der Sprung begonnen hat
    private bool isJumping = false;             // Flag für Sprungstatus
    private string currentState = "Idle";       // Aktueller Zustand (Idle, Attack)
    private bool jumpToLeft = true;             // Richtung des nächsten Idle-Sprungs

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (player == null)
        {
            player = GameObject.FindWithTag("Player")?.transform;
        }

        if (jumpCurve == null || jumpCurve.keys.Length == 0)
        {
            jumpCurve = AnimationCurve.EaseInOut(0, 0, 1, 0.8f);
        }

        SetIdleTarget();
    }

    private void Update()
    {
        // Prüfe, ob der FlummiFluff auf dem Boden ist
        bool isGrounded = CheckGrounded();

        if (isGrounded)
        {
            Debug.Log("Boden gefunden!");
        }
        else
        {
            Debug.Log("Kein Boden gefunden.");
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        switch (currentState)
        {
            case "Idle":
                if (isGrounded)
                    HandleIdleState(distanceToPlayer);
                break;

            case "Attack":
                if (isGrounded)
                    HandleAttackState(distanceToPlayer);
                break;
        }

        if (isJumping)
        {
            ProcessJump();
        }
    }

    private void HandleIdleState(float distanceToPlayer)
    {
        // Wechsel in den Attack-Zustand, wenn der Spieler nah genug ist
        if (distanceToPlayer <= detectionRadius && distanceToPlayer <= attackRangeMax)
        {
            currentState = "Attack";
            currentTarget = player.position;
            StartJump();
        }

        // Wenn kein Sprung läuft, Ziel wechseln (hin und her)
        if (!isJumping)
        {
            SetIdleTarget();
            StartJump();
        }
    }

    private void HandleAttackState(float distanceToPlayer)
    {
        // Zurück in den Idle-Zustand, wenn der Spieler außer Reichweite ist
        if (distanceToPlayer > detectionRadius || distanceToPlayer > attackRangeMax)
        {
            currentState = "Idle";
            SetIdleTarget();
            StartJump();
            return;
        }

        if (!isJumping)
        {
            // Springe auf den Spieler
            currentTarget = player.position;
            StartJump();
        }
    }

    private void StartJump()
    {
        startPosition = transform.position;
        jumpStartTime = Time.time;
        isJumping = true;
    }

    private void ProcessJump()
    {
        float timeElapsed = Time.time - jumpStartTime;
        float t = timeElapsed / jumpDuration;

        if (t >= 1.0f)
        {
            // Sprung beendet
            transform.position = currentTarget;
            isJumping = false;
            return;
        }

        // Berechne Position basierend auf der Zeit
        float height = jumpCurve.Evaluate(t) * (Vector3.Distance(startPosition, currentTarget) * jumpHeightFactor);
        Vector3 newPosition = Vector3.Lerp(startPosition, currentTarget, t);
        newPosition.y += height;

        rb.MovePosition(newPosition);
    }

    private void SetIdleTarget()
    {
        // Ziel für Idle-Sprung wechseln
        if (jumpToLeft)
        {
            currentTarget = transform.position + Vector3.left * idleJumpDistance;
        }
        else
        {
            currentTarget = transform.position + Vector3.right * idleJumpDistance;
        }

        jumpToLeft = !jumpToLeft; // Richtung wechseln
    }

    private bool CheckGrounded()
    {
        // Prüfe, ob der FlummiFluff den Boden berührt
        Collider2D groundCollider = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Debugging: Zeichne den Ground Check im Editor
        Debug.DrawRay(groundCheck.position, Vector2.down * groundCheckRadius, groundCollider != null ? Color.green : Color.red);

        // Debug.Log Ausgabe für Bodenprüfung
        if (groundCollider != null)
        {
            Debug.Log($"Boden erkannt: {groundCollider.gameObject.name}");

            currentTarget.y = Mathf.Min(currentTarget.y, groundCheck.position.y);
        }

        return groundCollider != null;
    }


    private void OnDrawGizmosSelected()
    {
        // Zeichne den Ground Check Radius im Editor
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
