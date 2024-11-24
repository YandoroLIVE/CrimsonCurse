using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement; // Für Szenenwechsel

public class LevelEndTrigger : MonoBehaviour
{
    [SerializeField] private Animator transitionAnim;
    public string nextSceneName;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Prüfen, ob der Spieler das Levelende erreicht
        if (other.CompareTag("Player"))
        {
            StartCoroutine(LoadNextLevel());
        }
    }

    IEnumerator LoadNextLevel()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            transitionAnim.SetTrigger("End");
            yield return new WaitForSeconds(1f);
            SceneManager.LoadScene(nextSceneName);
            transitionAnim.SetTrigger("Start");
        }
        else
        {
            Debug.LogWarning("Ke in Szenenname angegeben!");
        }
    }
}
