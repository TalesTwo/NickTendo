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
        public GenerationData generationData;

        void Start()
        {
            DebugUtils.LogSuccess("Successfully started the Dungeon Generator Manager.");
            // pick a random Spawn room to start with
            
            
        }
        
        void Awake()
        {
            //int randomIndex = UnityEngine.Random.Range(0, generationData.RoomDict[Types.RoomType.Spawn].Count);
            //GenerateRoomFromScene(generationData.RoomDict[Types.RoomType.Spawn][randomIndex]);
        }

        
        
        
        
        


        /// <summary>
        /// Loads a given room scene additively.
        /// </summary>
        private static void GenerateRoomFromScene(SceneField scene)
        {
            if (scene == null || string.IsNullOrEmpty(scene.SceneName))
            {
                DebugUtils.LogError("Tried to generate a room, but scene was null or empty.");
                return;
            }

            DebugUtils.LogSuccess($"Loading room scene: {scene.SceneName}");
            SceneManager.LoadScene(scene, LoadSceneMode.Additive); 
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
            GenerateRoomFromScene(possibleScenes[randomIndex]);
        }
    }
}