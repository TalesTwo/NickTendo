using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ShopInventory : MonoBehaviour
{
    public GameObject[] ItemList;
    // Start is called before the first frame update
    void Start()
    {
        //SayItemList("Shop Inventory");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SayItemList(string ClassName)
    {
        
    }

    public GameObject[] GetItemList()
    {
        return ItemList;
    }
}