using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Importiere das SceneManagement-Namespace

public class S_PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    private Renderer playerRenderer; // Renderer, um die Sichtbarkeit zu steuern
    private Collider2D playerCollider; // Collider, um die Angreifbarkeit zu steuern

    private Camera mainCamera; // Referenz zur Hauptkamera

    // F�ge eine �ffentliche Variable hinzu, um den Namen der n�chsten Szene zu speichern
    public string sceneToLoad; // Name der Szene, die geladen werden soll

    void Start()
    {
        currentHealth = maxHealth;
        playerRenderer = GetComponent<Renderer>(); // Hole den Renderer des Spielers
        playerCollider = GetComponent<Collider2D>(); // Hole den Collider des Spielers
        mainCamera = Camera.main; // Hole die Hauptkamera
    }

    // Funktion, um dem Spieler Schaden zuzuf�gen
    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        Debug.Log("Spieler nimmt " + damageAmount + " Schaden. Aktuelle Gesundheit: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Spieler stirbt
    void Die()
    {
        Debug.Log("Spieler ist gestorben.");

        // Teleportiere den Spieler au�erhalb des Sichtbereichs
        transform.position = new Vector3(1000, 1000, 0); // �ndere die Position zum Verschwinden
        playerCollider.enabled = false; // Deaktiviert den Collider, um Angriffe zu verhindern

        // Fixiere die Kamera an der aktuellen Spielerposition
        FixCameraAtPlayerPosition(transform.position);

        // Lade die n�chste Szene
        SceneManager.LoadScene(sceneToLoad); // Lade die Szene, die in sceneToLoad angegeben ist
    }

    // Fixiere die Kamera an der Position des Spielers
    private void FixCameraAtPlayerPosition(Vector3 playerPosition)
    {
        if (mainCamera != null)
        {
            mainCamera.transform.position = new Vector3(playerPosition.x, playerPosition.y, mainCamera.transform.position.z);
        }
    }
}
