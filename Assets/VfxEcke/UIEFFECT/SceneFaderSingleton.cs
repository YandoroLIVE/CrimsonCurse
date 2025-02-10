using UnityEngine;

public class SceneFaderSingleton : MonoBehaviour
{
    private static SceneFaderSingleton instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
