using System;
using Unity.VisualScripting;
using System.Collections;
using UnityEngine;

public class S_FlummiFluffIdle : MonoBehaviour
{
    // Sprungparameter
    public float jumpDuration = 1.0f;   // Gesamtdauer des Sprungs
    public float jumpDistance = 2.0f;    // Horizontale Distanz des Sprungs
    public float jumpHeight = 1.0f;      // Maximale H�he des Sprungs

    // Parameter f�r die Absprunggeschwindigkeit (ein Multiplikator f�r die Anfangsgeschwindigkeit)
    public float jumpVelocity = 10.0f;   // Initiale Geschwindigkeit beim Absprung

    // Parameter f�r die Fallgeschwindigkeit
    public float fallVelocity = 5.0f;     // Geschwindigkeit des Falls

    // Layer f�r die Bodenpr�fung (z.B., "Wall")
    public LayerMask groundLayer;

    // Animation Curve f�r dynamischen Sprung
    public AnimationCurve jumpCurve;

    private Vector3 startPosition;
    private bool isIdle = true;

    // Pause nach der Landung
    public float landPauseDuration = 0.5f;

    private void Start()
    {
        // Setze den Layer f�r die Bodenpr�fung auf den Layer "Wall"
        groundLayer = LayerMask.GetMask("Wall");

        // Standard-Animation Curve f�r den dynamischen Sprung setzen, falls nicht im Editor gesetzt
        if (jumpCurve == null || jumpCurve.keys.Length == 0)
        {
            jumpCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.2f, 1.2f), new Keyframe(0.5f, 1.0f), new Keyframe(1, 0));
        }

        // Speichere die Anfangsposition des Gegners
        startPosition = transform.position;

        // Starte die Landungsroutine, bevor der Sprungzyklus beginnt
        StartCoroutine(StartLandingRoutine());
    }

    private IEnumerator StartLandingRoutine()
    {
        // Landevorgang, wenn ein Boden erkannt wird
        while (!IsGroundBelow())
        {
            // Bewege den Gegner nach unten
            transform.position += Vector3.down * Time.deltaTime * fallVelocity;  // Verwende fallVelocity f�r die Bewegung nach unten

            // Warte einen Frame und �berpr�fe erneut
            yield return null;
        }

        // Stelle sicher, dass er auf dem Boden landet
        transform.position = new Vector3(transform.position.x, GetGroundHeight(), transform.position.z);

        // Starte nun den regul�ren Sprungzyklus
        StartCoroutine(IdleJumpRoutine());
    }

    private IEnumerator IdleJumpRoutine()
    {
        while (isIdle)
        {
            // Zielposition f�r den Sprung nach rechts berechnen
            Vector3 targetPosition = startPosition + new Vector3(jumpDistance, 0, 0);

            // Sprung in einem Bogen zur Zielposition
            yield return StartCoroutine(JumpArc(targetPosition));

            // Kurze Pause nach der Landung
            yield return new WaitForSeconds(landPauseDuration);

            // Sprung in einem Bogen zur�ck zur Startposition
            yield return StartCoroutine(JumpArc(startPosition));

            // Kurze Pause nach der Landung
            yield return new WaitForSeconds(landPauseDuration);
        }
    }

    private IEnumerator JumpArc(Vector3 targetPosition)
    {
        Vector3 start = transform.position;
        float timeElapsed = 0;
        bool groundDetected = false;

        // Berechne die Endh�he des Sprungs (Maximalh�he)
        float initialHeight = start.y;  // Anfangsh�he
        float peakHeight = initialHeight + jumpHeight; // H�chster Punkt des Sprungs

        // Sprungbewegung
        while (timeElapsed < jumpDuration)
        {
            float t = timeElapsed / jumpDuration;
            float curveValue = jumpCurve.Evaluate(t);
            float height = curveValue * jumpHeight;  // H�he des Sprungs wird von jumpHeight bestimmt

            // Berechne die Zielposition mit dynamischer H�he
            Vector3 targetWithHeight = Vector3.Lerp(start, targetPosition, t) + new Vector3(0, height, 0);

            // �berpr�fe, ob unter dem Gegner eine "Wall" ist
            if (IsGroundBelow() && !groundDetected)
            {
                // Landet auf der Y-Position des Bodens
                targetWithHeight.y = Mathf.Min(targetWithHeight.y, GetGroundHeight());

                // Markiere den Boden als erreicht
                groundDetected = true;
            }

            transform.position = targetWithHeight;

            // Zeit um die Sprungbewegung zu berechnen
            timeElapsed += Time.deltaTime; // Hier verwenden wir einfach die verstrichene Zeit
            yield return null;
        }

        // Sicherstellen, dass der Gegner an der exakten Zielposition landet
        transform.position = targetPosition;
    }

    private void Update()
    {
        // W�hrend der R�ckkehr zum Boden die Fallgeschwindigkeit anwenden, wenn der Gegner nicht auf dem Boden ist
        if (!IsGroundBelow())
        {
            transform.position += Vector3.down * Time.deltaTime * fallVelocity;
        }
    }

    private bool IsGroundBelow()
    {
        // Raycast nach unten, um den Boden (Layer "Wall") zu detektieren
        return Physics2D.Raycast(transform.position, Vector2.down, Mathf.Infinity, groundLayer);
    }

    private float GetGroundHeight()
    {
        // F�hrt einen Raycast durch, um die Y-Position des Bodens unter dem Gegner zu ermitteln
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, Mathf.Infinity, groundLayer);
        return hit.point.y;
    }
}













