using System;
using System.Collections;
using System.Collections.Generic;
using Generation.ScriptableObjects;
using UnityEngine;
using Object = UnityEngine.Object;

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
        DungeonController dungeonController;
        
        public List<List<Room>> GetDungeonRooms()
        {
            return dungeonRooms;
        }
        
        [Header("Generation Data")]
        [SerializeField] private  int rows = 5;
        [SerializeField] private  int cols = 5;
        [SerializeField] private float probabilityToAddOptionalDoor_OneRequiredDoor = 0.75f;
        [SerializeField] private float probabilityToAddOptionalDoor_TwoRequiredDoors = 0.5f;
        [SerializeField] private float probabilityToAddOptionalDoor_ThreeRequiredDoors = 0.25f;
        [SerializeField] private GenerationData generationData;
        [Space(10f)]
        [Header("Start/End Positions (Row, Col)")]
        [Header("If set to -1, -1, will randomize")]
        [SerializeField] private Vector2Int _startPos = new Vector2Int(-1, -1);
        private Vector2Int startPos = new Vector2Int(-1, -1); public Vector2Int GetStartPos() { return startPos; }
        [SerializeField] private Vector2Int _endPos = new Vector2Int(-1, -1);
        private Vector2Int endPos = new Vector2Int(-1, -1); public Vector2Int GetEndPos() { return endPos; }
        
        [SerializeField] private int Seed = 16; // for future use, if we want to have seeded generation
        // this is the distance between rooms, should be 200 for now so they dont overlap in any way
        [SerializeField] private int RoomOffset = 30;
        
        private (int row, int col) CurrentRoomCoords = (-1, -1); public (int row, int col) GetCurrentRoomCoords() { return CurrentRoomCoords; }
        
        public void Start()
        {
            EventBroadcaster.GameStarted += OnGameStarted;
            EventBroadcaster.PlayerChangedRoom += OnPlayerChangedRoom;
            dungeonController = FindFirstObjectByType<DungeonController>();
        }
        

        
        private void DisableAllRoomsExceptCurrent((int row, int col) currentRoomCoords)
        {
            for (int r = 0; r < dungeonRooms.Count; r++)
            {
                for (int c = 0; c < dungeonRooms[r].Count; c++)
                {
                    Room currentRoom = dungeonRooms[r][c];
                    if (currentRoom != null)
                    {
                        if (r == currentRoomCoords.row && c == currentRoomCoords.col)
                        {
                            currentRoom.SetRoomEnabled(true);
                            CurrentRoomCoords = (r, c);
                        }
                        else
                        {
                            currentRoom.SetRoomEnabled(false);
                        }
                    }
                }
            }
        }
        private void OnPlayerChangedRoom((int row, int col) newRoomCoords)
        {
            // enable the new rooms corns
            Room newRoom = dungeonRooms[newRoomCoords.row][newRoomCoords.col];
            newRoom.gameObject.SetActive(true);
            // and then after a small delay, disable all other rooms
            CurrentRoomCoords = newRoomCoords;
            
            StartCoroutine(DisableOtherRoomsCoroutine(newRoomCoords));
        }
        
        private IEnumerator DisableOtherRoomsCoroutine((int row, int col) currentRoomCoords)
        {
            yield return new WaitForSeconds(0.5f); // wait half a second before disabling other rooms
            DisableAllRoomsExceptCurrent(currentRoomCoords);
        }
        
        
        
        private void OnGameStarted()
        {
            LoadIntoDungeon();
            // Debug welcome message
            DebugUtils.Log("Welcome " + PlayerStats.Instance.GetPlayerName() + " to Friend Finder!");
        }

        public void LoadIntoDungeon()
        {
            // initialize the dungeon
            InitializeDungeonGrid(rows, cols);
            // generate the dungeon
            DungeonGeneration();
            
            // teleport the player into the dungeon
            Vector3 spawnRoomPosition = dungeonRooms[startPos.x][startPos.y].transform.Find("SPAWN_POINT").position;
            PlayerManager.Instance.TeleportPlayer(spawnRoomPosition, false);
            DisableAllRoomsExceptCurrent((startPos.x, startPos.y)); // disable all rooms except spawn on default
            EventBroadcaster.Broadcast_StartDialogue("BUDDEE");
        }
        
        // ReSharper disable Unity.PerformanceAnalysis
        private void DungeonGeneration()
        {
            if (Seed != 0)
            {
                UnityEngine.Random.InitState(Seed);
                EventBroadcaster.Broadcast_SetSeed(Seed);
            }
            
            InitializeStartAndEndRoom();
            
            GeneratePhaseOne();
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
            
            // Update all the rooms to hold their cords
            for (int r = 0; r < dungeonRooms.Count; r++)
            {
                for (int c = 0; c < dungeonRooms[r].Count; c++)
                {
                    Room currentRoom = dungeonRooms[r][c];
                    if (currentRoom != null)
                    {
                        currentRoom.SetRoomCoords(r, c);
                    }
                }
            }
            
            // start from the start room, and calculate the difficulty of each room
            Room startingRoom = dungeonRooms[startPos.x][startPos.y];
            CalculateRoomDifficulty(dungeonRooms, startingRoom, startPos.x, startPos.y);
            
            // warmup the dungeon
            //StartCoroutine(PrewarmDungeon(dungeonRooms));
            
        }
        
        
        private void PCG(List<List<Room>> dungeonMap, Room currentRoom, int currentRow, int currentCol)
        {
            // This will be the recursive depth first search algorithm to generate the dungeon
            // prevent infinite recursion
            if (currentRoom == null || currentRoom.bIsFinalized) { return;}
            // mark as visited
            currentRoom.bIsFinalized= true;
            
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
        private void CalculateRoomDifficulty(List<List<Room>> dungeonMap, Room currentRoom, int currentRow, int currentCol)
        {
            // This will be the recursive depth first search algorithm to generate the dungeon
            // prevent infinite recursion
            if (currentRoom == null || currentRoom.bIsDifficultySet) { return;}
            // mark as visited
            currentRoom.bIsDifficultySet= true;
            
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
                if (northRoom != null)
                {
                    int difficultyToSet = Math.Min(northRoom.GetRoomDifficulty(), currentRoom.GetRoomDifficulty() + 1);
                    northRoom.SetRoomDifficulty(difficultyToSet);
                    CalculateRoomDifficulty(dungeonMap, northRoom, currentRow - 1, currentCol);
                }
            }
            
            // east
            if(connections.EastDoorActive && currentCol + 1 < dungeonMap[currentRow].Count)
            {
                // get the room to the east
                Room eastRoom = dungeonMap[currentRow][currentCol + 1];
                // ensure the room has not been visited
                if (eastRoom != null)
                {
                    // PCG a new room at this location
                    int difficultyToSet = Math.Min(eastRoom.GetRoomDifficulty(), currentRoom.GetRoomDifficulty() + 1);
                    eastRoom.SetRoomDifficulty(difficultyToSet);
                    CalculateRoomDifficulty(dungeonMap, eastRoom, currentRow, currentCol + 1);
                }
            }
            
            // South
            if(connections.SouthDoorActive && currentRow + 1 < dungeonMap.Count)
            {
                // get the room to the south
                Room southRoom = dungeonMap[currentRow + 1][currentCol];
                // ensure the room has not been visited
                if (southRoom != null )
                {
                    // PCG a new room at this location
                    // update that rooms difficulty to be +1 of the current room
                    int difficultyToSet = Math.Min(southRoom.GetRoomDifficulty(), currentRoom.GetRoomDifficulty() + 1);
                    southRoom.SetRoomDifficulty(difficultyToSet);
                    CalculateRoomDifficulty(dungeonMap, southRoom, currentRow + 1, currentCol);
                }
            }
            
            // West
            if(connections.WestDoorActive && currentCol - 1 >= 0)
            {
                // get the room to the west
                Room westRoom = dungeonMap[currentRow][currentCol - 1];
                // ensure the room has not been visited
                if (westRoom != null)
                {
                    // PCG a new room at this location
                    // update that rooms difficulty to be the min of the rooms current difficulty, and this rooms difficulty + 1
                    int difficultyToSet = Math.Min(westRoom.GetRoomDifficulty(), currentRoom.GetRoomDifficulty() + 1);
                    westRoom.SetRoomDifficulty(difficultyToSet);
                    CalculateRoomDifficulty(dungeonMap, westRoom, currentRow, currentCol - 1);
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
                // Now build the room at the new position if it doesn't already exist
                if (dungeonRooms[currentRow][currentCol] == null)
                {
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
            // now we need to buid the last room, which is the room directly below the end room
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
                startPos = new Vector2Int(rows - 1, randomCol);
            }

            Vector3 startPosition = new Vector3(startPos.y * RoomOffset,-startPos.x * RoomOffset, 0);
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
            Vector3 endPosition = new Vector3(endPos.y * RoomOffset,-endPos.x * RoomOffset, 0);
            Room endRoom = GenerateRoomFromType(Types.RoomType.End, endPosition);
            
            endRoom.SetRoomDifficulty(int.MaxValue); // we will set the difficulty of the end room to be the max int, and then when we calculate the difficulty of the other rooms, it will be set to the correct value
            
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
            Room[] existingRooms = FindObjectsByType<Room>(FindObjectsSortMode.None);
            foreach (Room room in existingRooms)
            {
                DestroyImmediate(room.gameObject);
            }
            // destroy all animated buttons
            AnimatedButton[] existingButtons = FindObjectsByType<AnimatedButton>(FindObjectsSortMode.None);

            foreach (AnimatedButton button in existingButtons)
            {
                DestroyImmediate(button.gameObject);
            }
            // destroy all objects of the SpawnableObject type
            SpawnableObject[] existingSpawnableObjects = FindObjectsByType<SpawnableObject>(FindObjectsSortMode.None);
            foreach (SpawnableObject obj in existingSpawnableObjects)
            {
                DestroyImmediate(obj.gameObject);
            }
            // destroy all projectiles
            EnemyProjectileController[] existingProjectiles = FindObjectsByType<EnemyProjectileController>(FindObjectsSortMode.None);
            foreach (EnemyProjectileController proj in existingProjectiles)
            {
                DestroyImmediate(proj.gameObject);
            }
        }
        
        IEnumerator PrewarmDungeon(List<List<Room>> dungeonLayout)
        {
            foreach (var row in dungeonLayout)
            {
                foreach (var room in row)
                {
                    if (room != null)
                    {
                        room.gameObject.SetActive(true);
                        yield return null; // 1 frame ensures Awake/Start/shaders run
                        // if its not the start room, disable it again
                        if (room.GetRoomCoords() != (startPos.x, startPos.y))
                        {
                            room.gameObject.SetActive(false);
                        }
                    }
                }
            }
            Debug.Log("Dungeon prewarm complete");
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
            
            // if we are in phase one
            
            // this will start at 0, and increment each time we build a room in phase one

            roomInstance.InitializeRoom(int.MaxValue, (row, col));
            
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
            
            Vector3 position = new Vector3(col * RoomOffset, (-row * RoomOffset), 0);

            Types.RoomType roomTypeToSpawn = GenerateRoomTypeFromConfiguration(requiredConnections);
            Room newRoom = GenerateRoomFromType(roomTypeToSpawn, position, row, col);
            if (newRoom != null)
            {
                Instance.dungeonRooms[row][col] = newRoom;
            }
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
            
            if (dungeonRooms == null || dungeonRooms.Count == 0)
            {
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