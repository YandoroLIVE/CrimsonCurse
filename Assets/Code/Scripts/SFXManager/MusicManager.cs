using HeroController;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    [SerializeField] private AudioSource soundFXObject;
    [SerializeField] private AudioClip forestSong;   // Song für den Forest-Level
    [SerializeField] private AudioClip defaultSong;  // Standard-Song für andere Levels
    [SerializeField] private AudioClip bossSong;

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
        SetMusicForLevel();
       
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        bool newIsForestLevel = GetPlayer();

        if (newIsForestLevel != isCurrentForestLevel)
        {
            SetMusicForLevel();
            isCurrentForestLevel = newIsForestLevel;
        }
        SetMusicForBoss();
    }

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
        var playerController = S_PlayerHealth.GetInstance()?.GetComponent<PlayerController>();

        if (playerController != null)
        {
            return playerController.forrestLevel;
        }
        return false;
    }

    public void SetMusicForBoss()
    {
        if (SceneManager.GetActiveScene().buildIndex == 14)
        {
            soundFXObject.Stop();
            soundFXObject.clip = bossSong;
            soundFXObject.Play();
        }
        else if (S_PlayerHealth.GetInstance() == null) 
        {
            soundFXObject.clip = defaultSong;
            soundFXObject.Play();

        }
    }
}
