using UnityEngine;
using UnityEngine.SceneManagement;


public class EscapeToMainMenu : MonoBehaviour
{
    // Name der Hauptmen�-Szene
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    void Update()
    {
        // �berpr�fe, ob die Escape-Taste gedr�ckt wurde
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Escape gedr�ckt �ber Input Manager!");
            LoadMainMenu();
        }
    }

    void LoadMainMenu()
    {
        // �berpr�fe, ob die Szene existiert
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
