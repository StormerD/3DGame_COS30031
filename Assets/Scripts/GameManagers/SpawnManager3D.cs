using UnityEngine;
using UnityEngine.Pool;

public class SpawnManager3D : MonoBehaviour
{
    [SerializeField] private Transform[] _spawnPoints; // array of spawn point locations
    [SerializeField] private float _timeBetweenSpawns = 5f;
    private float _timeSinceLastSpawn;

    [SerializeField] private Enemy3D _enemyPrefab;
    private IObjectPool<Enemy3D> _enemyPool;

    private void Awake()
    {
        _enemyPool = new ObjectPool<Enemy3D>(CreateEnemy, OnGet, OnRelease);
    }

    private void OnGet(Enemy3D enemy)
    {
        enemy.gameObject.SetActive(true);
        Transform randomSpawnPoint = _spawnPoints[Random.Range(0, _spawnPoints.Length)];
        enemy.transform.position = randomSpawnPoint.transform.position;
    }

    private void OnRelease(Enemy3D enemy)
    {
        enemy.gameObject.SetActive(false);
    }

    private Enemy3D CreateEnemy()
    {
        Enemy3D enemy = Instantiate(_enemyPrefab);
        enemy.SetPool(_enemyPool);
        return enemy;
    }

    void Update()
    {
        if (Time.time > _timeSinceLastSpawn)
        {
            _enemyPool.Get();
            _timeSinceLastSpawn = Time.time + _timeBetweenSpawns;
        }
    }
}
