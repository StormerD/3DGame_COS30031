using UnityEngine;
using UnityEngine.Pool;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private Transform[] _spawnPoints; // array of spawn point locations
    [SerializeField] private float _timeBetweenSpawns = 5f;
    private float _timeSinceLastSpawn;


    [SerializeField] private Enemy _enemyPrefab;
    private IObjectPool<Enemy> _enemyPool;


    private void Awake()
    {
        _enemyPool = new ObjectPool<Enemy>(CreateEnemy, OnGet, OnRelease);


    }

    private void OnGet(Enemy enemy)
    {
        enemy.gameObject.SetActive(true);
        Transform randomSpawnPoint = _spawnPoints[Random.Range(0, _spawnPoints.Length)];

        enemy.transform.position = randomSpawnPoint.transform.position;
    }

    private void OnRelease(Enemy enemy)
    {
        enemy.gameObject.SetActive(false);
    }

    private Enemy CreateEnemy()
    {
        Enemy enemy = Instantiate(_enemyPrefab);
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