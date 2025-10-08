using System;
using System.Collections;
using System.Collections.Generic;
using Generation.Data;
using Generation.ScriptableObjects;
using UnityEngine;

public class RoomSpawnController : MonoBehaviour
{
    // reference to the room that we are attached to
    private Room _room;
    private RoomGridManager _roomGridManager;
    private List<EnemyControllerBase> enemiesInRoom = new List<EnemyControllerBase>();
    private List<GameObject> trapsInRoom = new List<GameObject>();
    private List<BaseItem> lootInRoom = new List<BaseItem>();
    
    [Header("Enemy Spawn Data")]
    public List<SpawnableGroup> spawnableMap = new List<SpawnableGroup>();
    
    

    
    private void Start()
    {
        _room = GetComponentInParent<Room>();
        _roomGridManager = GetComponentInParent<RoomGridManager>();
        
        Initialize();
        EventBroadcaster.SetSeed += SetSeed;
    }

    private void SetSeed(int seed)
    {
        UnityEngine.Random.InitState(seed);
    }
    
    private void Initialize()
    {
        
        // Get access to the Room
        Transform SpawnLocation = _roomGridManager.FindValidWalkableCell();
        
        
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
                int spawnCount = UnityEngine.Random.Range(enemyData.minSpawnCount, enemyData.maxSpawnCount + 1);
                for (int i = 0; i < spawnCount; i++)
                {
                    Transform spawnLocation = _roomGridManager.FindValidWalkableCell();
                    if (spawnLocation != null)
                    {
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
                    Transform spawnLocation = _roomGridManager.FindValidWalkableCell();
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
        enemiesInRoom.Add(spawnedEnemy);
    }
    
}
