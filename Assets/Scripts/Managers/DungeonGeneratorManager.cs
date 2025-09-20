using System;
using System.Collections.Generic;
using Generation.ScriptableObjects;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    
    
    // 2D representation of the dungeon layout

    
    
    
    public class DungeonGeneratorManager : Singleton<DungeonGeneratorManager>
    {
        private Room CurrentRoom;
        public GenerationData generationData;

        void Start()
        {
            DebugUtils.LogSuccess("Successfully started the Dungeon Generator Manager.");
            // pick a random Spawn room to start with
            GenerateDungeonLayout();
            
        }
        
        
        void Update()
        {
            // if we press G
            if (Input.GetKeyDown(KeyCode.G))
            {
                if (CurrentRoom )
                {
                    DebugUtils.Log($"Current Room Type: {CurrentRoom.GetRoomType()}");
                } else
                {
                    DebugUtils.LogWarning("Current Room is null.");
                }
            }

        }

        
        private void GenerateDungeonLayout()
        {
            int randomIndex = UnityEngine.Random.Range(0, generationData.RoomDict[Types.RoomType.Spawn].Count);
            SceneField sceneField = generationData.RoomDict[Types.RoomType.Spawn][randomIndex];

            // Subscribe to sceneLoaded so we can grab the room after Unity finishes loading
            SceneManager.sceneLoaded += OnSceneLoaded;

            SceneManager.LoadScene(sceneField.SceneName);
            
            // there is more i need to do here, but i need to ensure that the room is fully loaded first
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Unsubscribe so this only runs once
            SceneManager.sceneLoaded -= OnSceneLoaded;

            GameObject[] rootObjects = scene.GetRootGameObjects();
            foreach (GameObject obj in rootObjects)
            {
                Room roomComponent = obj.GetComponent<Room>();
                if (roomComponent != null)
                {
                    DebugUtils.LogSuccess($"Spawn room of type {roomComponent.GetRoomType()} loaded successfully.");
                    CurrentRoom = roomComponent;
                }
            }
            
        }
        


        /// <summary>
        /// Picks a random room of a given type and loads it additively.
        /// </summary>
        private static void GenerateRoomFromType(Types.RoomType roomType)
        {
            if (!Instance.generationData.RoomDict.TryGetValue(roomType, out List<SceneField> possibleScenes) || possibleScenes.Count == 0)
            {
                DebugUtils.LogError($"No room scenes found for type {roomType}");
                return;
            }

            int randomIndex = UnityEngine.Random.Range(0, possibleScenes.Count);
            //GenerateRoomFromScene(possibleScenes[randomIndex]);
        }
    }
}