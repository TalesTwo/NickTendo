using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
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
    public float dropForce = 0.5f;
    public float dropDrag = 5f;
    public int DropAmount = 1;

    private float _totalDropWeight = 0f;
    
    
    private int _numberOfDrops = 0;

    private GameObject player;
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
        if (!gameObject.scene.isLoaded || !Application.isPlaying) {return;}
        
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
                    // temp remove
                    //AddForceToItem(spawnedItem);
                    _numberOfDrops += 1;
                    break;
                }
            }
        }
    }
    
    private void AddForceToItem(GameObject item)
    {
        Rigidbody2D rb = item.GetComponent<Rigidbody2D>();

        rb.gravityScale = 0f;
        rb.drag = dropDrag;
        rb.freezeRotation = true;

        // get the player
        GameObject playerObj = PlayerManager.Instance.GetPlayer();
        if (playerObj == null) return;

        item.GetComponent<BaseItem>().TemporarilyDisableCollision(0.25f);

        

        // direction FROM player TO the loot drop
        Vector2 baseDirection = (item.transform.position - playerObj.transform.position).normalized;

        // apply a random ±45° variation
        float randomAngle = UnityEngine.Random.Range(-45f, 45f);

        Vector2 finalDirection = Quaternion.Euler(0, 0, randomAngle) * baseDirection;

        // apply scattering velocity
        rb.velocity = finalDirection * dropForce;
    }


}
