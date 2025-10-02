using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopUIManager : MonoBehaviour
{
    public GameObject ShopUI;

    public Image Item0Image;
    public Button Item0Button;
    public TextMeshProUGUI Item0Name;
    public TextMeshProUGUI Item0Desc;
    public Image Item0Spotlight;

    public Image Item1Image;
    public Button Item1Button;
    public TextMeshProUGUI Item1Name;
    public TextMeshProUGUI Item1Desc;
    public Image Item1Spotlight;

    public Image Item2Image;
    public Button Item2Button;
    public TextMeshProUGUI Item2Name;
    public TextMeshProUGUI Item2Desc;
    public Image Item2Spotlight;

    public Button CloseButton;
    public TextMeshProUGUI CoinCount;

    private ShopManager ShopM;

    // Start is called before the first frame update
    void Start()
    {
        CloseButton.onClick.AddListener(CloseShop);
        Item0Button.onClick.AddListener(AttemptBuyItem0);
        Item1Button.onClick.AddListener(AttemptBuyItem1);
        Item2Button.onClick.AddListener(AttemptBuyItem2);

        ShopM = gameObject.GetComponent<ShopManager>();

        UpdateCoinDisplay();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            if (ShopUI != null)
            {
                UpdateCoinDisplay();
                ShopUI.SetActive(true);
                DebugUtils.Log("Key is being pressed");
            }
        }        
    }

    void CloseShop()
    {
        DebugUtils.Log("Close Shop is being called");
        ShopUI.SetActive(false);    
    }

    public void UpdateCoinDisplay()
    {
        CoinCount.text = PlayerStats.Instance.GetCoins().ToString();
    }

    void AttemptBuyItem0()
    {
        ShopM.AttemptBuy(0);
    }

    void AttemptBuyItem1()
    {
        ShopM.AttemptBuy(1);
    }

    void AttemptBuyItem2()
    {
        ShopM.AttemptBuy(2);
    }

    public void RemoveItemFromShop(int Index)
    {
        if (Index == 0)
        {
            Destroy(Item0Image);
            Destroy(Item0Button.gameObject);
            Destroy(Item0Name);
            Destroy(Item0Desc);
            Destroy(Item0Spotlight);
        }
        else if (Index == 1)
        {
            Destroy(Item1Image);
            Destroy(Item1Button.gameObject);
            Destroy(Item1Name);
            Destroy(Item1Desc);
            Destroy(Item1Spotlight);
        }
        else if (Index == 2)
        {
            Destroy(Item2Image);
            Destroy(Item2Button.gameObject);
            Destroy(Item2Name);
            Destroy(Item2Desc);
            Destroy(Item2Spotlight);
        }
    }

    public void MouseOverItem0()
    {
        Item0Spotlight.gameObject.SetActive(true);

    }
    public void MouseOverItem1()
    {
        Item1Spotlight.gameObject.SetActive(true);
    }
    public void MouseOverItem2()
    {
        Item2Spotlight.gameObject.SetActive(true);
    }

    public void MouseOverNoItem()
    {
        if (!Item0Spotlight.IsDestroyed()) { Item0Spotlight.gameObject.SetActive(false); } 
        if (!Item1Spotlight.IsDestroyed()) { Item1Spotlight.gameObject.SetActive(false); }
        if (!Item2Spotlight.IsDestroyed()) { Item2Spotlight.gameObject.SetActive(false); }        
    }
}