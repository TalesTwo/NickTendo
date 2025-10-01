using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public GameObject[] ItemList;
    private GameObject[] ShopList;

    private ShopUIManager ShopUI;


    // Start is called before the first frame update
    void Start()
    {
        GetRandomShopList();
        //CheckListContents();
        ShopUI = gameObject.GetComponent<ShopUIManager>();
        SetItem0();
        SetItem1();
        SetItem2();
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

        List<int> numbers = new List<int> { 0, 1, 2, 3, 4, 5 }; // pool

        // Do the Fisher-Yates Shuffle
        for (int i = 0; i < numbers.Count; i++)
        {
            int randomIndex = rand.Next(i, numbers.Count);
            int temp = numbers[i];
            numbers[i] = numbers[randomIndex];
            numbers[randomIndex] = temp;
        }

        ShopList[0] = ItemList[numbers[0]];
        ShopList[1] = ItemList[numbers[1]];
        ShopList[2] = ItemList[numbers[2]];
    }

    void SetItem0()
    {
        ShopUI.Item0Image.sprite = ShopList[0].gameObject.GetComponent<SpriteRenderer>().sprite;
        ShopUI.Item0Name.text = ShopList[0].GetComponent<ShopItem>().Name;
        ShopUI.Item0Desc.text = ShopList[0].GetComponent<ShopItem>().GetDescription();
    }

    void SetItem1()
    {
        ShopUI.Item1Image.sprite = ShopList[1].gameObject.GetComponent<SpriteRenderer>().sprite;
        ShopUI.Item1Name.text = ShopList[1].GetComponent<ShopItem>().Name;
        ShopUI.Item1Desc.text = ShopList[1].GetComponent<ShopItem>().GetDescription();
    }

    void SetItem2()
    {
        ShopUI.Item2Image.sprite = ShopList[2].gameObject.GetComponent<SpriteRenderer>().sprite;
        ShopUI.Item2Name.text = ShopList[2].GetComponent<ShopItem>().Name;
        ShopUI.Item2Desc.text = ShopList[2].GetComponent<ShopItem>().GetDescription();
    }
}
