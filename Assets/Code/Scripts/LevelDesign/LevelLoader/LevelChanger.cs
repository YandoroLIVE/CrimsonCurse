using UnityEngine;
using UnityEngine.SceneManagement;

namespace HeroController
{
    public class LevelChanger : MonoBehaviour
    {
        [SerializeField] private LevelConnection _connection;
        [SerializeField] private string _targetSceneName;
        [SerializeField] private Transform _spawnPoint;

        private void Start()
        {
            if (_connection == LevelConnection.ActiveConnection)
            {
                FindObjectOfType<PlayerControllerNew>().transform.position = _spawnPoint.position;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var player = other.GetComponent<PlayerControllerNew>();
            if (player != null)
            {
                Debug.Log("Player entered trigger");
                LevelConnection.ActiveConnection = _connection;
                SceneManager.LoadScene(_targetSceneName);
            }
            else
            {
                Debug.Log("Non-player object entered trigger");
            }
        }
    }
}