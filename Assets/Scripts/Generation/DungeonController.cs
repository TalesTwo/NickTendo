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



    private void Start()
    {
        EventBroadcaster.PlayerChangedRoom += OnPlayerChangedRoom;
    }
    
    private void OnPlayerChangedRoom((int row, int col) targetRoomCoords)
    {
        // there is a few things we need to look for:
        /*
         * check to see one of these conditions:
         * shop -> non-shop
         * non-shop -> shop
         * boss room -> non-boss room
         * non-boss room -> boss room
         *
         * And we need to also note, that a "boss room" is the same as an "non shop" room for the purposes of music
         */
        
        // get access to our current room
        Room currentRoom = GetCurrentRoom();
        if (currentRoom == null) { return; }
        
        // get the room we are going to
        (int row, int col) targetCoords = targetRoomCoords;
        List<List<Room>> dungeonRooms = DungeonGeneratorManager.Instance.GetDungeonRooms();
        if(dungeonRooms == null) { return; }
        Room targetRoom = dungeonRooms[targetCoords.row][targetCoords.col];
        if (targetRoom == null) { return; }
        // check if we are in a shop room or boss room
        bool isInShopRoom = currentRoom.GetRoomClassification() == Types.RoomClassification.Shop;
        bool isInBossRoom = currentRoom.GetRoomClassification() == Types.RoomClassification.Boss;
        // check if we are going to a shop room or boss room
        bool isGoingToShopRoom = targetRoom.GetRoomClassification() == Types.RoomClassification.Shop;
        bool isGoingToBossRoom = targetRoom.GetRoomClassification() == Types.RoomClassification.Boss;
        
        // Case1: shop -> non-shop
        if (isInShopRoom && !isGoingToShopRoom)
        {
            EventBroadcaster.Broadcast_PlayerEnteredShopRoom(false);
        }
        // Case2: non-shop -> shop
        else if (!isInShopRoom && isGoingToShopRoom)
        {
            EventBroadcaster.Broadcast_PlayerEnteredShopRoom(true);
        }
        // Case3: boss room -> non-boss room
        else if (isInBossRoom && !isGoingToBossRoom)
        {
            EventBroadcaster.Broadcast_PlayerEnteredBossRoom(false);
        }
        // Case4: non-boss room -> boss room
        else if (!isInBossRoom && isGoingToBossRoom)
        {
            EventBroadcaster.Broadcast_PlayerEnteredBossRoom(true);
        }
        
        
        

    }
    
    public bool IsPlayerInAnyTutorialRoom()
    {
        /*
         * This will return true if the player is currently in a tutorial room, which will be done by checking the current room's properties
         */
        Room currentRoom = GetCurrentRoom();
        if (currentRoom != null)
        {
            return currentRoom.GetRoomClassification() == Types.RoomClassification.Tutorial;
        }
        return false;
    }
    // this has been changed to have a slight time delay on updating, to avoid it returning too quickly
    public int GetNumberOfEnemiesInCurrentRoom()
    {
        if (!isUpdating)
            StartCoroutine(UpdateEnemyCount());

        return cachedEnemyCount;
    }
    
    public int GetNumberOfEnemiesInRoom(Room room)
    {
        if (room)
        {
            RoomSpawnController spawnController = room.GetComponentInChildren<RoomSpawnController>();
            if (spawnController != null)
                return spawnController.GetEnemiesInRoom().Count;
        }
        return 0;
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

            try
            {
                Room currentRoom = dungeonRooms[CurrentRoomCoords.row][CurrentRoomCoords.col];
                if (currentRoom)
                {
                    return currentRoom;
                }
            } catch (System.ArgumentOutOfRangeException)
            {
                return null;
            }
        }
        return null;
    }
    
    public Vector3 SpawnEnemyInCurrentRoomByType(Types.EnemyType enemyType, bool addToRoomList = true, int enemyLevelOverride = -1)
    {
        //  Get the current room

        Room currentRoom = GetCurrentRoom();
        
        if (currentRoom == null )
        {
            // in this case, we will attempt to find a possible potential room. so we will look in the heirachy, and search for the first valid room
            Room[] allRooms = FindObjectsByType<Room>(FindObjectsSortMode.None);
            if (allRooms.Length > 0)
            {
                currentRoom = allRooms[0];
            }
            else
            {
                Debug.LogWarning("No current room found and no rooms in scene to fallback to.");
                return Vector3.zero;
            }
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
        Vector3 spawnLocation = roomSpawnController.SpawnEnemyAtValidLocation(enemyPrefab, addToRoomList, enemyLevelOverride);
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
