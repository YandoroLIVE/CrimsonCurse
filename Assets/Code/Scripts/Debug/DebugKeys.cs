using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugKeys : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void Update()
    {
        RestartLevel();
    }

    void RestartLevel()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            // Aktuelle Szene neu laden
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }


}
