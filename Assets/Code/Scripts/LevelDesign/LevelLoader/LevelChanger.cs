
    using UnityEngine;

    public class LevelChanger : MonoBehaviour
    {
        [SerializeField] private LevelConnection _connection;

        [SerializeField] private string _targetSceneName;
        [SerializeField] private Transform _spawnPoint;
    }