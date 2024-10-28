using UnityEngine;
using UnityEngine.SceneManagement; // Zum Laden der Szenen

public class TriggerToMainMenu : MonoBehaviour
{
    // Die Methode wird aufgerufen, wenn der Spieler den Trigger betritt
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Überprüfen, ob das Objekt, das den Trigger betritt, der Spieler ist
        if (collision.CompareTag("Player"))
        {
            // Lädt die Szene "MainMenu". Der Szenenname muss korrekt gesetzt sein.
            SceneManager.LoadScene("MainMenu");
        }
    }
}