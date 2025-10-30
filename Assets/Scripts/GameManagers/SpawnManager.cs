using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private IntEventObject _freezeStream;
    [SerializeField] private Transform[] _spawnPoints; // array of spawn point locations
    private Transform _nextSpawnPoint; // chosen spawn for next enemy
    private float _spawnRadius = 15f;
    private Transform _player;

    [SerializeField] private float _timeBetweenSpawns = 5f;
    [SerializeField] private bool _disabled = false;
    private float _timeSinceLastSpawn;

    [SerializeField] private Enemy _enemyPrefab;
    private IObjectPool<Enemy> _enemyPool;
    private readonly HashSet<Enemy> _activeEnemies = new HashSet<Enemy>(); // store currently active enemies in a HashSet

    private void Awake()
    {
        if (_spawnPoints == null || _spawnPoints.Length == 0)
            PopulateSpawnPoints();

        _enemyPool = new ObjectPool<Enemy>(CreateEnemy, OnGet, OnRelease);
        _timeSinceLastSpawn = Time.time + _timeBetweenSpawns;
        _disabled = false;
        if (_player == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p) _player = p.transform;
        }
    }

    private void PopulateSpawnPoints()
    {
        int n = transform.childCount;
        _spawnPoints = n == 0 ? System.Array.Empty<Transform>() : new Transform[n];
        for (int i = 0; i < n; i++)
            _spawnPoints[i] = transform.GetChild(i);
    }

    void OnEnable()
    {
        _freezeStream.RegisterListener(FreezeEvent);
    }

    void OnDisable()
    {
        _freezeStream.UnregisterListener(FreezeEvent);
    }

    private void FreezeEvent(int state)
    {
        if (state == 0) UnfreezeActions();
        else FreezeActions();
    }

    public void FreezeActions()
    {
        _disabled = true;
    }

    public void UnfreezeActions()
    {
        _disabled = false;
    }

    private void OnGet(Enemy enemy)
    {
        Transform spawn = _nextSpawnPoint != null && _nextSpawnPoint ? _nextSpawnPoint : _spawnPoints[Random.Range(0, _spawnPoints.Length)];
        _nextSpawnPoint = null;
        enemy.transform.position = spawn.position;

        enemy.gameObject.SetActive(true);
        _activeEnemies.Add(enemy);
    }

    private void OnRelease(Enemy enemy)
    {
        enemy.gameObject.SetActive(false);
        _activeEnemies.Remove(enemy);
    }

    private Enemy CreateEnemy()
    {
        Enemy enemy = Instantiate(_enemyPrefab);
        enemy.gameObject.SetActive(false);
        enemy.SetPool(_enemyPool);
        return enemy;
    }

    private Transform PickSpawnInRange()
    {
        if (_player == null || _spawnPoints == null || _spawnPoints.Length == 0) return null;
        float r2 = _spawnRadius * _spawnRadius;
        var candidates = new List<Transform>();
        foreach (var spawn in _spawnPoints)
        {
            if (spawn == null) continue;
            if ((spawn.position - _player.position).sqrMagnitude <= r2)
                candidates.Add(spawn);
        }
        if (candidates.Count == 0) return null;
        return candidates[Random.Range(0, candidates.Count)];
    }

    public void DisableSpawner()
    {
        _disabled = true;
    }

    public void StopAndClear(bool destroy = false)
    {
        _disabled = true;
        if (_activeEnemies.Count == 0) return;

        var enemies = new Enemy[_activeEnemies.Count];
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
            var spawn = PickSpawnInRange();
            if (spawn != null)
            {
                _nextSpawnPoint = spawn;
                _enemyPool.Get();
                _timeSinceLastSpawn = Time.time + _timeBetweenSpawns;
            }
        }
    }
}