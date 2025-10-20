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
    
    [Space(10f)]
    [Header("Room Type")]
    [SerializeField] private Types.RoomType roomType;
    
    public bool bIsFinalized = false; // has the player been in this room before?
    public bool bIsDifficultySet = false; // has the difficulty been set for this room?
    
    
    private RoomSpawnController roomSpawnController;
    
    
    
    
    // what is the difficulty rating of this room?
    public int roomDifficulty = int.MaxValue; // default to max value, so we can tell if it has been set or not.
    // What are the coordinates of this room in the grid? (-1, -1) if not set
    private (int row, int col) RoomCoords = (-1, -1);
    
    
    
    
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
                        doorComponent.SetDoorState(Door.DoorState.Locked);
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
    }
    
    public void Update()
    {
        // only run this logic if we are the spawn room
        if (roomType == Types.RoomType.Spawn)
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
        
        if (roomSpawnController)
        {
            int enemyCount = roomSpawnController.GetEnemiesInRoom().Count;
            if (enemyCount > 0 || forceLocked)
            {
                DebugUtils.Log($"Room: {name} still has {enemyCount} enemies. Keeping doors locked.");
                // set all closed doors to locked
                foreach (Transform door in doors.transform)
                {
                    // cast to a Door
                    Door doorComponent = door.GetComponent<Door>();
                    if (doorComponent != null)
                    {
                        DebugUtils.Log($"Locking door: {doorComponent.name} in room: {name} and the current state is: {doorComponent.GetCurrentState()}");
                        if (doorComponent.GetCurrentState() == Door.DoorState.Closed)
                        {
                            
                            doorComponent.SetDoorState(Door.DoorState.Locked);
                            DebugUtils.Log($"Room: {name} locking door: {doorComponent.name} and the current state is now: {doorComponent.GetCurrentState()}");
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
                        DebugUtils.Log($"Unlocking door: {doorComponent.name} in room: {name} and the current state is: {doorComponent.GetCurrentState()}");
                        if (doorComponent.GetCurrentState() == Door.DoorState.Locked)
                        {
                            doorComponent.SetDoorState(Door.DoorState.Closed);
                            DebugUtils.Log($"Room: {name} unlocking door: {doorComponent.name} and the current state is now: {doorComponent.GetCurrentState()}");
                        }
                    }
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
    }
    
    private void OnEnemyDeath(EnemyControllerBase enemy, Room room)
    {
        // only care about deaths in this room
        if (room != this)
            return;
        
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
    public int GetRoomDifficulty()
    {
        return roomDifficulty;
    }
    public void SetRoomCoords(int row, int col)
    {
        RoomCoords = (row, col);
    }
    public (int row, int col) GetRoomCoords()
    {
        return RoomCoords;
    }


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
}
