using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public GameObject[] ItemList;
    public GameObject[] PermaItemList;

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

    public void GetRandomShopList()
    {
        ShopList = new GameObject[4];

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
        //ShopList[3] = ItemList[numbers[3]];


        int RandomInt = UnityEngine.Random.Range(0, PermaItemList.Length);
        ShopList[3] = PermaItemList[RandomInt];
    }

    public void SetItems()
    {
        for (int i = 0; i <= 3; i++)
        {
            ShopUIM.ItemSlots[i].SetActive(true);
            ShopUIM.ItemSpotlights[i].gameObject.SetActive(false);
            ShopUIM.ItemImages[i].sprite = ShopList[i].gameObject.GetComponent<SpriteRenderer>().sprite;
            ShopUIM.ItemNames[i].text = ShopList[i].GetComponent<ShopItem>().Name;
            ShopUIM.ItemTooltipText[i] = ShopList[i].GetComponent<ShopItem>().GetTooltipText();
            //ShopUIM.ItemFlavorText[i] = ShopList[i].GetComponent<ShopItem>().flavorText;
            ShopUIM.ItemFlavorText[i] = ShopList[i].GetComponent<ShopItem>().flavorTextUUGUI;
            ShopUIM.ItemEmotes[i] = ShopList[i].GetOrAddComponent<ShopItem>().emote;
        }
    }

    public void AttemptBuy(int Index)
    {
        ShopItem AttemptItem = ShopList[Index].GetComponent<ShopItem>();
        if (Index != 3)
        {
            if (PlayerStats.Instance.GetCoins() >= AttemptItem.itemValue)
            {
                Managers.AudioManager.Instance.PlayItemBuySound(1, 0);
                PlayerStats.Instance.ApplyItemBuffs(PlayerStatsEnum.Coins, -AttemptItem.itemValue);
                PlayerStats.Instance.ApplyItemBuffs(AttemptItem.buffType, AttemptItem.buffValue);
                ShopUIM.RemoveItemFromShop(Index);
            }
            else
            {
                ShopUIM.NotEnoughMoney();
            }
        }
        else
        {
            if (PlayerStats.Instance.GetChips() >= AttemptItem.itemValue)
            {
                Managers.AudioManager.Instance.PlayItemBuySound(1, 0);
                PlayerStats.Instance.ApplyItemBuffs(PlayerStatsEnum.Chips, -AttemptItem.itemValue);
                PlayerStats.Instance.ApplyItemBuffs(AttemptItem.buffType, AttemptItem.buffValue);
                ShopUIM.RemoveItemFromShop(Index);
            }
            else
            {
                ShopUIM.NotEnoughChips();
            }
        }
    }
}