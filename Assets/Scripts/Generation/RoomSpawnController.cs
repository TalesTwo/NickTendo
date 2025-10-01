using System;
using System.Collections;
using System.Collections.Generic;
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
    
    public List<EnemyData> EnemyData; // List of enemy prefabs that can be spawned in this room
    
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
        
        // pick a random enemy from the list of enemy prefabs
        if (EnemyData.Count > 0 && SpawnLocation != null)
        {
            
            // Look through the enemy data, and look for any with a spawnChance of 1 (guaranteed spawn)
            List<EnemyData> guaranteedEnemies = new List<EnemyData>();
            foreach (EnemyData enemyData in EnemyData)
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
            foreach (EnemyData enemyData in guaranteedEnemies)
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
                EnemyData.Remove(enemyData);
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
