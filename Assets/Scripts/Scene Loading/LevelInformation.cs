using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelInformation : MonoBehaviour
{
    public int levelNumber;
    public List<SpawnLocation> spawnLocations;
    private Dictionary<int, Vector2> _spawnLocations = new();
    public Vector2 defaultSpawn = Vector2.zero;

    void Awake()
    {
        foreach(var s in spawnLocations)
        {
            _spawnLocations.Add(s.fromLevel, s.location);
        }
    }

    public Vector2 GetSpawnPositionComingFrom(int level)
    {
        if (!_spawnLocations.ContainsKey(level))
        {
            Debug.LogWarning("No suitable spawn location found from level: " + level);
            return defaultSpawn;
        }
        else return _spawnLocations[level];
    }
}

[Serializable]
public class SpawnLocation
{
    public int fromLevel;
    public Vector2 location;
}