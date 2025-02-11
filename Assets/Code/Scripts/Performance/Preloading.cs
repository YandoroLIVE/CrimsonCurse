using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Preloading : MonoBehaviour
{
    [SerializeField] private List<string> scenes;
    [SerializeField] private string firstScene;
    private AsyncOperation sceneToPreload;
    private float actualProgress;
    void Start()
    {
        StartCoroutine(LoadScenes());    
    }

    IEnumerator LoadScenes() 
    {
        float progressCurrentScene = 0f;
        for (int i = 0; i < scenes.Count; i++) 
        {
            sceneToPreload = SceneManager.LoadSceneAsync(scenes[i],LoadSceneMode.Additive);
            while (!sceneToPreload.isDone) 
            {
                progressCurrentScene += sceneToPreload.progress;
                yield return null;
            }
            sceneToPreload = SceneManager.UnloadSceneAsync(scenes[i]);
            while (!sceneToPreload.isDone)
            {
                progressCurrentScene += sceneToPreload.progress;
                yield return null;
            }

        }
        SceneManager.LoadScene(firstScene);
    }
}
