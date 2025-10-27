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



    private int cachedEnemyCount;
    private bool isUpdating = false;
    private float updateInterval = 0.75f;

    // this has been changed to have a slight time delay on updating, to avoid it returning too quickly
    public int GetNumberOfEnemiesInCurrentRoom()
    {
        if (!isUpdating)
            StartCoroutine(UpdateEnemyCount());

        return cachedEnemyCount;
    }

    private IEnumerator UpdateEnemyCount()
    {
        isUpdating = true;
        yield return new WaitForSeconds(updateInterval);

        (int row, int col) CurrentRoomCoords = DungeonGeneratorManager.Instance.GetCurrentRoomCoords();
        var dungeonRooms = DungeonGeneratorManager.Instance.GetDungeonRooms();

        int result = 0;

        if (dungeonRooms != null)
        {
            try
            {
                Room currentRoom = dungeonRooms[CurrentRoomCoords.row][CurrentRoomCoords.col];
                if (currentRoom)
                {
                    RoomSpawnController spawnController = currentRoom.GetComponentInChildren<RoomSpawnController>();
                    if (spawnController != null)
                        result = spawnController.GetEnemiesInRoom().Count;
                }
            }
            catch (System.ArgumentOutOfRangeException)
            {
                result = 0;
            }
        }

        cachedEnemyCount = result;
        isUpdating = false;
    }
}
