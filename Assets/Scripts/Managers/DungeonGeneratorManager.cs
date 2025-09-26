using System;
using System.Collections;
using System.Collections.Generic;
using Generation.ScriptableObjects;
using UnityEngine;

/*
 * NOTE TO SELF:
 * pick the door in the direction we want to travel, and then add in the rooms, to guarantee that we always have a door to travel through
 */

/*
 * Note to self, we need to artificially move foreward, get our required cnnection, and another required based on the direction of travel,
 * and then build the room
 */

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
        [SerializeField] private Vector2Int _startPos = new Vector2Int(-1, -1);
        [SerializeField] private Vector2Int _endPos = new Vector2Int(-1, -1);
        
        private Vector2Int startPos = new Vector2Int(-1, -1);
        private Vector2Int endPos = new Vector2Int(-1, -1);
        
        [SerializeField] private int Seed = 16; // for future use, if we want to have seeded generation
        private float waitTime = 2f;
        
        // list of cords where we need to PCG
        private List<(int row, int col)> alteredRoomCoords = new List<(int row, int col)>();
        
        private bool bPhaseOneComplete = false;
        
        void Start()
        {
            //InitializeDungeonGrid(rows, cols);
            // create a function, that will be called every n seconds, to initalize and print a new dungeon
            //StartCoroutine(WaitAndGenerateDungeon());
            
        }
        private IEnumerator WaitAndGenerateDungeon()
        {
            while (true)
            {
                // wait for n seconds
                yield return new WaitForSeconds(waitTime);

                // only run if not paused
   
                InitializeDungeonGrid(rows, cols);
                DungeonGeneration();
            
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                InitializeDungeonGrid(rows, cols);
                DungeonGeneration();
            }
            
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
            if (Seed != 0)
            {
                UnityEngine.Random.InitState(Seed);
            }
            
            InitializeStartAndEndRoom();
            // Now we move onto the two phase generation
            // Phase 1: Create a "weighted random walk" from the start room to the end room
            // if we reach the row -1 before the end room, we will force the generator to create the optimal path to the end room
            // the end room will always be entered from the south, so the room before it must have a north door
            // the random walk will never move "backward":
            // Directions of movement: North (up), East (right), West (left)
            GeneratePhaseOne();
            bPhaseOneComplete = true;
            // build a room directly below the end room
            //BuildRoomAtCords(endPos.x + 1, endPos.y, new Types.DoorConfiguration(false, false, true, false));
            
            // Temporarily, we will hardcode phase 2 to ensure it works
            // we will do this on row 1, col 0

            //Phase 2: Loop through the alteredRoomCoords, and PCG from each of those rooms
            /*
            foreach (var (row, col) in alteredRoomCoords)
            {
                Room currentRoom = dungeonRooms[row][col];
                PCG(dungeonRooms, currentRoom, row, col);
            }
            */
            // maybe instead, we just loop through every room in the dungeon, and PCG from there, as long as it aint null
            for (int r = 0; r < dungeonRooms.Count; r++)
            {
                for (int c = 0; c < dungeonRooms[r].Count; c++)
                {
                    Room currentRoom = dungeonRooms[r][c];
                    if (currentRoom != null)
                    {
                        PCG(dungeonRooms, currentRoom, r, c);
                    }
                }
            }
            
            DebugPrintDungeonLayout();
        }
        
        
        private void PCG(List<List<Room>> dungeonMap, Room currentRoom, int currentRow, int currentCol)
        {
            // This will be the recursive depth first search algorithm to generate the dungeon
            // prevent infinite recursion
            if (currentRoom == null || currentRoom.bIsFinalized) { return;}
            // mark as visited
            currentRoom.bIsFinalized= true;
            DebugUtils.Log($"PCG: Visiting room at ({currentRow}, {currentCol})");
            
            // ensure we are within bounds
            if (currentRow < 0 || currentCol < 0 || currentRow >= dungeonMap.Count || currentCol >= dungeonMap[currentRow].Count) return;
            
            // Get access to the connections of the current room
            Types.DoorConfiguration connections = currentRoom.configuration;
            
            // we will go in a north, east, south, west order
            
            // North
            if(connections.NorthDoorActive && currentRow - 1 >= 0)
            {
                // get the room to the north
                Room northRoom = dungeonMap[currentRow - 1][currentCol];
                
                // ensure the room has not been visited
                if (northRoom == null)
                {
                    // PCG a new room at this location
                    BuildRoomAtCords(currentRow - 1, currentCol, new Types.DoorConfiguration(false, false, false, false));
                    northRoom = dungeonMap[currentRow - 1][currentCol]; 
                    PCG(dungeonMap, northRoom, currentRow - 1, currentCol);
                }
            }
            
            // east
            if(connections.EastDoorActive && currentCol + 1 < dungeonMap[currentRow].Count)
            {
                // get the room to the east
                Room eastRoom = dungeonMap[currentRow][currentCol + 1];
                // ensure the room has not been visited
                if (eastRoom == null)
                {
                    // PCG a new room at this location
                    BuildRoomAtCords(currentRow, currentCol + 1, new Types.DoorConfiguration(false, false, false, false));
                    eastRoom = dungeonMap[currentRow][currentCol + 1];
                    PCG(dungeonMap, eastRoom, currentRow, currentCol + 1);
                }
            }
            
            // South
            if(connections.SouthDoorActive && currentRow + 1 < dungeonMap.Count)
            {
                // get the room to the south
                Room southRoom = dungeonMap[currentRow + 1][currentCol];
                // ensure the room has not been visited
                if (southRoom == null )
                {
                    // PCG a new room at this location
                    BuildRoomAtCords(currentRow + 1, currentCol, new Types.DoorConfiguration(false, false, false, false));
                    southRoom = dungeonMap[currentRow + 1][currentCol];
                    PCG(dungeonMap, southRoom, currentRow + 1, currentCol);
                }
            }
            
            // West
            if(connections.WestDoorActive && currentCol - 1 >= 0)
            {
                // get the room to the west
                Room westRoom = dungeonMap[currentRow][currentCol - 1];
                // ensure the room has not been visited
                if (westRoom == null)
                {
                    // PCG a new room at this location
                    BuildRoomAtCords(currentRow, currentCol - 1, new Types.DoorConfiguration(false, false, false, false));
                    westRoom = dungeonMap[currentRow][currentCol - 1];
                    PCG(dungeonMap, westRoom, currentRow, currentCol - 1);
                }
            }
            
        }

        private void GeneratePhaseOne()
        {
            /*
             * WE NEED TO ENSURE THAT THE PROBABILITY TO SPAWN A SECOND DOOR IS ALWAYS 100% IN THIS PHASE
             */
            
            // Always start with the room above the start room
            //BuildRoomAtCords(startPos.x-1, startPos.y); ///TODO: we might need to move this to after we determine the direction we want to move, to ensure we have a door pointing in the right direction
            
            // Start the random walk
            
            // move left, right, or up, untill we are row-1
            
            int currentRow = startPos.x-1;
            int currentCol = startPos.y;
            // we want to break out of this loop when we reach row 1, since then we build across that row to the end room
            while (currentRow > 1)
            {
                DebugUtils.Log($"Current Position: ({currentRow}, {currentCol})");
                Types.DoorConfiguration AdditionalConnections = new Types.DoorConfiguration(false, false, false, false);
                // determine the possible directions we can move
                List<string> possibleDirections = new List<string>();
                // we can always move up

                possibleDirections.Add("Up");
                
                // we can move left if we are not in the first column and the room to the left is empty AND we have a door pointing in that direction
                if (currentCol > 0 && dungeonRooms[currentRow][currentCol - 1] == null)
                {
                    possibleDirections.Add("Left");
                }
                // we can move right if we are not in the last column
                if (currentCol < cols - 1 && dungeonRooms[currentRow][currentCol + 1] == null)
                {
                    possibleDirections.Add("Right");
                }

                int randomIndex = UnityEngine.Random.Range(0, possibleDirections.Count);
                
                // This needs to be fixed, so that we only move in a direction that we have a door pointing to
                
                string selectedDirection = possibleDirections[randomIndex];
                
                
                switch (selectedDirection)
                {
                    case "Up":
                        AdditionalConnections.NorthDoorActive = true;
                        break;
                    case "Left":
                        AdditionalConnections.WestDoorActive = true; 
                        break;
                    case "Right":
                        AdditionalConnections.EastDoorActive = true;
                        break;
                }
                DebugUtils.Log($"Moving {selectedDirection}.");
                DebugUtils.LogSuccess($"(based on direction of travel) ({currentRow}, {currentCol}): N:{AdditionalConnections.NorthDoorActive}, E:{AdditionalConnections.EastDoorActive}, S:{AdditionalConnections.SouthDoorActive}, W:{AdditionalConnections.WestDoorActive}");
                // Now build the room at the new position if it doesn't already exist
                if (dungeonRooms[currentRow][currentCol] == null)
                {
                    DebugUtils.Log($"Building room at ({currentRow}, {currentCol})");
                    BuildRoomAtCords(currentRow, currentCol, AdditionalConnections);
                }
                
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
                
            }
            
            // now build rooms directly across to the end room
            while (currentCol != endPos.y)
            {
                Types.DoorConfiguration AdditionalConnections = new Types.DoorConfiguration(false, false, false, false);
                if (currentCol < endPos.y)
                {
                    AdditionalConnections.EastDoorActive = true; // we are moving right, so we need a west door
                    //currentCol++;
                }
                else
                {
                    AdditionalConnections.WestDoorActive = true; // we are moving left, so we need an east door
                    //currentCol--;
                }
                if (dungeonRooms[currentRow][currentCol] == null)
                {
                    
                    BuildRoomAtCords(currentRow, currentCol, AdditionalConnections);
                }
                
                // Now we increment or decrement the column
                if(AdditionalConnections.EastDoorActive)
                {
                    currentCol++;
                }
                if(AdditionalConnections.WestDoorActive)
                {
                    currentCol--;
                }
            }
            // no we need to buid the last room, which is the room directly below the end room
            if (dungeonRooms[currentRow][currentCol] == null)
            {
                BuildRoomAtCords(currentRow, currentCol, new Types.DoorConfiguration(false, false, false, false));
            }
        }

        private void InitializeStartAndEndRoom()
        {
            // Spawn in a random SpawnRoom at the start position. If startPos is -1, -1, randomize it
            // Note: random spawns will always be on the Bottom row
            if (_startPos.x == -1 && _startPos.y == -1)
            {
                int randomCol = UnityEngine.Random.Range(0, cols);
                DebugUtils.Log($"Randomized Start Position: ({rows - 1}, {randomCol})");
                startPos = new Vector2Int(rows - 1, randomCol);
            }

            Vector3 startPosition = new Vector3(startPos.y * 20,-startPos.x * 20, 0);
            Room startRoom = GenerateRoomFromType(Types.RoomType.Spawn, startPosition);
            startRoom.SetRoomDifficulty(0);

            // Make sure dungeonRooms has been initialized
            if (dungeonRooms[startPos.x][startPos.y] == null)
            {
                dungeonRooms[startPos.x][startPos.y] = startRoom;
            }
            // Set up the ending room, with the same logic as above
            if (_endPos.x == -1 && _endPos.y == -1)
            {
                int randomCol = UnityEngine.Random.Range(0, cols);
                endPos = new Vector2Int(0, randomCol); // End room will always be on the top row
            }
            Vector3 endPosition = new Vector3(endPos.y * 20,-endPos.x * 20, 0);
            Room endRoom = GenerateRoomFromType(Types.RoomType.End, endPosition);
            
            endRoom.SetRoomDifficulty(CalculateRoomDifficulty((endPos.x, endPos.y)));
            
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
            // I also want to clear all "room" objects from the world
            Room[] existingRooms = FindObjectsOfType<Room>();
            foreach (Room room in existingRooms)
            {
                DestroyImmediate(room.gameObject);
            }
            // destroy all animated buttons
            AnimatedButton[] existingButtons = FindObjectsOfType<AnimatedButton>();
            foreach (AnimatedButton button in existingButtons)
            {
                DestroyImmediate(button.gameObject);
            }
        }
        
        private static Room GenerateRoomFromClass(Room roomPrefab, Vector3 position, int row = -1, int col = -1)
        {
            if (roomPrefab == null)
            {
                DebugUtils.LogError("Tried to generate a room, but prefab was null.");
                return null;
            }

            Room roomInstance = Instantiate(roomPrefab, position, Quaternion.identity);
            // Calculate room difficulty
            // get the room coordinates in the grid
            
            int roomDifficulty = Instance.CalculateRoomDifficulty((row, col));
            roomInstance.InitializeRoom(roomDifficulty, (row, col));
            return roomInstance;
        }
        
        private IEnumerable<(int, int)> GetNeighbors((int row, int col) node)
        {
            int[][] dirs = new int[][]
            {
                new[] { 1, 0 }, new[] { -1, 0 },
                new[] { 0, 1 }, new[] { 0, -1 }
            };

            foreach (var d in dirs)
            {
                int nr = node.row + d[0];
                int nc = node.col + d[1];

                if (nr >= 0 && nr < dungeonRooms.Count &&
                    nc >= 0 && nc < dungeonRooms[0].Count)
                {
                    // only allow through nulls or valid rooms
                    if (dungeonRooms[nr][nc] == null)
                        yield return (nr, nc);
                }
            }
        }
        private int CalculateRoomDifficulty((int row, int col) roomCoords)
        {
            var start = (row: startPos.y, col: startPos.x);
            var goal = roomCoords;

            var visited = new HashSet<(int, int)>();
            var queue = new Queue<((int, int) pos, int dist)>();

            queue.Enqueue((start, 0));
            visited.Add(start);

            while (queue.Count > 0)
            {
                var (pos, dist) = queue.Dequeue();

                if (pos == goal)
                    return dist;

                foreach (var neighbor in GetNeighbors(pos))
                {
                    if (!visited.Contains(neighbor))
                    {
                        visited.Add(neighbor);
                        queue.Enqueue((neighbor, dist + 1));
                    }
                }
            }

            // No path
            return int.MaxValue;
        }
        
        private static Room GenerateRoomFromType(Types.RoomType roomType, Vector3 position, int row = -1, int col = -1)
        {
            if (!Instance.generationData.RoomDict.TryGetValue(roomType, out List<Room> possibleRooms) || possibleRooms.Count == 0)
            {
                DebugUtils.LogError($"No room prefabs found for type {roomType}");
                return null;
            }

            int randomIndex = UnityEngine.Random.Range(0, possibleRooms.Count);
            return GenerateRoomFromClass(possibleRooms[randomIndex], position, row, col);
        }


        private void BuildRoomAtCords(int row, int col, Types.DoorConfiguration additionalRequirements)
        {
            // this function will check what its required doors are, and then build a room that fits those requirements
            // check all of the adjacent rooms for doors. we will have two different sets of connections
            // REQUIRED connections - doors that must be present (i.e. if the room to the north has a south door, this room must have a north door)
            // OPTIONAL connections - doors that can be present, but are not required (i.e. if the room to the east is empty, this room can have an east door, but it doesn't have to)
            
            // Step1. get the required connections
            Types.DoorConfiguration requiredConnections = GenerateRequiredConnections(row, col);
            
            // add in any additional requirements
            if (additionalRequirements.NorthDoorActive) requiredConnections.NorthDoorActive = true;
            if (additionalRequirements.EastDoorActive) requiredConnections.EastDoorActive = true;
            if (additionalRequirements.SouthDoorActive) requiredConnections.SouthDoorActive = true;
            if (additionalRequirements.WestDoorActive) requiredConnections.WestDoorActive = true;
            
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
            DebugUtils.LogSuccess($"Required connections for room at ({row}, {col}): N:{requiredConnections.NorthDoorActive}, E:{requiredConnections.EastDoorActive}, S:{requiredConnections.SouthDoorActive}, W:{requiredConnections.WestDoorActive}");
            DebugUtils.LogSuccess($"Optional connections for room at ({row}, {col}): N:{optionalConnections.NorthDoorActive}, E:{optionalConnections.EastDoorActive}, S:{optionalConnections.SouthDoorActive}, W:{optionalConnections.WestDoorActive}");
            
            
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
            
            Vector3 position = new Vector3(col * 20, (-row * 20), 0);

            Types.RoomType roomTypeToSpawn = GenerateRoomTypeFromConfiguration(requiredConnections);
            Room newRoom = GenerateRoomFromType(roomTypeToSpawn, position, row, col);
            if (newRoom != null)
            {
                Instance.dungeonRooms[row][col] = newRoom;
                if (!bPhaseOneComplete)
                {
                    alteredRoomCoords.Add((row, col));
                }
                
            }
            // add it to the dungeon grid
        }
        
        private static Types.RoomType GenerateRoomTypeFromConfiguration(Types.DoorConfiguration configuration)
        {
            /*
             * The patern this will ALWAYS follow is:
             * North, East, South, West
             *
             * MEANING: if a room has a North and East door, it will be classified as a "NE" room
             *          or if a room is South and West, it will be classified as a "SW" room
             *          East will never appear before North, and South will never appear before East.. ect.
             */
            
            // Create a string mapping to Transfer the door configuration to the correct order
            string doorPattern = "";
            if (configuration.NorthDoorActive) doorPattern += "N";
            if (configuration.EastDoorActive) doorPattern += "E";
            if (configuration.SouthDoorActive) doorPattern += "S";
            if (configuration.WestDoorActive) doorPattern += "W";
            
            // Now create all the possible room types
            List<string> possibleRoomTypes = new List<string>
            {
                "N", "E", "S", "W",
                "NE", "NS", "NW", "ES", "EW", "SW",
                "NES", "NEW", "NSW", "ESW",
                "NESW"
            };
            // Now we can match the doorPattern to the possibleRoomTypes
            if (possibleRoomTypes.Contains(doorPattern))
            {
                DebugUtils.LogSuccess($"Room configuration {doorPattern} is valid.");
                // Now we can generate the room based on the door pattern
                Types.RoomType roomTypeToSpawn = doorPattern switch
                {
                    "N" => Types.RoomType.N,
                    "E" => Types.RoomType.E,
                    "S" => Types.RoomType.S,
                    "W" => Types.RoomType.W,
                    "NE" => Types.RoomType.NE,
                    "NS" => Types.RoomType.NS,
                    "NW" => Types.RoomType.NW,
                    "ES" => Types.RoomType.ES,
                    "EW" => Types.RoomType.EW,
                    "SW" => Types.RoomType.SW,
                    "NES" => Types.RoomType.NES,
                    "NEW" => Types.RoomType.NEW,
                    "NSW" => Types.RoomType.NSW,
                    "ESW" => Types.RoomType.ESW,
                    "NESW" => Types.RoomType.NESW,
                    _ => Types.RoomType.DEFAULT
                };
                return roomTypeToSpawn;
                
            }
            
            return Types.RoomType.DEFAULT;
            
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
            //DebugUtils.ClearConsole();
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