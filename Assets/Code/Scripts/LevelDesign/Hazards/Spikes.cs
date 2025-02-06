using UnityEngine;
using UnityEngine.SceneManagement;

public class Spikes : MonoBehaviour
{
    [SerializeField] private float damage = 5000;
    private const string FIRST_LEVEL_NAME = "L1_Redo_Nika";
    private void OnTriggerEnter2D(Collider2D collision)
    {
        S_PlayerHealth.GetInstance().TakeDamage(damage);
        //Return to safepoint
        //if (Safepoint.GetCurrentSafepoint() != null)
        //{
        //    SafepointObject.LoadCurrentSafepoint();
        //}

        //else
        //{
        //    SceneManager.LoadScene(FIRST_LEVEL_NAME);
        //}
    }
}