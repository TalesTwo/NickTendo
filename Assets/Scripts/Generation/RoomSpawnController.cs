using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSpawnController : MonoBehaviour
{
    // reference to the room that we are attached to
    [SerializeField]
    private Room room;
    
    
    
    private void Awake()
    {
        room = GetComponentInParent<Room>();
    }

    public void Initialize()
    {
        // fill up the spawn points for enemies, traps, and loot
        // loop for a child object called "EnemySpawnPoints", "TrapSpawnPoints", and "LootSpawnPoints"
        
        // find the rooms chil,d which has the name "EnemySpawnPoints". not the "transform. its an empty game object
        // 🔹 Find the "EnemySpawnPoints" child under this room
        Transform enemyParent = transform.Find("EnemySpawnPoints");
        if (enemyParent != null)
        {
            foreach (Transform child in enemyParent)
            {
                // Add each child GameObject to the list
                room.GetEnemySpawnPoints().Add(child.gameObject);
            }
        }


    }
    
    
    
    
}
