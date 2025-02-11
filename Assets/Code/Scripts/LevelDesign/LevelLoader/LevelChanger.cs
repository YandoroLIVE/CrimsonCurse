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
                S_PlayerHealth.GetInstance().transform.position = _spawnPoint.position;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                StartCoroutine(ChangeLevelAfterDelay(0.6f));
            }
        }

        private IEnumerator ChangeLevelAfterDelay(float delay)
        {
            SceneFader myScript = FindAnyObjectByType<SceneFader>();

            if (myScript != null)
            {
                myScript.FadeOut(myScript.CurrentFadeType);
                yield return new WaitForSeconds(delay);
            }
            LevelConnection.ActiveConnection = _connection;
            SceneManager.LoadScene(_targetSceneName);
            yield return null;
        }
    }
}
