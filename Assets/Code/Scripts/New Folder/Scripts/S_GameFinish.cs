using UnityEngine;
using UnityEngine.SceneManagement;

public class S_GameFinish : MonoBehaviour
{
    [SerializeField] private string sceneToLoad = "NextScene"; // Name der Szene, die geladen werden soll

    private void Start()
    {
        Debug.Log("S_GameFinish ist aktiv und wartet auf Trigger-Ereignisse.");
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Trigger betreten: {other.gameObject.name}");

        // Überprüfen, ob der Spieler den Trigger betreten hat
        if (other.CompareTag("Player"))
        {
            Debug.Log("Der Spieler hat den Trigger betreten. Szene wird gewechselt: " + sceneToLoad);
            SceneManager.LoadScene(sceneToLoad); // Szene wechseln
        }
    }
}
