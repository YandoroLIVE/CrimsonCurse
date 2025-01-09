using UnityEngine;
using UnityEngine.SceneManagement;

public class Spikes : MonoBehaviour
{
    private const int FIRST_LEVEL_BUILD_INDEX = 1;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Return to safepoint
        if (Safepoint.GetCurrentSafepoint() != null)
        {
            SafepointObject.LoadCurrentSafepoint();
        }

        else
        {
            SceneManager.LoadScene(SceneManager.GetSceneByBuildIndex(FIRST_LEVEL_BUILD_INDEX).name);
        }
    }
}