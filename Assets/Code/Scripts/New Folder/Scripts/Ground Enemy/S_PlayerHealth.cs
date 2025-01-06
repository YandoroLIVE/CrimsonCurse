
using UnityEngine;
using UnityEngine.SceneManagement; // Importiere das SceneManagement-Namespace


public class S_PlayerHealth : MonoBehaviour
{
    const int FIRST_LEVEL_BUILD_INDEX = 1;
    public int maxHealth = 100;
    public int currentHealth;
    public int healthRegenOnKill = 25; // Menge an Gesundheit, die bei einem Kill regeneriert wird

    private Renderer playerRenderer; // Renderer, um die Sichtbarkeit zu steuern
    private Collider2D playerCollider; // Collider, um die Angreifbarkeit zu steuern

    private Camera mainCamera; // Referenz zur Hauptkamera

    public HealthUi healthUi;

    // Füge eine öffentliche Variable hinzu, um den Namen der nächsten Szene zu speichern
    public string sceneToLoad; // Name der Szene, die geladen werden soll

    void Start()
    {
        currentHealth = maxHealth;
        playerRenderer = GetComponent<Renderer>(); // Hole den Renderer des Spielers
        playerCollider = GetComponent<Collider2D>(); // Hole den Collider des Spielers
        mainCamera = Camera.main; // Hole die Hauptkamera
    }

    // Funktion, um dem Spieler Schaden zuzufügen
    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        Debug.Log("Spieler nimmt " + damageAmount + " Schaden. Aktuelle Gesundheit: " + currentHealth);
        if (healthUi != null)
        {
            healthUi.GetDamage();
        }
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Spieler stirbt
    void Die()
    {
        //Debug.Log("Spieler ist gestorben.");

        //// Teleportiere den Spieler außerhalb des Sichtbereichs
        //transform.position = new Vector3(1000, 1000, 0); // Ändere die Position zum Verschwinden
        //playerCollider.enabled = false; // Deaktiviert den Collider, um Angriffe zu verhindern

        //// Fixiere die Kamera an der aktuellen Spielerposition
        //FixCameraAtPlayerPosition(transform.position);

        //// Lade die nächste Szene
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Lade die Szene, die in sceneToLoad angegeben ist
        //if(Safepoint.GetCurrentSafepoint() != null) 
        //{
        //    SafepointObject.LoadCurrentSafepoint();
        //}

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        //else 
        //{
            //SceneManager.LoadScene(SceneManager.GetSceneByBuildIndex(FIRST_LEVEL_BUILD_INDEX).name);
        //}
    }

    // Spieler regeneriert Gesundheit nach einem Gegner-Kill
    public void RegenerateHealthOnKill()
    {
        currentHealth += healthRegenOnKill;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth; // Gesundheit kann nicht über das Maximum hinaus gehen
        }
        Debug.Log("Spieler regeneriert " + healthRegenOnKill + " Gesundheit. Aktuelle Gesundheit: " + currentHealth);
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
