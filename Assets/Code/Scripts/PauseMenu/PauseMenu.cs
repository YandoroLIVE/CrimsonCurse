
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject evenSystem;
    private PauseMenu _instance;
    private bool inMenu = false;
    public GameObject pauseMenuUI; // Reference to the Pause Menu UI
    private bool isPaused = false;

    private void Awake()
    {
        
        if (_instance == null) 
        {
            _instance = this;
            DontDestroyOnLoad(this);
            Resume();
            SceneManager.activeSceneChanged += OnNewSceneLoaded;
            
        }
        else if(_instance != this) 
        {
            Destroy(this.gameObject);
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
            this.enabled = false;
            Time.timeScale = 1f;
            isPaused = false;
            pauseMenuUI.SetActive(false);
        }
        else 
        {
            this.enabled = true;
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