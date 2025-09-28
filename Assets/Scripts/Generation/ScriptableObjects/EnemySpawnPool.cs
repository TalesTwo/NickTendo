using UnityEngine;

namespace Generation.ScriptableObjects
{
    /// <summary>
    /// Breakdown of room generation data for procedural generation.
    /// Note: this is a scriptable Object used to store a single rooms info.
    /// roomPrefab: The prefab to use for this room.
    /// roomType: The type of room (e.g., Spawn, Enemy, Treasure).
    /// roomWidth: The width of the room in grid units.
    /// roomHeight: The height of the room in grid units.
    /// </summary>
    [CreateAssetMenu(fileName = "Spawning", menuName = "ScriptableObjects/EnemySpawnData", order = 1)]
    public class EnemySpawnPool : ScriptableObject
    {
        public GameObject enemyPrefab;
        public float spawnRate = 1.0f;
    }
}