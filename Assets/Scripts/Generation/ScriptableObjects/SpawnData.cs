using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Information about an enemy type that can be spawned in a room.
/// </summary>
[System.Serializable]
public class SpawnData
{
    [Header("Prefab Reference")]
    public GameObject enemyPrefab;

    [Header("Spawn Count")]
    [Min(0)] public int minSpawnCount = 1;
    [Min(1)] public int maxSpawnCount = 3;

    [Header("Spawn Chance / Weight")]
    [Range(0f, 1f)] public float spawnChance = 1f;
}

[System.Serializable]
public class SpawnableGroup
{
    public Types.SpawnableType spawnType;
    public List<SpawnData> Spawnables = new List<SpawnData>(); //TOD): Convert to a spawnable Parent class
}
    

