using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : BaseItem
{
    public int itemValue = 100;
    public float buffValue = 10;
    public PlayerStatsEnum buffType;
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
        string returnSting = "";
        if (buffValue >= 0)
        {
            returnSting = Name + " increases " + AddSpace(buffType) + " by " + buffValue;
        }
        else
        {
            returnSting = Name + " decreases " + AddSpace(buffType) + " by " + -buffValue;
        }
        return returnSting;
    }

    string AddSpace(PlayerStatsEnum nameInEnum)
    {
        string[] splitName = nameInEnum.ToString().Split('_');
        return String.Join(" ", splitName);
    }
}
