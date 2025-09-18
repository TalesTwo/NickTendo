using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public int InvetoryLimit = 3;
    private BaseItem[] ItemList;
    private int ItemListIndex = 0;
    // Start is called before the first frame update
    void Start()
    {
        DebugUtils.Log("Current inventory size: " + ItemListIndex);
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
    public void AddItem(BaseItem Item)
    {

        DebugUtils.Log(Item.name + " was added to the player inventory");

        //turns out passing Objects is kinda annoying so I'll figure something out later


        //if (ItemListIndex <= InvetoryLimit - 1)
        //{
        //    ItemList[ItemListIndex] = Item;
        //    ItemListIndex++;
        //}
        //DebugUtils.Log("Current inventory size: " + ItemListIndex);
    }
}
