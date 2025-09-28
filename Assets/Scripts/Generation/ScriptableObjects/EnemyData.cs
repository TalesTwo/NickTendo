using UnityEngine;

namespace Generation.ScriptableObjects
{
    /// <summary>
    /// Information about an enemy type that can be spawned in a room.
    /// enemyPrefab: The prefab of the enemy to spawn.
    /// minSpawnCount: The minimum number of this enemy type to spawn in a room.
    /// maxSpawnCount: The maximum number of this enemy type to spawn in a room.
    /// spawnChance: The chance (0 to 1) that this enemy data will be considered for spawning.
    /// </summary>
    [CreateAssetMenu(fileName = "Spawning", menuName = "ScriptableObjects/EnemyData", order = 1)]
    public class EnemyData : ScriptableObject
    {
        [Header("Prefab Reference")]
        public GameObject enemyPrefab;
        
        [Header("Spawn Count")]
        public int minSpawnCount = 1;
        public int maxSpawnCount = 1;
        
        [Header("Spawn Chance / Weight")]
        [Range(0f, 1f)] public float spawnChance = 1f;
    }
}