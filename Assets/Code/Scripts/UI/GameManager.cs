using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton-Instanz
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        // �berpr�fen, ob bereits eine Instanz existiert
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // L�schen, wenn eine andere Instanz vorhanden ist
            return;
        }

        // Diese Instanz als Singleton setzen
        Instance = this;

        // Dieses Objekt nicht zerst�ren, wenn die Szene gewechselt wird
        DontDestroyOnLoad(gameObject);

        // Optionale Initialisierungslogik
        InitializeGame();
    }

    private void InitializeGame()
    {
        // Hier kannst du alle Initialisierungen vornehmen, die der GameManager braucht
        Debug.Log("GameManager initialized.");
    }

    // Beispiel f�r eine Funktion im GameManager
    public void SetPlayerName(string name)
    {
        Debug.Log("Player Name Set: " + name);
    }
}