using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;

public class DungeonController : Singleton<DungeonController>
{
    /*
     * This will be in charge of over-arching things that need to happen for the entire dungeon
     * Specifically during gameplay, this will be in charge of:
     * Ensuring that all rooms are disabled untill the player is about to enter them
     *
     * we need to pull some logic out of the generator, into this controller
     */



    public int GetNumberOfEnemiesInCurrentRoom()
    {
        (int row, int col) CurrentRoomCoords = DungeonGeneratorManager.Instance.GetCurrentRoomCoords();
        Room currentRoom = DungeonGeneratorManager.Instance.GetDungeonRooms()[CurrentRoomCoords.row][CurrentRoomCoords.col];
        if (currentRoom)
        {
            //if (!currentRoom.GetRequireFullRoomCleared())
            //{
                //return -1; // indicates that the room does not require clearing
            //}
            
            RoomSpawnController roomSpawnController = currentRoom.GetComponentInChildren<RoomSpawnController>();
            if (roomSpawnController != null)
            {
                // check to see if the bool 
                return roomSpawnController.GetEnemiesInRoom().Count;
            }

        }
        // Otherwise, there is no enemies in the room
        return 0;
    }
    
    

    
}
