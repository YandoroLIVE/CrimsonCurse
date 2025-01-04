using UnityEngine;
using UnityEngine.SceneManagement;


public class EscapeToMainMenu : MonoBehaviour
{
    // Name der Hauptmenü-Szene
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    static EscapeToMainMenu _instance;


    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }
        else if (_instance != this) 
        {
            Destroy(this);
        }
    }
    void Update()
    {
        // Überprüfe, ob die Escape-Taste gedrückt wurde
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Escape gedrückt über Input Manager!");
            LoadMainMenu();
        }
    }

    void LoadMainMenu()
    {
        // Überprüfe, ob die Szene existiert
        if (!string.IsNullOrEmpty(mainMenuSceneName))
        {
            SceneManager.LoadScene(mainMenuSceneName);
        }
        else
        {
            Debug.LogError("MainMenu Szene-Name ist nicht gesetzt!");
        }
    }
}
