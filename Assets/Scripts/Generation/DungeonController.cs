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
    
    public Room GetCurrentRoom()
    {
        (int row, int col) CurrentRoomCoords = DungeonGeneratorManager.Instance.GetCurrentRoomCoords();
        List<List<Room>> dungeonRooms = DungeonGeneratorManager.Instance.GetDungeonRooms();

        if (dungeonRooms != null)
        {
   
            Room currentRoom = dungeonRooms[CurrentRoomCoords.row][CurrentRoomCoords.col];
            if (currentRoom)
            {
                return currentRoom;
            }
        }
        return null;
    }
    
    public Vector3 SpawnEnemyInCurrentRoomByType(Types.EnemyType enemyType, int enemyLevelOverride = -1)
    {
        //  Get the current room
        Room currentRoom = GetCurrentRoom();
        if (currentRoom == null )
        {
            Debug.LogWarning("No current room found — cannot spawn enemy.");
            return Vector3.zero;
        }
        // get the spawn room controller 
        RoomSpawnController roomSpawnController = currentRoom.GetComponentInChildren<RoomSpawnController>();
        if (roomSpawnController == null)
        {
            Debug.LogWarning($"No RoomSpawnController found in room {currentRoom.name}");
            return Vector3.zero;
        }

        //  Get the spawn controller for this room
        RoomSpawnController spawnController = currentRoom.GetComponentInChildren<RoomSpawnController>();
        if (spawnController == null)
        {
            Debug.LogWarning($"No RoomSpawnController found in room {currentRoom.name}");
            return Vector3.zero;
        }

        //  Get the enemy prefab by type from your EnemyDatabase or Factory
        EnemyControllerBase enemyPrefab = EnemyDatabase.Instance.GetEnemyPrefabByType(enemyType);
        if (enemyPrefab == null)
        {
            return Vector3.zero;
        }
        Vector3 spawnLocation = roomSpawnController.SpawnEnemyAtValidLocation(enemyPrefab, true, enemyLevelOverride);
        return spawnLocation;

    }
    
    public void Update()
    {
     if(Input.GetKeyDown(KeyCode.K))   
     {
         DungeonController.Instance.SpawnEnemyInCurrentRoomByType(Types.EnemyType.FollowerEnemy);
     }
    }

}
