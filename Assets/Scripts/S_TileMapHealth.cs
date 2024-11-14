using UnityEngine;
using UnityEngine.Tilemaps;

public class S_TileMapHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    private int currentHealth;

    private Tilemap tilemap;

    private void Start()
    {
        // Initialisiert die aktuelle Gesundheit und die Tilemap-Komponente
        currentHealth = maxHealth;
        tilemap = GetComponent<Tilemap>();

        if (tilemap == null)
        {
            Debug.LogError("Keine Tilemap gefunden.");
        }
    }

    // Diese Methode kann von externen Skripten aufgerufen werden, um Schaden zu verursachen
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Tilemap getroffen! Aktuelle HP: " + currentHealth);

        // Überprüft, ob die Gesundheit auf 0 oder darunter gesunken ist
        if (currentHealth <= 0)
        {
            DestroyTilemap();
        }
    }

    private void DestroyTilemap()
    {
        // Entfernt die gesamte Tilemap und zerstört das GameObject
        if (tilemap != null)
        {
            tilemap.ClearAllTiles();
            Destroy(gameObject);
            Debug.Log("Tilemap wurde zerstört!");
        }
    }
}
