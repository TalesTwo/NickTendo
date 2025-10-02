using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public GameObject[] ItemList;
    private GameObject[] ShopList;

    private ShopUIManager ShopUIM;


    // Start is called before the first frame update
    void Start()
    {
        GetRandomShopList();
        ShopUIM = gameObject.GetComponent<ShopUIManager>();
        SetItem0();
        SetItem1();
        SetItem2();
        PlayerStats.Instance.DisplayAllBuffableStats();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void CheckListContents()
    {
        DebugUtils.Log("Current player attack: " + PlayerStats.Instance.GetAttackDamage());
        if (ShopList != null)
        {
            for (int i = 0; i < ShopList.Length; i++)
            {
                DebugUtils.Log(ShopList[i].GetComponent<ShopItem>().GetDescription());

            }
        }
    }

    void GetRandomShopList()
    {
        ShopList = new GameObject[3];
        System.Random rand = new System.Random();

        int[] numbers = new int[ItemList.Length];
        for (int i =0; i<ItemList.Length; i++)
        {
            numbers[i] = i;
        }

        // Do the Fisher-Yates Shuffle
        for (int i = 0; i < numbers.Length; i++)
        {
            int randomIndex = rand.Next(i, numbers.Length);
            int temp = numbers[i];
            numbers[i] = numbers[randomIndex];
            numbers[randomIndex] = temp;
        }

        ShopList[0] = ItemList[numbers[0]];
        ShopList[1] = ItemList[numbers[1]];
        ShopList[2] = ItemList[numbers[2]];
    }


    //probably move these to the Shop UI Manager (makes more sense there)
    void SetItem0()
    {
        ShopUIM.Item0Image.sprite = ShopList[0].gameObject.GetComponent<SpriteRenderer>().sprite;
        ShopUIM.Item0Name.text = ShopList[0].GetComponent<ShopItem>().Name;
        ShopUIM.Item0Desc.text = ShopList[0].GetComponent<ShopItem>().GetDescription();
    }

    void SetItem1()
    {
        ShopUIM.Item1Image.sprite = ShopList[1].gameObject.GetComponent<SpriteRenderer>().sprite;
        ShopUIM.Item1Name.text = ShopList[1].GetComponent<ShopItem>().Name;
        ShopUIM.Item1Desc.text = ShopList[1].GetComponent<ShopItem>().GetDescription();
    }

    void SetItem2()
    {
        ShopUIM.Item2Image.sprite = ShopList[2].gameObject.GetComponent<SpriteRenderer>().sprite;
        ShopUIM.Item2Name.text = ShopList[2].GetComponent<ShopItem>().Name;
        ShopUIM.Item2Desc.text = ShopList[2].GetComponent<ShopItem>().GetDescription();
    }

    public void AttemptBuy(int Index)
    {
        if (PlayerStats.Instance.GetCoins() >= ShopList[Index].GetComponent<ShopItem>().itemValue)
        {
            
            PlayerStats.Instance.UpdateCoins(-ShopList[Index].GetComponent<ShopItem>().itemValue);
            ApplyBuffs(ShopList[Index].GetComponent<ShopItem>());
            PlayerStats.Instance.DisplayAllBuffableStats();
            ShopUIM.UpdateCoinDisplay();
            ShopUIM.RemoveItemFromShop(Index);
        }
        else
        {
            DebugUtils.LogError("Not enough money to buy " + ShopList[Index].GetComponent<ShopItem>().name + "!");
        }
    }

    void ApplyBuffs(ShopItem Item)
    {
        DebugUtils.Log("Apply buff is called");
        if (Item.buffType == PlayerStatsEnum.Max_Health)
        {
            PlayerStats.Instance.UpdateMaxHealth(Item.buffValue);
        }
        else if (Item.buffType == PlayerStatsEnum.Movement_Speed)
        {
            PlayerStats.Instance.UpdateMovementSpeed(Item.buffValue);
        }
        else if (Item.buffType == PlayerStatsEnum.Dash_Speed)
        {
            PlayerStats.Instance.UpdateDashSpeed(Item.buffValue);
        }
        else if (Item.buffType == PlayerStatsEnum.Attack_Damage)
        {
            PlayerStats.Instance.UpdateAttackDamage(Item.buffValue);
        }
        else if (Item.buffType == PlayerStatsEnum.Dash_Damage)
        {
            PlayerStats.Instance.UpdateDashDamage(Item.buffValue);
        }
        else if (Item.buffType == PlayerStatsEnum.Dash_Cooldown)
        {
            PlayerStats.Instance.UpdateDashCooldown(Item.buffValue);
        }
        else if (Item.buffType == PlayerStatsEnum.Attack_Cooldown)
        {
            PlayerStats.Instance.UpdateAttackCooldown(Item.buffValue);
        }
        else if (Item.buffType == PlayerStatsEnum.Dash_Distance)
        {
            PlayerStats.Instance.UpdateDashDistance(Item.buffValue);
        }
    }
}