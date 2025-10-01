using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : BaseItem
{
    public int itemValue = 100;
    public float buffValue = 10;
    //public Image ItemImage;
    
    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Use()
    {
        //just for testing]
        //after testing is done replace it so it can work with every stat
        PlayerStats.Instance.UpdateAttackDamage(buffValue);
    }

    public string GetDescription()
    {
        //change so attack is replaced by stat type
        return (Name + " increases attack by " + buffValue);
    }
}
