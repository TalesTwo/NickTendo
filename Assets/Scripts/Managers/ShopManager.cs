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
        EventBroadcaster.SetSeed += SetSeed;
        GetRandomShopList();
        ShopUIM = gameObject.GetComponent<ShopUIManager>();
        SetItems();
    }

    void SetSeed(int seed)
    {
        UnityEngine.Random.InitState(seed);
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

    public void GetRandomShopList()
    {
        ShopList = new GameObject[3];

        int[] numbers = new int[ItemList.Length];
        for (int i =0; i<ItemList.Length; i++)
        {
            numbers[i] = i;
        }

        // Do the Fisher-Yates Shuffle
        for (int i = 0; i < numbers.Length; i++)
        {
            int randomIndex = UnityEngine.Random.Range(i, numbers.Length);
            int temp = numbers[i];
            numbers[i] = numbers[randomIndex];
            numbers[randomIndex] = temp;
        }

        ShopList[0] = ItemList[numbers[0]];
        ShopList[1] = ItemList[numbers[1]];
        ShopList[2] = ItemList[numbers[2]];
    }

    public void SetItems()
    {
        for (int i = 0; i <= 2; i++)
        {
            ShopUIM.ItemImages[i].sprite = ShopList[i].gameObject.GetComponent<SpriteRenderer>().sprite;
            ShopUIM.ItemNames[i].text = ShopList[i].GetComponent<ShopItem>().Name;
            //ShopUIM.ItemDescriptions[i].text = ShopList[i].GetComponent<ShopItem>().GetDescription();
            //ShopUIM.ItemPrices[i].text = "Item Price: " + ShopList[i].GetComponent<ShopItem>().itemValue.ToString();
        }
    }

    public void AttemptBuy(int Index)
    {
        ShopItem AttemptItem = ShopList[Index].GetComponent<ShopItem>();
        if (PlayerStats.Instance.GetCoins() >= AttemptItem.itemValue)
        {
            Managers.AudioManager.Instance.PlayItemBuySound(1, 0);
            PlayerStats.Instance.ApplyItemBuffs(PlayerStatsEnum.Coins, -AttemptItem.itemValue);
            PlayerStats.Instance.ApplyItemBuffs(AttemptItem.buffType, AttemptItem.buffValue);
            ShopUIM.RemoveItemFromShop(Index);
        }
        else
        {
            DebugUtils.LogError("Not enough money to buy " + AttemptItem.name + "!");
        }
    }
}