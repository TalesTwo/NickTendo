using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;

public class DoorTriggerInteraction : TriggerInteractBase
{
    
    [Header("Door Settings")]
    [SerializeField] public Types.DoorClassification CurrentDoorPosition = Types.DoorClassification.None;

    public override void Interact()
    {
        //SceneSwapManager.SwapSceneFromDoorUse(_sceneToLoad, DoorToSpawnTo);
        // We need to teleport the player, to the correct door for the correct room
        // first, we can get access to the room this door is a part of
        Room currentRoom = GetComponentInParent<Room>();
        
        // get access to the room manager, to get the dungeon layout
        List<List<Room>> dungeonLayout = DungeonGeneratorManager.Instance.GetDungeonRooms();
        // now, we can get the rooms coordinates
        (int row, int col) currentRoomCoords = currentRoom.GetRoomCoords(row: -1, col: -1);
        
        // Now depending on what type of door we are, we will adjust the coordinates accordingly
        (int row, int col) targetRoomCoords = currentRoomCoords;
        DebugUtils.Log("Current Room Coords: " + currentRoomCoords);
        switch (CurrentDoorPosition)
        {
            case Types.DoorClassification.North:
                targetRoomCoords.row -= 1;
                HandleDoorTeleport(dungeonLayout, targetRoomCoords, Types.DoorClassification.South);
                break;
            case Types.DoorClassification.East:
                targetRoomCoords.col += 1;
                HandleDoorTeleport(dungeonLayout, targetRoomCoords, Types.DoorClassification.West);
                break;
            case Types.DoorClassification.South:
                targetRoomCoords.row += 1;
                HandleDoorTeleport(dungeonLayout, targetRoomCoords, Types.DoorClassification.North);
                break;
            case Types.DoorClassification.West:
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

    private static void HandleDoorTeleport(List<List<Room>> dungeonLayout, (int row, int col) targetRoomCoords, Types.DoorClassification doorToSpawnTo)
    {
        // Get the room at these coordinates
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
}
