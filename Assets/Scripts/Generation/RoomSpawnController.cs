using System;
using System.Collections;
using System.Collections.Generic;
using Generation.ScriptableObjects;
using Unity.Mathematics;
using UnityEngine;

public class RoomSpawnController : MonoBehaviour
{
    // reference to the room that we are attached to
    private Room _room;
    private RoomGridManager _roomGridManager;
    private List<EnemyControllerBase> enemiesInRoom = new List<EnemyControllerBase>(); public List<EnemyControllerBase> GetEnemiesInRoom() { return enemiesInRoom; }
    private List<GameObject> trapsInRoom = new List<GameObject>();
    private List<BaseItem> lootInRoom = new List<BaseItem>();
    
    [Header("Enemy Spawn Data")]
    public List<SpawnableGroup> spawnableMap = new List<SpawnableGroup>();
    [SerializeField] private int minRoomDifficulty = 1;
    [SerializeField] private int maxRoomDifficulty = 20;
    [SerializeField] private int spawnVariance = 1;
    
    
    
    

    
    private void Start()
    {
        _room = GetComponentInParent<Room>();
        _roomGridManager = GetComponentInParent<RoomGridManager>();
        
        Initialize();
        EventBroadcaster.SetSeed += SetSeed;
    }

    // remove an enemy from the room's enemy list
    public void RemoveEnemyFromRoom(EnemyControllerBase enemy)
    {
        if (enemiesInRoom.Contains(enemy))
        {
            enemiesInRoom.Remove(enemy);
        }
    }
    
    private void SetSeed(int seed)
    {
        UnityEngine.Random.InitState(seed);
    }
    
    private void Initialize()
    {
        
        // Get access to the Room
        Transform SpawnLocation = _roomGridManager.FindValidSpawnableCell();
        
        // if there is no safe spawn location, do not spawn anything, as something went wrong in this room
        if (SpawnLocation == null)
        {
            DebugUtils.LogWarning("No valid spawn location found in room: " + _room.name);
            return;
        }
        // check to see if there is any enemies already as children of the room (these would be pre-placed enemies)
        FollowerEnemyController[] prePlacedEnemies = _room.GetComponentsInChildren<FollowerEnemyController>();
        foreach (FollowerEnemyController prePlacedEnemy in prePlacedEnemies)
        {
            enemiesInRoom.Add(prePlacedEnemy);
        }
        RangedEnemyController[] prePlacedRangedEnemies = _room.GetComponentsInChildren<RangedEnemyController>();
        foreach (RangedEnemyController prePlacedRangedEnemy in prePlacedRangedEnemies)
        {
            enemiesInRoom.Add(prePlacedRangedEnemy);
        }
        
        // look at the spawnable map, and find the list with the enemy type
        List<SpawnData> enemiesToSpawn = new List<SpawnData>();
        foreach (SpawnableGroup spawnableGroup in spawnableMap)
        {
            if (spawnableGroup.spawnType == Types.SpawnableType.Enemy)
            {
                enemiesToSpawn = spawnableGroup.Spawnables;
                break;
            }
        }
        // look for items to spawn
        List<SpawnData> itemsToSpawn = new List<SpawnData>();
        foreach (SpawnableGroup spawnableGroup in spawnableMap)
        {
            if (spawnableGroup.spawnType == Types.SpawnableType.Item)
            {
                itemsToSpawn = spawnableGroup.Spawnables;
                break;
            }
        }
        
        
        // pick a random enemy from the list of enemy prefabs
        if (enemiesToSpawn.Count > 0 && SpawnLocation != null)
        {
            
            // Look through the enemy data, and look for any with a spawnChance of 1 (guaranteed spawn)
            List<SpawnData> guaranteedEnemies = new List<SpawnData>();
            foreach (SpawnData enemyData in enemiesToSpawn)
            {
                if (enemyData.spawnChance >= 1f)
                {
                    guaranteedEnemies.Add(enemyData);
                }
                else
                {
                    float roll = UnityEngine.Random.Range(0f, 1f);
                    if (roll <= enemyData.spawnChance)
                    {
                        guaranteedEnemies.Add(enemyData);
                    }
                }
            }
            // loop through guaranteed enemies and spawn a random amount of them
            foreach (SpawnData enemyData in guaranteedEnemies)
            {
                int spawnCount = CalculateSpawnNumber(enemyData, _room.roomDifficulty);
                for (int i = 0; i < spawnCount; i++)
                {
                    Transform spawnLocation = _roomGridManager.FindValidSpawnableCell();
                    if (spawnLocation != null)
                    {
                        DebugUtils.Log("Spawning enemy at the following location: " + spawnLocation.position);
                        SpawnEnemy(enemyData.enemyPrefab.GetComponent<EnemyControllerBase>(), spawnLocation);
                    }
                }
                // Remove the guaranteed enemy from the list so we don't spawn it again
                enemiesToSpawn.Remove(enemyData);
            }
            

        }
        // pick a random item from the list of item prefabs
        if (itemsToSpawn.Count > 0 && SpawnLocation != null)
        {
            // Look through the item data, and look for any with a spawnChance of 1 (guaranteed spawn)
            List<SpawnData> guaranteedItems = new List<SpawnData>();
            foreach (SpawnData itemData in itemsToSpawn)
            {
                if (itemData.spawnChance >= 1f)
                {
                    guaranteedItems.Add(itemData);
                }
                else
                {
                    float roll = UnityEngine.Random.Range(0f, 1f);
                    if (roll <= itemData.spawnChance)
                    {
                        guaranteedItems.Add(itemData);
                    }
                }
            }
            // loop through guaranteed items and spawn a random amount of them
            foreach (SpawnData itemData in guaranteedItems)
            {
                int spawnCount = UnityEngine.Random.Range(itemData.minSpawnCount, itemData.maxSpawnCount + 1);
                for (int i = 0; i < spawnCount; i++)
                {
                    Transform spawnLocation = _roomGridManager.FindValidSpawnableCell();
                    if (spawnLocation != null)
                    {
                        // cast the enemy prefab to a BaseItem
                        BaseItem Prefab = itemData.enemyPrefab.GetComponent<BaseItem>();
                        BaseItem spawnedItem = Instantiate(Prefab, spawnLocation.position, Quaternion.identity);
                        spawnedItem.transform.parent = _room.transform;
                        lootInRoom.Add(spawnedItem);
                    }
                }
                // Remove the guaranteed item from the list so we don't spawn it again
                itemsToSpawn.Remove(itemData);
            }
        }
    }

    private void SpawnEnemy(EnemyControllerBase EnemyBasePrefab, Transform SpawnLocation)
    {
        EnemyControllerBase spawnedEnemy = Instantiate(EnemyBasePrefab, SpawnLocation.position, Quaternion.identity);
        // set the enemy prefab as a child of the room
        spawnedEnemy.transform.parent = _room.transform;
        spawnedEnemy.Initialize(_room.roomDifficulty);
        // see if we can cast to either a MeleeEnemyController or RangedEnemyController
        if (spawnedEnemy is FollowerEnemyController meleeEnemy || spawnedEnemy is RangedEnemyController rangedEnemy)
        {
            enemiesInRoom.Add(spawnedEnemy);
        }
        DebugUtils.Log("Spawned enemy: " + spawnedEnemy.name + " in room: " + _room.name);
    }

    private int CalculateSpawnNumber(SpawnData spawnData, int roomDifficulty)
    {
        /*
         * The overall spawning system is gonna be altered, where now when a thing spawns, rather than having a flat chance
         * at the number of enemies to spawn, we will have a system where based on the rooms difficulty, we will scale the number
         * So for example. we will have a overall Min - Max room difficulty.
         *
         * we will be scaling from minRoomDifficulty to maxRoomDifficulty in a normalized way, and then mapping that to a spawn count
         * of the enemy spawn data, which will also be normalized.
         *
         * slighty variance will be added to this value aswell, but it will never go below the min spawn count
         */
        int spawnCount = 0;
        
        float normalizedRoomDifficulty = (float)(roomDifficulty - minRoomDifficulty) / (float)(maxRoomDifficulty - minRoomDifficulty);

        float normalizedSpawnCount = normalizedRoomDifficulty * (spawnData.maxSpawnCount - spawnData.minSpawnCount) + spawnData.minSpawnCount;
        spawnCount = Mathf.RoundToInt(normalizedSpawnCount);
        // calculate the variance, which is just a random value between -spawnVariance and +spawnVariance
        
        // upgrade spawnVarience based on room difficulty
        // every int past the maxRoomDifficulty increases spawnVariance by 1
        int extraVariance = 0;
        if (roomDifficulty > maxRoomDifficulty)
        {
            extraVariance = roomDifficulty - maxRoomDifficulty;
        }
        
        int variance = UnityEngine.Random.Range(0, spawnVariance);
        variance += extraVariance;
        spawnCount += variance;
        
        
        return spawnCount;
    }
    
}
