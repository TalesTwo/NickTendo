using System;
using System.Collections.Generic;
using Generation.ScriptableObjects;
using UnityEngine;

namespace Managers
{
    public class DungeonGeneratorManager : Singleton<DungeonGeneratorManager>
    {
        // a 2d array to hold the rooms
        List<List<Room>> dungeonRooms = new List<List<Room>>();
        
        [Header("Generation Data")]
        [SerializeField] private  int rows = 5;
        [SerializeField] private  int cols = 5;
        public GenerationData generationData;
        [Space(10f)]
        [Header("Start/End Positions (Row, Col)")]
        [Header("If set to -1, -1, will randomize")]
        [SerializeField] private Vector2Int startPos = new Vector2Int(-1, -1);
        [SerializeField] private Vector2Int endPos = new Vector2Int(-1, -1);

        
        void Start()
        {
            InitializeDungeonGrid(rows, cols);
            DungeonGeneration();
        }

        // Update is called once per frame
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.H))
            {
                DebugPrintDungeonLayout();
            }
        }



        private void DungeonGeneration()
        {
            // Spawn in a random SpawnRoom at the start position. If startPos is -1, -1, randomize it
            // Note: random spawns will always be on the Bottom row
            if (startPos.x == -1 && startPos.y == -1)
            {
                int randomCol = UnityEngine.Random.Range(0, cols);
                startPos = new Vector2Int(rows - 1, randomCol);
            }

            Vector3 startPosition = new Vector3(startPos.y * 20, 0, startPos.x * 20);
            Room startRoom = GenerateRoomFromType(Types.RoomType.Spawn, startPosition);

            // Make sure dungeonRooms has been initialized
            if (dungeonRooms[startPos.x][startPos.y] == null)
            {
                dungeonRooms[startPos.x][startPos.y] = startRoom;
            }
            else
            {
                Debug.LogWarning("Start room position already occupied!");
            }
        }

        
        private void InitializeDungeonGrid(int rows, int cols)
        {
            dungeonRooms.Clear();
            for (int r = 0; r < rows; r++)
            {
                List<Room> row = new List<Room>();
                for (int c = 0; c < cols; c++)
                {
                    row.Add(null); // Initialize with nulls
                }
                dungeonRooms.Add(row);
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


        private static void BuildRoomAtCords(int row, int col)
        {
            // this function will check what its required doors are, and then build a room that fits those requirements
        }
        
        
        private void DebugPrintDungeonLayout()
        {
            DebugUtils.ClearConsole();
            if (dungeonRooms == null || dungeonRooms.Count == 0)
            {
                DebugUtils.Log("Dungeon is empty.");
                return;
            }

            int rowCount = dungeonRooms.Count;
            int colCount = dungeonRooms[0].Count;

            string layout = "\n DUNGEON LAYOUT\n  ";

            // Print column headers
            for (int c = 0; c < colCount; c++)
            {
                layout += c.ToString().PadLeft(6); // adjust spacing
            }
            layout += "\n";

            // Print each row
            for (int r = 0; r < rowCount; r++)
            {
                layout += r.ToString().PadLeft(2); // row header

                for (int c = 0; c < colCount; c++)
                {
                    if (dungeonRooms[r][c] != null)
                    {
                        int doorCount = dungeonRooms[r][c].GetActiveDoorCount();
                        layout += $"[{doorCount}]".PadLeft(6); // keep spacing consistent
                    }
                    else
                    {
                        layout += "[]".PadLeft(6);
                    }
                }

                layout += "\n";
            }

            DebugUtils.Log(layout);
        }

        
    }
}