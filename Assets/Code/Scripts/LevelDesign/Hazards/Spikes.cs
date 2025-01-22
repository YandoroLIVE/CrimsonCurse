using UnityEngine;
using UnityEngine.SceneManagement;

public class Spikes : MonoBehaviour
{
    private const string FIRST_LEVEL_NAME = "L1_Redo_Nika";
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Return to safepoint
        if (Safepoint.GetCurrentSafepoint() != null)
        {
            SafepointObject.LoadCurrentSafepoint();
        }

        else
        {
            SceneManager.LoadScene(FIRST_LEVEL_NAME);
        }
    }
}