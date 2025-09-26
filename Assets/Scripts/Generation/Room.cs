using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    // DoorOne = North, DoorTwo = East, DoorThree = South, DoorFour = West
    
    
    // A list of doors that can be used to enter/exit the room
    [Header("Room Configuration")]
    [SerializeField] public Types.DoorConfiguration configuration;
    [SerializeField] private GameObject doors;
    
    [Space(10f)]
    [Header("Room Type")]
    [SerializeField] private Types.RoomType roomType;
    
    public bool bIsFinalized = false; // has the player been in this room before?
    
    
    
    // what is the difficulty rating of this room?
    public int roomDifficulty = 1;
    // What are the coordinates of this room in the grid? (-1, -1) if not set
    public (int row, int col) RoomCoords = (-1, -1);
    
    
    public void SetRoomDifficulty(int difficulty)
    {
        roomDifficulty = difficulty;
    }
    public void SetRoomCoords(int row, int col)
    {
        RoomCoords = (row, col);
    }


    public void Awake()
    {
        InitializeRoom();
    }
    
    public void InitializeRoom(int difficulty = 1, (int row, int col)? coords = null)
    {
        roomDifficulty = difficulty;
        if (coords.HasValue)
            RoomCoords = coords.Value;
        
        // Initialize room logic here
        ApplyDoorConfiguration();
        //TODO: Rename this to initalized
        bIsFinalized = false;

        
    }

    
    public void SetRoomEnabled(bool bEnabled)
    {
        Action action = bEnabled ? EnableRoom : DisableRoom;
        action();
    }

    private void EnableRoom()
    {
        // disable the room here
        gameObject.SetActive(true);
        
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
