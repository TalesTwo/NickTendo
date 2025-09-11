using System;
using System.Collections.Generic;
using Generation.ScriptableObjects;
using UnityEngine;

namespace Managers
{
    public class DungeonGeneratorManager : Singleton<DungeonGeneratorManager>
    {
        
        public GenerationData generationData;
        
        void Start()
        {
            DebugUtils.LogSuccess("Successfully started.");
        }

        // Update is called once per frame
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.G))
            {
                // pick a random spawn room from the list
                //GenerateRoomFromType(Types.RoomType.Spawn, Vector3.zero);
                GenerateRoomFromClass(generationData.RoomDict[Types.RoomType.Spawn][0], Vector3.zero);
            }
        }

        
        private static Room GenerateRoomFromClass(Room roomPrefab, Vector3 position)
        {
            if (roomPrefab == null)
            {
                DebugUtils.LogError("Tried to generate a room, but prefab was null.");
                return null;
            }

            Room roomInstance = Instantiate(roomPrefab, position, Quaternion.identity);
            roomInstance.InitializeRoom();
            return roomInstance;
        }

        
        private static Room GenerateRoomFromType(Types.RoomType roomType, Vector3 position)
        {
            if (!Instance.generationData.RoomDict.TryGetValue(roomType, out List<Room> possibleRooms) || possibleRooms.Count == 0)
            {
                DebugUtils.LogError($"No room prefabs found for type {roomType}");
                return null;
            }

            int randomIndex = UnityEngine.Random.Range(0, possibleRooms.Count);
            return GenerateRoomFromClass(possibleRooms[randomIndex], position);
        }
        
    }
}
