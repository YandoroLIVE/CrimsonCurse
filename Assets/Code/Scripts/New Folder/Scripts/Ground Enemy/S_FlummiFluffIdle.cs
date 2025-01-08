using System.Collections;
using UnityEngine;

public class S_FlummiFluff : MonoBehaviour
{
    public Transform player;                    // Referenz auf den Player
    public float jumpDuration = 1.0f;           // Sprungdauer
    public float jumpHeightFactor = 1.0f;       // Faktor f�r die Sprungh�he
    public float detectionRadius = 5.0f;        // Erkennungsradius f�r den Spieler
    public float attackRangeMax = 7.0f;         // Maximale Angriffsreichweite
    public AnimationCurve jumpCurve;            // Animationskurve f�r den Sprung
    public LayerMask groundLayer;               // Layer f�r den Boden (z. B. "Wall")
    public float idleJumpDistance = 2.0f;       // Distanz f�r Idle-Spr�nge
    public float groundCheckRadius = 0.2f;      // Radius f�r die Bodenpr�fung
    public Transform groundCheck;               // Transform, das den Bodenpr�fpunkt darstellt

    private Rigidbody2D rb;                     // Rigidbody2D f�r die physikalische Bewegung
    private Vector3 currentTarget;              // Aktuelles Sprungziel
    private Vector3 startPosition;              // Startposition des aktuellen Sprungs
    private float jumpStartTime;                // Zeit, zu der der Sprung begonnen hat
    private bool isJumping = false;             // Flag f�r Sprungstatus
    private string currentState = "Idle";       // Aktueller Zustand (Idle, Attack)
    private bool jumpToLeft = true;             // Richtung des n�chsten Idle-Sprungs

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
        // Pr�fe, ob der FlummiFluff auf dem Boden ist
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

        // Wenn kein Sprung l�uft, Ziel wechseln (hin und her)
        if (!isJumping)
        {
            SetIdleTarget();
            StartJump();
        }
    }

    private void HandleAttackState(float distanceToPlayer)
    {
        // Zur�ck in den Idle-Zustand, wenn der Spieler au�er Reichweite ist
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
        // Ziel f�r Idle-Sprung wechseln
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
        // Pr�fe, ob der FlummiFluff den Boden ber�hrt
        Collider2D groundCollider = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Debugging: Zeichne den Ground Check im Editor
        Debug.DrawRay(groundCheck.position, Vector2.down * groundCheckRadius, groundCollider != null ? Color.green : Color.red);

        // Debug.Log Ausgabe f�r Bodenpr�fung
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
