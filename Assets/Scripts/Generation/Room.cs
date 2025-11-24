using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;
using UnityEngine.UIElements;

public class Room : MonoBehaviour
{
    // DoorOne = North, DoorTwo = East, DoorThree = South, DoorFour = West
    
    
    // A list of doors that can be used to enter/exit the room
    [Header("Room Configuration")]
    [SerializeField] public Types.DoorConfiguration configuration;
    [SerializeField] private GameObject doors;
    [SerializeField] private bool bRequireFullRoomCleared; // parent object for all room content (enemies, pickups, etc)
    public bool GetRequireFullRoomCleared() { return bRequireFullRoomCleared; }
    
    [Space(10f)]
    [Header("Room Type")]
    [SerializeField] private Types.RoomType roomType;
    [SerializeField] private Types.RoomClassification roomClassification = Types.RoomClassification.Normal;
    public Types.RoomClassification GetRoomClassification() { return roomClassification; }
    
    public bool bIsFinalized = false; // has the player been in this room before?
    public bool bIsDifficultySet = false; // has the difficulty been set for this room?
    
    
    private RoomSpawnController roomSpawnController;
    
    private int initial_number_of_enemies_in_room = 0;
    
    
    
    
    // what is the difficulty rating of this room?
    public int roomDifficulty = int.MaxValue; // default to max value, so we can tell if it has been set or not.
    // What are the coordinates of this room in the grid? (-1, -1) if not set
    private (int row, int col) RoomCoords = (-1, -1);
    //Signals whether the door unlock sound has to be played or not.
    bool openSoundHasPlayed = false;


    // Special logic for the spawn room
    private void SpecialRoomLogic()
    {
        
        // if the player currently is the "Normal" persona and has MORE than 1 Persona, disable all doors
        // once the player has a different persona, enable the doors again
        
        // doors should only be enabled as Normal if the player has more 1 persona remaining
        if (roomType == Types.RoomType.Spawn)
        {
            
            int numberOfPersonas = PersonaManager.Instance.GetNumberOfAvailablePersonas();
            if (PersonaManager.Instance.GetPersona() == Types.Persona.Normal && numberOfPersonas > 1)
            {
                // disable all doors
                foreach (Transform door in doors.transform)
                {
                    // cast to a Door
                    Door doorComponent = door.GetComponent<Door>();

                    
                    if (doorComponent != null)
                    {
                        // get the door trigger interaction
                        DoorTriggerInteraction doorTrigger = door.GetComponent<DoorTriggerInteraction>();
                        // we only wanna auto lock the north door on the spawn room
                        if (doorTrigger && doorTrigger.CurrentDoorPosition == Types.DoorClassification.North)
                        {
                            //TODO: this is where we need to somehow do persona logic
                            /*
                             * This system will check to see if we are atleast on the second run (since the first needs to be P pressed to unlock the north door)
                             * This ensures the tutorial is relevant
                             *
                             * But once the persona menu is opened, we need to watch to see if they change their persona,
                             * and if they do, then we close the menu and launch them into the game)
                             */
                            //doorComponent.SetDoorState(Door.DoorState.Locked);
                            // STEP 1: Check if we are on the second run or higher
                            if (GameStateManager.Instance.GetPlayerDeathCount() <= 0)
                            {
                                doorComponent.SetDoorState(Door.DoorState.Locked);
                                return;
                            }
                            
                            //Step 2: Allow the door to open the persona menu
                            if(doorTrigger.IsSpawnDoor() == false){ return; }

                            if (PersonaManager.Instance.GetPersona() != Types.Persona.Normal)
                            {
                                doorTrigger.SetReadyToOpenMenu(false);
                                return;
                            }
                            doorTrigger.SetReadyToOpenMenu(true);
                        }
                        
                    }
                }
                
            }
            else
            {
                foreach (Transform door in doors.transform)
                {
                    // cast to a Door
                    Door doorComponent = door.GetComponent<Door>();
                    if (doorComponent != null)
                    {
                        // if the door is open then we don't want to close it
                        if (doorComponent.GetCurrentState() == Door.DoorState.Open)
                            continue;
                        doorComponent.SetDoorState(Door.DoorState.Closed);
                    }
                }
            }
            
        }

        // End is the boss room, so once we enter, we wanna permananetly lock the south door
        if (roomType == Types.RoomType.End)
        {
            // Find the south door and lock it
            foreach (Transform door in doors.transform)
            {
                // cast to a Door
                Door doorComponent = door.GetComponent<Door>();
                if (doorComponent != null)
                {
                    // get the door trigger interaction
                    DoorTriggerInteraction doorTrigger = door.GetComponent<DoorTriggerInteraction>();
                    if (doorTrigger && doorTrigger.CurrentDoorPosition == Types.DoorClassification.South)
                    {
                        doorComponent.SetDoorState(Door.DoorState.Locked);
                    }
                }
            }
            
        }
        if (roomType == Types.RoomType.Final)
        {
            
            if (GameStateManager.Instance.GetBuddeeDialogState() != "Vertwin")
            {
                GameStateManager.Instance.SetEndGameFlag();
            }
            
        }
        
    }
    
    public void Update()
    {
        // only run this logic if we are the spawn room
        if (roomType == Types.RoomType.Spawn || roomType == Types.RoomType.End || roomType == Types.RoomType.End)
        {
            SpecialRoomLogic();
        }
        
        // debug key to check how many enemies are in the room
        if(Input.GetKeyDown(KeyCode.P))
        {
            if (roomSpawnController)
            {
                int enemyCount = roomSpawnController.GetEnemiesInRoom().Count;
                Debug.Log($"Enemies in room: {enemyCount}");
            }
        }

        
    }
    public void UpdateLockedDoors(bool forceLocked = false)
    {
        EnableAllDoors();
        if (roomSpawnController)
        {
            int enemyCount = roomSpawnController.GetEnemiesInRoom().Count;
            if (enemyCount > 0 || forceLocked)
            {
                // set all closed doors to locked
                foreach (Transform door in doors.transform)
                {
                    // cast to a Door
                    Door doorComponent = door.GetComponent<Door>();
                    if (doorComponent != null)
                    {
                        if (doorComponent.GetCurrentState() == Door.DoorState.Closed)
                        {
                            
                            doorComponent.SetDoorState(Door.DoorState.Locked);
                        }
                    }
                }
            }
            else
            {
                // set all locked doors to closed
                foreach (Transform door in doors.transform)
                {
                    // cast to a Door
                    Door doorComponent = door.GetComponent<Door>();
                    if (doorComponent != null)
                    {
                        if (doorComponent.GetCurrentState() == Door.DoorState.Locked)
                        {
                            doorComponent.SetDoorState(Door.DoorState.Closed);
                        }
                    }
                }
                if (!openSoundHasPlayed && DungeonController.Instance.GetNumberOfEnemiesInCurrentRoom() == 0 && initial_number_of_enemies_in_room > 0)
                {
                    AudioManager.Instance.PlayUnlockDoorSound(1, 0.1f);
                    openSoundHasPlayed = true;
                }
            }
        }
    }
    

    
    private void Start()
    {
        roomSpawnController = GetComponent<RoomSpawnController>();
        
        SpecialRoomLogic();
        
        // hook up to the enemy death event to check if we need to unlock doors
        EventBroadcaster.EnemyDeath += OnEnemyDeath;
        EventBroadcaster.PlayerChangedRoom += OnPlayerChangedRoom;
        // Register the pits in this room
        PitManager.Instance.RegisterPitsInRoom(this);
        initial_number_of_enemies_in_room = roomSpawnController != null ? roomSpawnController.GetEnemiesInRoom().Count : 0;
        EnableAllDoors();
    }

    public void EnableAllDoors()
    {
        /*
         * loop through all of our doors, and just enable them
         */
        foreach (Transform door in doors.transform)
        {
            door.gameObject.SetActive(true);
        }
            
    }
    private void OnPlayerChangedRoom((int row, int col) targetRoomCoords)
    {
        
        // check to see if its the final room we went too
        Vector2 cords = DungeonGeneratorManager.Instance.GetEndPos();
        // convert to int tuple
        (int row, int col) endRoomCoords = ((int)cords.y, (int)cords.x);
        if (targetRoomCoords == endRoomCoords)
        {
            // we are entering the final room
            GameStateManager.Instance.SetEndGameFlag();
        }
        // attempt to get the room grid manager to regenerate grids
        RoomGridManager roomGridManager = FindAnyObjectByType<RoomGridManager>();
        if (roomGridManager != null)
        {
            roomGridManager.RegenerateGrids();
        }
    }
    
    private void OnEnemyDeath(EnemyControllerBase enemy, Room room)
    {
        // only care about deaths in this room
        if (room != this)
            return;
        // ensure its not a chest or pot enemy
        if (enemy.enemyType == Types.EnemyType.ChestEnemy || enemy.enemyType == Types.EnemyType.PotEnemy)
        {
            return;
        }
        
        // tell the room spawn controller to remove the enemy from its list
        if (roomSpawnController)
        {
            roomSpawnController.RemoveEnemyFromRoom(enemy);
        }
        
        // update the locked doors
        UpdateLockedDoors();
    }
    

    public void SetRoomDifficulty(int difficulty)
    {
        roomDifficulty = difficulty;
    }
    public int GetRoomDifficulty() { return roomDifficulty; }
    public void SetRoomCoords(int row, int col) { RoomCoords = (row, col); }
    public (int row, int col) GetRoomCoords() { return RoomCoords; }


    public void Awake()
    {
        // Catch the doors, incase its null
        if (doors == null)
        {
            doors = transform.Find("Doors")?.gameObject;
            if (doors == null)
            {
                Debug.LogError($"{name}: Could not find child object named 'Doors'.");
                return;
            }
        }
        InitializeRoom();
    }
    
    public void InitializeRoom(int difficulty = 1, (int row, int col)? coords = null)
    {

        roomDifficulty = difficulty;
        if (coords.HasValue)
            RoomCoords = coords.Value;
        
        // Initialize room logic here
        ApplyDoorConfiguration();
        
        bIsFinalized = false;
    }
    

    public void SetRoomEnabled(bool bEnabled)
    {
        Action action = bEnabled ? EnableRoom : DisableRoom;
        action();
    }

    
    private IEnumerator EnableRoomCoroutine(float delay)
    {
        // wait for end of frame to ensure all room content is loaded
        yield return new WaitForSeconds(delay);
        UpdateLockedDoors();
    }
    
    private void EnableRoom()
    {
        // disable the room here
        gameObject.SetActive(true);
        // enable all doors
        EnableAllDoors();
        StartCoroutine(EnableRoomCoroutine(2f));
        // this is a separate function, incase we need to do more complex logic in the future

    }
    private void DisableRoom()
    {
        // disable the room here
        gameObject.SetActive(false);
        
        // this is a separate function, incase we need to do more complex logic in the future
    }
    
    private void ApplyDoorConfiguration()
    {
        // Apply the door configuration to the doors in the room
        
        // get a list of all of the children of the doors object
        List<DoorTriggerInteraction> doorObjects = new List<DoorTriggerInteraction>(doors.GetComponentsInChildren<DoorTriggerInteraction>());
        foreach (var door in doorObjects)
        {
            switch (door.CurrentDoorPosition)
            {
                case Types.DoorClassification.North:
                    door.gameObject.SetActive(configuration.NorthDoorActive);
                    break;
                case Types.DoorClassification.East:
                    door.gameObject.SetActive(configuration.EastDoorActive);
                    break;
                case Types.DoorClassification.South:
                    door.gameObject.SetActive(configuration.SouthDoorActive);
                    break;
                case Types.DoorClassification.West:
                    door.gameObject.SetActive(configuration.WestDoorActive);
                    break;
                default:
                    Debug.LogWarning("Door has no classification set.");
                    break;
            }
        }
    }

    public Types.DoorConfiguration GetRequiredConnections()
    {
        // this will return a door configuration based on the current room type.
        // for example, if the current room is a North room, it will return a configuration with the South door active.
        // because a North room can only be entered from the South.
        // East room -> West door active
        // if a room has East and South doors, it will return a configuration with the West and North doors active... and so fourth
        Types.DoorConfiguration config = new Types.DoorConfiguration
        {
            NorthDoorActive = false,
            EastDoorActive = false,
            SouthDoorActive = false,
            WestDoorActive = false
        };

        // Look at this room’s doors and flip them to the opposite side
        if (configuration.NorthDoorActive)
            config.SouthDoorActive = true;

        if (configuration.SouthDoorActive)
            config.NorthDoorActive = true;

        if (configuration.EastDoorActive)
            config.WestDoorActive = true;

        if (configuration.WestDoorActive)
            config.EastDoorActive = true;

        return config;
        
    }
    public int GetActiveDoorCount()
    {
        int count = 0;
        if (configuration.NorthDoorActive) count++;
        if (configuration.EastDoorActive) count++;
        if (configuration.SouthDoorActive) count++;
        if (configuration.WestDoorActive) count++;
        return count;
    }
    
    public Types.RoomType GetRoomType()
    {
        return roomType;
    }

    public void OnDestroy()
    {
        //unsubscribe from events
        EventBroadcaster.EnemyDeath -= OnEnemyDeath;
        EventBroadcaster.PlayerChangedRoom -= OnPlayerChangedRoom;
    }
}
