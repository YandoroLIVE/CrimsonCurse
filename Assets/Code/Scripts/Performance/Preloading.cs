using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Preloading : MonoBehaviour
{
    [SerializeField] private List<string> scenes;
    private AsyncOperation sceneToPreload;
    private float progress1;
    private float progress2;
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
                progress1 = (progressCurrentScene / scenes.Count) * 0.5f;
                actualProgress = progress1;
                yield return null;
            }
            sceneToPreload = SceneManager.UnloadSceneAsync(scenes[i]);
            while (!sceneToPreload.isDone)
            {
                progressCurrentScene += sceneToPreload.progress;
                progress2 = (progressCurrentScene / scenes.Count) * 0.5f;
                actualProgress = progress1 + progress2;
                yield return null;
            }

        }
        SceneManager.LoadScene(scenes[0]);
    }
}
