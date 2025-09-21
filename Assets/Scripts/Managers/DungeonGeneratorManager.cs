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
        [SerializeField] private float probabilityToAddOptionalDoor_OneRequiredDoor = 0.75f;
        [SerializeField] private float probabilityToAddOptionalDoor_TwoRequiredDoors = 0.5f;
        [SerializeField] private float probabilityToAddOptionalDoor_ThreeRequiredDoors = 0.25f;
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
            if (Input.GetKeyDown(KeyCode.J))
            {
                Types.DoorConfiguration requiredConnections = GenerateRequiredConnections(startPos.x - 1, startPos.y);
                Debug.Log($"Required Connections for room above start: N:{requiredConnections.NorthDoorActive}, E:{requiredConnections.EastDoorActive}, S:{requiredConnections.SouthDoorActive}, W:{requiredConnections.WestDoorActive}");
                Types.DoorConfiguration optionalConnections = GenerateOptionalConnections(startPos.x - 1, startPos.y);
                Debug.Log($"Optional Connections for room above start: N:{optionalConnections.NorthDoorActive}, E:{optionalConnections.EastDoorActive}, S:{optionalConnections.SouthDoorActive}, W:{optionalConnections.WestDoorActive}");
            }
        }



        private void DungeonGeneration()
        {
            InitializeStartAndEndRoom();
            // Now we move onto the two phase generation
            // Phase 1: Create a "weighted random walk" from the start room to the end room
            // if we reach the row -1 before the end room, we will force the generator to create the optimal path to the end room
            // the end room will always be entered from the south, so the room before it must have a north door
            // the random walk will never move "backward":
            // Directions of movement: North (up), East (right), West (left)
            GeneratePhaseOne();
        }

        private void GeneratePhaseOne()
        {
            // Always start with the room above the start room
            BuildRoomAtCords(startPos.x-1, startPos.y);
            
            // Start the random walk
            
            // move left, right, or up, untill we are row-1
            int currentRow = startPos.x - 1;
            int currentCol = startPos.y;
            // we want to break out of this loop when we reach row 1, since then we build across that row to the end room
            while (currentRow > 1)
            {
                // determine the possible directions we can move
                List<string> possibleDirections = new List<string>();
                // we can always move up
                possibleDirections.Add("Up");
                // we can move left if we are not in the first column
                if (currentCol > 0 && dungeonRooms[currentRow][currentCol - 1] == null)
                {
                    possibleDirections.Add("Left");
                }
                // we can move right if we are not in the last column
                if (currentCol < cols - 1 && dungeonRooms[currentRow][currentCol + 1] == null)
                {
                    possibleDirections.Add("Right");
                }
                // now we randomly select a direction to move
                int randomIndex = UnityEngine.Random.Range(0, possibleDirections.Count);
                string selectedDirection = possibleDirections[randomIndex];
                DebugUtils.Log($"Current Position: ({currentRow}, {currentCol}). Moving {selectedDirection}.");
                switch (selectedDirection)
                {
                    case "Up":
                        currentRow--;
                        break;
                    case "Left":
                        currentCol--;
                        break;
                    case "Right":
                        currentCol++;
                        break;
                }
                // Now build the room at the new position if it doesn't already exist
                if (dungeonRooms[currentRow][currentCol] == null)
                {
                    BuildRoomAtCords(currentRow, currentCol);
                }
            }
            
            // now build rooms directly across to the end room
            while (currentCol != endPos.y)
            {
                if (currentCol < endPos.y)
                {
                    currentCol++;
                }
                else
                {
                    currentCol--;
                }
                if (dungeonRooms[currentRow][currentCol] == null)
                {
                    BuildRoomAtCords(currentRow, currentCol);
                }
            }
        }

        private void InitializeStartAndEndRoom()
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
            // Set up the ending room, with the same logic as above
            if (endPos.x == -1 && endPos.y == -1)
            {
                int randomCol = UnityEngine.Random.Range(0, cols);
                endPos = new Vector2Int(0, randomCol); // End room will always be on the top row
            }
            Vector3 endPosition = new Vector3(endPos.y * 20, 0, endPos.x * 20);
            Room endRoom = GenerateRoomFromType(Types.RoomType.End, endPosition);
            if (dungeonRooms[endPos.x][endPos.y] == null)
            {
                dungeonRooms[endPos.x][endPos.y] = endRoom;
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
            // check all of the adjacent rooms for doors. we will have two different sets of connections
            // REQUIRED connections - doors that must be present (i.e. if the room to the north has a south door, this room must have a north door)
            // OPTIONAL connections - doors that can be present, but are not required (i.e. if the room to the east is empty, this room can have an east door, but it doesn't have to)
            
            // Step1. get the required connections
            Types.DoorConfiguration requiredConnections = GenerateRequiredConnections(row, col);
            // Step2. determine the optional connections
            Types.DoorConfiguration optionalConnections = GenerateOptionalConnections(row, col);
            // Get a count of the total number of doors we currently have
            int requiredDoorCount = requiredConnections.ActiveDoorCount();
            /*
             * The rules for this will be as follows:
             * if we have 1 required door, we will have a 75% chance of adding an optional door
             * if we have 2 required doors, we will have a 50% chance of adding an optional door
             * if we have 3 required doors, we will have a 25% chance of adding an optional door
             *
             * Once we determine the number of doors, we will randomly select from the optional doors that are available
             */
            DebugUtils.Log($"Building room at ({row}, {col}) with {requiredDoorCount} total doors (NOT including optional).");
            
            int numOptionalDoorsToAdd = requiredDoorCount;
            float randomValue = UnityEngine.Random.value; // Random value between 0 and 1
            if (numOptionalDoorsToAdd == 1 && randomValue < Instance.probabilityToAddOptionalDoor_OneRequiredDoor)
            {
                numOptionalDoorsToAdd += 1;
            }
            if (numOptionalDoorsToAdd == 2 && randomValue < Instance.probabilityToAddOptionalDoor_TwoRequiredDoors)
            {
                numOptionalDoorsToAdd += 1;
            }
            if (numOptionalDoorsToAdd == 3 && randomValue < Instance.probabilityToAddOptionalDoor_ThreeRequiredDoors)
            {
                numOptionalDoorsToAdd += 1;
            }
            numOptionalDoorsToAdd -= requiredDoorCount; // we only want the number of optional doors to add
            // Now we have the total number of doors we need, we can start building the door configuration
            // debug print the number of required doors count
            DebugUtils.Log($"Building room at ({row}, {col}) with {numOptionalDoorsToAdd} optional doors.");
            // Randomly select n optional doors to add
            List<Types.DoorClassification> availableOptionalDoors = new List<Types.DoorClassification>();
            if (optionalConnections.NorthDoorActive) availableOptionalDoors.Add(Types.DoorClassification.North);
            if (optionalConnections.EastDoorActive) availableOptionalDoors.Add(Types.DoorClassification.East);
            if (optionalConnections.SouthDoorActive) availableOptionalDoors.Add(Types.DoorClassification.South);
            if (optionalConnections.WestDoorActive) availableOptionalDoors.Add(Types.DoorClassification.West);
            List<Types.DoorClassification> selectedOptionalDoors = new List<Types.DoorClassification>();
            for (int i = 0; i < numOptionalDoorsToAdd && availableOptionalDoors.Count > 0; i++)
            {
                int randomIndex = UnityEngine.Random.Range(0, availableOptionalDoors.Count);
                selectedOptionalDoors.Add(availableOptionalDoors[randomIndex]);
                availableOptionalDoors.RemoveAt(randomIndex); // Ensure we don't pick the same door again
            }
            
            // Now update the requiredConnections to include the selected optional doors
            foreach (var door in selectedOptionalDoors)
            {
                switch (door)
                {
                    case Types.DoorClassification.North:
                        requiredConnections.NorthDoorActive = true;
                        break;
                    case Types.DoorClassification.East:
                        requiredConnections.EastDoorActive = true;
                        break;
                    case Types.DoorClassification.South:
                        requiredConnections.SouthDoorActive = true;
                        break;
                    case Types.DoorClassification.West:
                        requiredConnections.WestDoorActive = true;
                        break;
                }
            }
            
            // FINALLY, we have the final connections that this room needs to have
            DebugUtils.LogSuccess($"Final door configuration for room at ({row}, {col}): N:{requiredConnections.NorthDoorActive}, E:{requiredConnections.EastDoorActive}, S:{requiredConnections.SouthDoorActive}, W:{requiredConnections.WestDoorActive}");
            
            //TODO: Temporarily, just generate a spawn room, and place it inside the grid
            Vector3 position = new Vector3(col * 20, 0, row * 20);
            Room newRoom = GenerateRoomFromType(Types.RoomType.Spawn, position);
            if (newRoom != null)
            {
                Instance.dungeonRooms[row][col] = newRoom;
            }
            
            
        }

        private static Types.DoorConfiguration GenerateRequiredConnections(int row, int col)
        {
            Types.DoorConfiguration requiredConnections;
            // Check the room to the north
            if (row > 0 && Instance.dungeonRooms[row - 1][col] != null)
            {
                requiredConnections.NorthDoorActive = Instance.dungeonRooms[row - 1][col].configuration.SouthDoorActive;
            }
            else
            {
                requiredConnections.NorthDoorActive = false;
            }
            // Check the room to the east
            if (col < Instance.cols - 1 && Instance.dungeonRooms[row][col + 1] != null)
            {
                requiredConnections.EastDoorActive = Instance.dungeonRooms[row][col + 1].configuration.WestDoorActive;
            }
            else
            {
                requiredConnections.EastDoorActive = false;
            }
            // Check the room to the south
            if (row < Instance.rows - 1 && Instance.dungeonRooms[row + 1][col] != null)
            {
                requiredConnections.SouthDoorActive = Instance.dungeonRooms[row + 1][col].configuration.NorthDoorActive;
            }
            else
            {
                requiredConnections.SouthDoorActive = false;
            }
            // Check the room to the west
            if (col > 0 && Instance.dungeonRooms[row][col - 1] != null)
            {
                requiredConnections.WestDoorActive = Instance.dungeonRooms[row][col - 1].configuration.EastDoorActive;
            }
            else
            {
                requiredConnections.WestDoorActive = false;
            }

            return requiredConnections;
        }

        private static Types.DoorConfiguration GenerateOptionalConnections(int row, int col)
        {
            Types.DoorConfiguration optionalConnections;
            // Check the room to the north
            if (row > 0 && Instance.dungeonRooms[row - 1][col] == null)
            {
                optionalConnections.NorthDoorActive = true;
            }
            else
            {
                optionalConnections.NorthDoorActive = false;
            }
            // Check the room to the east
            if (col < Instance.cols - 1 && Instance.dungeonRooms[row][col + 1] == null)
            {
                optionalConnections.EastDoorActive = true;
            }
            else
            {
                optionalConnections.EastDoorActive = false;
            }
            // Check the room to the south
            if (row < Instance.rows - 1 && Instance.dungeonRooms[row + 1][col] == null)
            {
                optionalConnections.SouthDoorActive = true;
            }
            else
            {
                optionalConnections.SouthDoorActive = false;
            }
            // Check the room to the west
            if (col > 0 && Instance.dungeonRooms[row][col - 1] == null)
            {
                optionalConnections.WestDoorActive = true;
            }
            else
            {
                optionalConnections.WestDoorActive = false;
            }

            return optionalConnections;
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
                        // check the type of the room, and see if its the start room
                        if (r == startPos.x && c == startPos.y)
                        {
                            layout += "[S]".PadLeft(6);
                        }
                        else if (r == endPos.x && c == endPos.y)
                        {
                            layout += "[E]".PadLeft(6);
                        }
                        else
                        {
                            layout += $"[{doorCount}]".PadLeft(6);
                        }
                        
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