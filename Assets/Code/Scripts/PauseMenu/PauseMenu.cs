
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject evenSystem;
    private static PauseMenu _Instance;
    private bool inMenu = false;
    public GameObject pauseMenuUI; // Reference to the Pause Menu UI
    private bool isPaused = false;

    private void Awake()
    {
        
        if (_Instance == null) 
        {
            _Instance = this;
            DontDestroyOnLoad(this);
            Resume();
            SceneManager.activeSceneChanged += OnNewSceneLoaded;
            
        }
        else if(_Instance != this) 
        {
            Destroy(gameObject);
        }
        else { Resume(); };


        
    }
    private void OnNewSceneLoaded(Scene current, Scene next)
    {
        if (FindAnyObjectByType<EventSystem>() == null)
        {
            Instantiate(evenSystem);
        }
        if (next.buildIndex == 0)
        {
            inMenu = true;
            Time.timeScale = 1f;
            isPaused = false;
            pauseMenuUI.SetActive(false);
        }
        else 
        {
            inMenu = false;
            Time.timeScale = 1f; // Ensure game time resumes when switching scenes
            isPaused = false;
            pauseMenuUI.SetActive(false);
        }
    }

    void Update()
    {
        // Toggle pause menu on pressing the Escape key
        if (Input.GetKeyDown(KeyCode.Escape) && !inMenu)
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f; // Resume game time
        isPaused = false;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f; // Pause game time
        isPaused = true;
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f; // Ensure game time resumes when switching scenes
        isPaused = false;
        pauseMenuUI.SetActive(false);
        SceneManager.LoadScene("MainMenu"); // Replace with your main menu scene name
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}