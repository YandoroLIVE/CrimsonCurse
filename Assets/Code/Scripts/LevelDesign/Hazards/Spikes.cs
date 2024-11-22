using UnityEngine;
using UnityEngine.SceneManagement;

public class Spikes : MonoBehaviour
{
    // Funktion, die aufgerufen wird, wenn der Spieler mit einem Collider in Berührung kommt
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Prüfen, ob das Objekt, das den Trigger berührt, der Spieler ist
        if (collision.CompareTag("Player"))
        {
            // Aktuelle Szene neu laden
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}