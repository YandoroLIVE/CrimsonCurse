using UnityEngine;
using UnityEngine.SceneManagement;


public class LoadAScene : MonoBehaviour
{
    public string targetScene;

    public void LoadScene() 
    {
        SceneManager.LoadScene(targetScene);
        Debug.Log(SceneManager.GetActiveScene().name);
    }
}
