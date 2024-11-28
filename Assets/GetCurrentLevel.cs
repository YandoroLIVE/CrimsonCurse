using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GetCurrentLevel : MonoBehaviour
{
    public TextMeshProUGUI sceneNameText;

    void Start()
    {
        // Szenennamen abfragen
        string currentSceneName = SceneManager.GetActiveScene().name;

        // Text aktualisieren
        if (sceneNameText != null)
        {
            sceneNameText.text = "Scene: " + currentSceneName;
        }
    }
}