using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace HeroController
{
    public class LevelChanger : MonoBehaviour
    {
        [SerializeField] private LevelConnection _connection;
        [SerializeField] private string _targetSceneName;
        [SerializeField] private Transform _spawnPoint;

        private void Awake()
        {
            if (_connection == LevelConnection.ActiveConnection && !SafepointObject._Respawned)
            {
                FindObjectOfType<PlayerController>().transform.position = _spawnPoint.position;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                StartCoroutine(ChangeLevelAfterDelay(0.6f));
            }
            else
            {
                Debug.Log("Non-player object entered trigger");
            }
        }

        private IEnumerator ChangeLevelAfterDelay(float delay)
        {
            SceneFader myScript = FindObjectOfType<SceneFader>();
            myScript.FadeOut(myScript.CurrentFadeType);

            Debug.Log("Player entered trigger");

            yield return new WaitForSeconds(delay);

            LevelConnection.ActiveConnection = _connection;
            SceneManager.LoadScene(_targetSceneName);
        }
    }
}
