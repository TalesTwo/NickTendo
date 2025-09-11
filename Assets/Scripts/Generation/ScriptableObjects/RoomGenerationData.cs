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
    [CreateAssetMenu(fileName = "RoomGenerationData", menuName = "ScriptableObjects/RoomGenerationData", order = 1)]
    public class RoomGenerationData : ScriptableObject
    {
        public GameObject roomPrefab;
        public int roomWidth = 16;
        public int roomHeight = 16;
        // more if we ever need it can go here
    }
}
