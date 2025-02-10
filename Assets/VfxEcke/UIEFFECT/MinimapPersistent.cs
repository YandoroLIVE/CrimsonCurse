using UnityEngine;

public class MinimapPersistent : MonoBehaviour
{
    private static MinimapPersistent instance;

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
