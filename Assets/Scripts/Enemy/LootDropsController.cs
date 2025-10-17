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

    private float _totalDropWeight = 0f;

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
        Random generalDropWeight = new Random();
        float drop = (float)generalDropWeight.NextDouble() * 100f;

        if (drop <= generalDropRate)
        {
            float currentDropWeight = 0f;
            Random dropWeight = new Random();
            float itemDrop = (float)dropWeight.NextDouble() * _totalDropWeight;

            foreach (LootDrop lootDrop in lootTable)
            {
                currentDropWeight += lootDrop.dropChance;
                if (currentDropWeight >= itemDrop)
                {
                    Instantiate(lootDrop.drop, transform.position, Quaternion.identity);
                    break;
                }
            }
        }
    }
}
