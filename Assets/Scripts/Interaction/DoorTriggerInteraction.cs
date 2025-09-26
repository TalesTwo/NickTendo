using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;

public class DoorTriggerInteraction : TriggerInteractBase
{

    
    [Header("Spawn To")]
    [SerializeField] private SceneField _sceneToLoad;
    [SerializeField] private Types.DoorClassification DoorToSpawnTo= Types.DoorClassification.None;
    
    [Space(10f)]
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
        (int row, int col) currentRoomCoords = currentRoom.RoomCoords;
        
        // Now depending on what type of door we are, we will adjust the coordinates accordingly
        (int row, int col) targetRoomCoords = currentRoomCoords;
        DebugUtils.Log("Current Room Coords: " + currentRoomCoords);
        switch (CurrentDoorPosition)
        {
            case Types.DoorClassification.North:
                targetRoomCoords.row -= 1;
                // Get the room at these coordinates
                Room targetRoom = dungeonLayout[targetRoomCoords.row][targetRoomCoords.col];
                // get the door in the target room that is a south door
                Transform[] doors = targetRoom.GetComponentsInChildren<Transform>();
                foreach (Transform door in doors)
                {
                    DoorTriggerInteraction doorTrigger = door.GetComponent<DoorTriggerInteraction>();
                    if (doorTrigger != null && doorTrigger.CurrentDoorPosition == Types.DoorClassification.South)
                    {
                        // teleport the player to this door's spawn point
                        // get the player
                        PlayerController player = FindObjectOfType<PlayerController>();
                        if (player != null)
                        {
                            // find the child named "Spawn_Location" under the doorTrigger
                            Transform spawnLocation = doorTrigger.transform.Find("Spawn_Location");
                            if (spawnLocation != null)
                            {
                                player.transform.position = spawnLocation.position;
                            }
                            

                        }
                        break;
                    }
                }
                break;
            case Types.DoorClassification.East:
                targetRoomCoords.col += 1;
                Room targetRoomE = dungeonLayout[targetRoomCoords.row][targetRoomCoords.col];
                // get the door in the target room that is a west door
                Transform[] doorsE = targetRoomE.GetComponentsInChildren<Transform>();
                foreach (Transform door in doorsE)
                {
                    DoorTriggerInteraction doorTrigger = door.GetComponent<DoorTriggerInteraction>();
                    if (doorTrigger != null && doorTrigger.CurrentDoorPosition == Types.DoorClassification.West)
                    {
                        // teleport the player to this door's spawn point
                        // get the player
                        PlayerController player = FindObjectOfType<PlayerController>();
                        if (player != null)
                        {
                            // get the position of the door's spawn point
                            Transform spawnLocation = doorTrigger.transform.Find("Spawn_Location");
                            if (spawnLocation != null)
                            {
                                player.transform.position = spawnLocation.position;
                            }

                        }
                        break;
                    }
                }
                break;
            case Types.DoorClassification.South:
                targetRoomCoords.row += 1;
                Room targetRoomS = dungeonLayout[targetRoomCoords.row][targetRoomCoords.col];
                // get the door in the target room that is a north door
                Transform[] doorsS = targetRoomS.GetComponentsInChildren<Transform>();
                foreach (Transform door in doorsS)
                {
                    DoorTriggerInteraction doorTrigger = door.GetComponent<DoorTriggerInteraction>();
                    if (doorTrigger != null && doorTrigger.CurrentDoorPosition == Types.DoorClassification.North)
                    {
                        // teleport the player to this door's spawn point
                        // get the player
                        PlayerController player = FindObjectOfType<PlayerController>();
                        if (player != null)
                        {
                            Transform spawnLocation = doorTrigger.transform.Find("Spawn_Location");
                            if (spawnLocation != null)
                            {
                                player.transform.position = spawnLocation.position;
                            }

                        }
                        break;
                    }
                }
                break;
            case Types.DoorClassification.West:
                targetRoomCoords.col -= 1;
                Room targetRoomW = dungeonLayout[targetRoomCoords.row][targetRoomCoords.col];
                // get the door in the target room that is a east door
                Transform[] doorsW = targetRoomW.GetComponentsInChildren<Transform>();
                foreach (Transform door in doorsW)
                {
                    DoorTriggerInteraction doorTrigger = door.GetComponent<DoorTriggerInteraction>();
                    if (doorTrigger != null && doorTrigger.CurrentDoorPosition == Types.DoorClassification.East)
                    {
                        // teleport the player to this door's spawn point
                        // get the player
                        PlayerController player = FindObjectOfType<PlayerController>();
                        if (player != null)
                        {
                            Transform spawnLocation = doorTrigger.transform.Find("Spawn_Location");
                            if (spawnLocation != null)
                            {
                                player.transform.position = spawnLocation.position;
                            }

                        }
                        break;
                    }
                }
                break;
            case Types.DoorClassification.None:
                Debug.LogError("DoorTriggerInteraction: CurrentDoorPosition is set to None. This is invalid.");
                return;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        
    }
}
