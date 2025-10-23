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

    private float _nextAllowedInteractTime = 0f;
    private const float InteractCooldown = 0.75f;
    public override void Interact()
    {

        // Prevent interaction if not ready
        if (Time.time < _nextAllowedInteractTime)
            return;

        // Update the next allowed time 
        _nextAllowedInteractTime = Time.time + InteractCooldown;
        
        base.Interact();
        // log the current door state 
        DebugUtils.Log("DoorTriggerInteraction: Current door state is " + (_doorScript != null ? _doorScript.GetCurrentState().ToString() : "No Door script found."));
        if (_doorScript != null && _doorScript.GetCurrentState() == Door.DoorState.Locked)
        {
            // play a "locked door" sound
            return;
        }
        // if we successfuly interacted with a door, and its closed, we can open it
        if (_doorScript != null && _doorScript.GetCurrentState() == Door.DoorState.Closed)
        {
            DebugUtils.Log("DoorTriggerInteraction: Opening door.");
            Managers.AudioManager.Instance.PlayOpenDoorSound(1, 0);
            _doorScript.SetDoorState(Door.DoorState.Open);
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
        
        // Teleport handling
        HandleDoorTeleport(dungeonLayout, targetRoomCoords, Types.DoorClassification.South);
        break;

    case Types.DoorClassification.East:
        TryOpenDoor(currentRoom, Types.DoorClassification.East);

        targetRoomCoords.col += 1;


        HandleDoorTeleport(dungeonLayout, targetRoomCoords, Types.DoorClassification.West);
        break;

    case Types.DoorClassification.South:
        TryOpenDoor(currentRoom, Types.DoorClassification.South);

        targetRoomCoords.row += 1;
        
        HandleDoorTeleport(dungeonLayout, targetRoomCoords, Types.DoorClassification.North);
        break;

    case Types.DoorClassification.West:
        TryOpenDoor(currentRoom, Types.DoorClassification.West);

        targetRoomCoords.col -= 1;

        HandleDoorTeleport(dungeonLayout, targetRoomCoords, Types.DoorClassification.East);
        break;

    case Types.DoorClassification.None:
        DebugUtils.LogError("DoorTriggerInteraction: CurrentDoorPosition is set to None. This is invalid.");
        return;

    default:
        throw new ArgumentOutOfRangeException();
}


    }

    private static void TryOpenDoor(Room currentRoom, Types.DoorClassification direction)
    {
        if (currentRoom == null)
        {
            DebugUtils.LogError("TryOpenDoor: currentRoom is null.");
            return;
        }

        // Get all doors in this room
        var doors = currentRoom.GetComponentsInChildren<DoorTriggerInteraction>(true).ToList();

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
    
    private void HandleDoorTeleport(List<List<Room>> dungeonLayout, (int row, int col) targetRoomCoords, Types.DoorClassification doorToSpawnTo)
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
                // cause of TIMING ISSUES
                DungeonGeneratorManager.Instance.StartCoroutine(OpenDoorWhenReady(targetRoom, doorToSpawnTo));
                break;
            }
        }
    }
    private static IEnumerator OpenDoorWhenReady(Room targetRoom, Types.DoorClassification direction)
    {
        // Wait until room and its children are active
        yield return new WaitUntil(() => targetRoom.gameObject.activeInHierarchy);
        yield return null; // also allow 1 frame for child activation

        TryOpenDoor(targetRoom, direction);
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
        
        // bind to the persona changed delegate, so we can update accordingly
        // when the persona is changed WHILE we are inside the hitbox
        EventBroadcaster.PersonaChanged += OnPersonaChanged;
    }

    private void OnPersonaChanged(Types.Persona newPersona)
    {
        // we will check again to see if we are currently overlapping
        if (_currentlyInOverlap)
        {
            // recall the overlap function
            OnTriggerEnter2D(Player.GetComponent<Collider2D>());
        }
        
    }
    
}
