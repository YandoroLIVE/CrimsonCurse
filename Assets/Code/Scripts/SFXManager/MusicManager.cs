using HeroController;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    [SerializeField] private AudioSource soundFXObject;
    [SerializeField] private AudioClip forestSong;   // Song für den Forest-Level
    [SerializeField] private AudioClip defaultSong;  // Standard-Song für andere Levels

    private bool isCurrentForestLevel;  // Speichert den aktuellen Leveltyp

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        // Szenenwechsel überwachen
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        // Initialer Musikcheck beim Start des Spiels
        SetMusicForLevel();
    }

    private void OnDestroy()
    {
        // Event-Listener beim Zerstören des Objekts entfernen
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Diese Methode wird nach jedem Szenenwechsel aufgerufen
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        bool newIsForestLevel = GetPlayer();

        // Musik nur ändern, wenn sich der Leveltyp geändert hat
        if (newIsForestLevel != isCurrentForestLevel)
        {
            SetMusicForLevel();
            isCurrentForestLevel = newIsForestLevel; // Aktuellen Leveltyp speichern
        }
    }

    // Musik basierend auf dem Leveltyp setzen
    private void SetMusicForLevel()
    {
        bool isForestLevel = GetPlayer();

        if (isForestLevel)
        {
            soundFXObject.clip = forestSong;
        }
        else
        {
            soundFXObject.clip = defaultSong;
        }

        soundFXObject.Play();
    }

    bool GetPlayer()
    {
        var playerController = S_PlayerHealth.GetInstance().GetComponent<PlayerController>();
        if (playerController != null)
        {
            return playerController.forrestLevel;
        }
        return false;
    }
}
