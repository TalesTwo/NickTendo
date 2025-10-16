using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Managers;
using UnityEngine;

public class DoorTriggerInteraction : TriggerInteractBase
{
    
    [Header("Door Settings")]
    [SerializeField] public Types.DoorClassification CurrentDoorPosition = Types.DoorClassification.None;
    
    
    // get a reference to the door scripy
    private Door _doorScript;

    public override void Interact()
    {
        base.Interact();
        // if we successfuly interacted with a door, and its closed, we can open it
        if (_doorScript != null && _doorScript.GetCurrentState() == Door.DoorState.Closed)
        {
            DebugUtils.Log("DoorTriggerInteraction: Opening door.");
            _doorScript.SetDoorState(Door.DoorState.Open);
            return;
        }

        if (_doorScript != null && _doorScript.GetCurrentState() == Door.DoorState.Locked)
        {
            // play a "locked door" sound
            return;
        }

        //SceneSwapManager.SwapSceneFromDoorUse(_sceneToLoad, DoorToSpawnTo);
        // We need to teleport the player, to the correct door for the correct room
        // first, we can get access to the room this door is a part of
        Room currentRoom = GetComponentInParent<Room>();

        // get access to the room manager, to get the dungeon layout
        List<List<Room>> dungeonLayout = DungeonGeneratorManager.Instance.GetDungeonRooms();
        // now, we can get the rooms coordinates
        (int row, int col) currentRoomCoords = currentRoom.GetRoomCoords();

        // Now depending on what type of door we are, we will adjust the coordinates accordingly
        (int row, int col) targetRoomCoords = currentRoomCoords;
        DebugUtils.Log("Current Room Coords: " + currentRoomCoords);
        switch (CurrentDoorPosition)
{
    case Types.DoorClassification.North:
        // Open north door of current room
        TryOpenDoor(currentRoom, Types.DoorClassification.North);

        // Move to the room north of the current one
        targetRoomCoords.row -= 1;

        // Open the south door of the target room
        Room northRoom = dungeonLayout[targetRoomCoords.row][targetRoomCoords.col];
        TryOpenDoor(northRoom, Types.DoorClassification.South);

        // Teleport handling
        HandleDoorTeleport(dungeonLayout, targetRoomCoords, Types.DoorClassification.South);
        break;

    case Types.DoorClassification.East:
        TryOpenDoor(currentRoom, Types.DoorClassification.East);

        targetRoomCoords.col += 1;
        Room eastRoom = dungeonLayout[targetRoomCoords.row][targetRoomCoords.col];
        TryOpenDoor(eastRoom, Types.DoorClassification.West);

        HandleDoorTeleport(dungeonLayout, targetRoomCoords, Types.DoorClassification.West);
        break;

    case Types.DoorClassification.South:
        TryOpenDoor(currentRoom, Types.DoorClassification.South);

        targetRoomCoords.row += 1;
        Room southRoom = dungeonLayout[targetRoomCoords.row][targetRoomCoords.col];
        TryOpenDoor(southRoom, Types.DoorClassification.North);

        HandleDoorTeleport(dungeonLayout, targetRoomCoords, Types.DoorClassification.North);
        break;

    case Types.DoorClassification.West:
        TryOpenDoor(currentRoom, Types.DoorClassification.West);

        targetRoomCoords.col -= 1;
        Room westRoom = dungeonLayout[targetRoomCoords.row][targetRoomCoords.col];
        TryOpenDoor(westRoom, Types.DoorClassification.East);

        HandleDoorTeleport(dungeonLayout, targetRoomCoords, Types.DoorClassification.East);
        break;

    case Types.DoorClassification.None:
        DebugUtils.LogError("DoorTriggerInteraction: CurrentDoorPosition is set to None. This is invalid.");
        return;

    default:
        throw new ArgumentOutOfRangeException();
}


    }

    private void TryOpenDoor(Room currentRoom, Types.DoorClassification direction)
    {
        if (currentRoom == null)
        {
            DebugUtils.LogError("TryOpenDoor: currentRoom is null.");
            return;
        }

        // Get all doors in this room
        var doors = currentRoom.GetComponentsInChildren<DoorTriggerInteraction>().ToList();

        // Find the door with the matching direction
        var targetDoor = doors.Find(door => door.CurrentDoorPosition == direction);

        // Open it if it exists and is closed
        if (targetDoor != null && targetDoor._doorScript != null)
        {
            if (targetDoor._doorScript.GetCurrentState() == Door.DoorState.Closed)
            {
                DebugUtils.Log($"DoorTriggerInteraction: Opening {direction} door of current room.");
                targetDoor._doorScript.SetDoorState(Door.DoorState.Open);
            }
        }
        else
        {
            DebugUtils.LogWarning($"TryOpenDoor: Could not find door {direction} in current room.");
        }
    }
    
    private static void HandleDoorTeleport(List<List<Room>> dungeonLayout, (int row, int col) targetRoomCoords, Types.DoorClassification doorToSpawnTo)
    {
        // Get the room at these coordinates
        // we need to do this first, so we can ensure that the room is loaded and active to teleport to
        EventBroadcaster.Broadcast_PlayerChangedRoom(targetRoomCoords);
        Room targetRoom = dungeonLayout[targetRoomCoords.row][targetRoomCoords.col];
        // get the door in the target room that is a south door
        Transform[] doors = targetRoom.GetComponentsInChildren<Transform>();
        foreach (Transform door in doors)
        {
            DoorTriggerInteraction doorTrigger = door.GetComponent<DoorTriggerInteraction>();
            if (doorTrigger != null && doorTrigger.CurrentDoorPosition == doorToSpawnTo)
            {
                PlayerManager.Instance.TeleportPlayer(doorTrigger.transform.Find("Spawn_Location").position);
                
                break;
            }
        }
    }
    
    protected override void Start()
    {
        // call super
        base.Start();
        
        _doorScript = GetComponent<Door>();
        if (_doorScript == null)
        {
            DebugUtils.LogError("DoorTriggerInteraction: No Door script found on this object.");
        }
    }
}
