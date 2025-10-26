using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class SpawnManager3D : MonoBehaviour
{
    [SerializeField] private Transform[] _spawnPoints; // array of spawn point locations
    [SerializeField] private float _timeBetweenSpawns = 5f;
    [SerializeField] private bool _disabled = false;
    private float _timeSinceLastSpawn;

    [SerializeField] private Enemy3D _enemyPrefab;
    private IObjectPool<Enemy3D> _enemyPool;
    private readonly HashSet<Enemy3D> _activeEnemies = new HashSet<Enemy3D>(); // store currently active enemies in a HashSet

    private void Awake()
    {
        _enemyPool = new ObjectPool<Enemy3D>(CreateEnemy, OnGet, OnRelease);
        _timeSinceLastSpawn = Time.time + _timeBetweenSpawns;
        _disabled = false;
    }

    private void OnGet(Enemy3D enemy)
    {
        Transform randomSpawnPoint = _spawnPoints[Random.Range(0, _spawnPoints.Length)];
        enemy.transform.position = randomSpawnPoint.transform.position;
        enemy.gameObject.SetActive(true);
        _activeEnemies.Add(enemy);
    }

    private void OnRelease(Enemy3D enemy)
    {
        enemy.gameObject.SetActive(false);
        _activeEnemies.Remove(enemy);
    }

    private Enemy3D CreateEnemy()
    {
        Enemy3D enemy = Instantiate(_enemyPrefab);
        enemy.gameObject.SetActive(false);
        enemy.SetPool(_enemyPool);
        return enemy;
    }

    public void DisableSpawner()
    {
        _disabled = true;
    }

    public void StopAndClear(bool destroy = false)
    {
        _disabled = true;
        if (_activeEnemies.Count == 0) return;

        var enemies = new Enemy3D[_activeEnemies.Count];
        _activeEnemies.CopyTo(enemies);
        if (destroy) { // destroying the enemy gameObjects
            foreach (var enemy in enemies)
            {
                if (enemy == null) continue;
                Destroy(enemy.gameObject);
            }
            _activeEnemies.Clear();
        } else // just releasing the enemies back into the pool
        {
            foreach (var enemy in enemies)
            {
                _enemyPool.Release(enemy);
            }
        }
    }

    void Update()
    {
        if (Time.time > _timeSinceLastSpawn && !_disabled)
        {
            _enemyPool.Get();
            _timeSinceLastSpawn = Time.time + _timeBetweenSpawns;
        }
    }
}
