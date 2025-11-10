using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class LootDropsController : MonoBehaviour
{

    [System.Serializable]
    public class LootDrop
    {
        public GameObject drop;
        [Range(0f, 100f)]
        public float dropChance;
    }

    [Header("Loot Table")]
    public List<LootDrop> lootTable;
    [Range(0f, 100f)]
    public float generalDropRate;
    public float dropForce = 5f;
    public int DropAmount = 1;

    private float _totalDropWeight = 0f;
    
    
    private int _numberOfDrops = 0;

    // establish the weight of the full pool
    private void Start()
    {
        foreach (LootDrop lootDrop in lootTable)
        {
            _totalDropWeight += lootDrop.dropChance;
        }
    }

    // when the enemy is defeated, they will drop an item with set odds
    private void OnDestroy()
    {
        for (int i = 0; i < DropAmount; i++)
        {
            AttemptDrop();
        }

    }

    private void AttemptDrop()
    {
        
        // get a random float between 0 and 100, to see if we even drop something
        float drop = UnityEngine.Random.Range(0f, 100f);

        if (drop <= generalDropRate)
        {
            // recalculate total drop weight to ensure it’s correct
            _totalDropWeight = 0f;
            foreach (LootDrop lootDrop in lootTable)
            {
                _totalDropWeight += lootDrop.dropChance;
            }

            float currentDropWeight = 0f;
            float itemDrop = UnityEngine.Random.Range(0f, _totalDropWeight);

            foreach (LootDrop lootDrop in lootTable)
            {
                currentDropWeight += lootDrop.dropChance;
                if (currentDropWeight >= itemDrop)
                {
                    GameObject spawnedItem = Instantiate(lootDrop.drop, transform.position, Quaternion.identity);
                    AddForceToItem(spawnedItem, lootDrop);
                    break;
                }
            }
        }
    }
    
    private void AddForceToItem(GameObject item, LootDrop lootDrop)
    {
        
        // Ensure it has a Rigidbody2D
        Rigidbody2D itemRb = item.GetComponent<Rigidbody2D>();
        if (itemRb == null)
        {
            itemRb = item.AddComponent<Rigidbody2D>();
            // disable gravity (otherwise we "fall" to the bottom of the tilemap)
            itemRb.gravityScale = 0f;
            // we need to add a "drag" so it slows down over time otherwise it flies away lol
            itemRb.drag = 2f;
        }

        // Get player position
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) { return; }
        Vector2 awayFromPlayer = (item.transform.position - player.transform.position).normalized;

        // Rotate that vector by a random angle (±range around the opposite direction)
        float randomAngle = UnityEngine.Random.Range(-45f, 45f); 
        Vector2 randomDir = Quaternion.Euler(0, 0, randomAngle) * awayFromPlayer;

        // Apply the force
        float dropForce = 5f; // adjust as needed
        itemRb.AddForce(randomDir * dropForce, ForceMode2D.Impulse);
        DebugUtils.Log($"Dropped item: {lootDrop.drop.name} from enemy: {gameObject.name}");
        _numberOfDrops += 1;
        DebugUtils.Log($"Total drops so far: {_numberOfDrops}");

    }
}
