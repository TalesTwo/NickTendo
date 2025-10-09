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

    public Image[] ItemImages;
    public Button[] ItemButtons;
    public TextMeshProUGUI[] ItemNames;
    public TextMeshProUGUI[] ItemDescriptions;
    public Image[] ItemSpotlights;
    public TextMeshProUGUI[] ItemPrices;

    public Button CloseButton;
    /*public TextMeshProUGUI CoinCount;*/

    private ShopManager ShopM;

    // Start is called before the first frame update
    void Start()
    {
        CloseButton.onClick.AddListener(CloseShop);
        ItemButtons[0].onClick.AddListener(delegate { AttemptBuyItem(0); });
        ItemButtons[1].onClick.AddListener(delegate { AttemptBuyItem(1); });
        ItemButtons[2].onClick.AddListener(delegate { AttemptBuyItem(2); });

        ShopM = gameObject.GetComponent<ShopManager>();

        /*UpdateCoinDisplay();*/
    }

    // Update is called once per frame
    void Update()
    {
        /*if(Input.GetKeyDown(KeyCode.P))
        {
            OpenShop();
        } */       
    }

    public void OpenShop()
    {
        if (ShopUI != null)
        {
            /*UpdateCoinDisplay();*/
            ShopUI.SetActive(true);
            EventBroadcaster.Broadcast_StartStopAction();
        }
    }

    void CloseShop()
    {
        ShopUI.SetActive(false);
        EventBroadcaster.Broadcast_StartStopAction();
    }

    /*public void UpdateCoinDisplay()
    {
        CoinCount.text = PlayerStats.Instance.GetCoins().ToString();
    }*/

    void AttemptBuyItem(int index)
    {
        ShopM.AttemptBuy(index);
    }

    public void RemoveItemFromShop(int Index)
    {
        Destroy(ItemImages[Index]);
        Destroy(ItemButtons[Index].gameObject);
        Destroy(ItemNames[Index]);
        Destroy(ItemDescriptions[Index]);
        Destroy(ItemSpotlights[Index]);
        Destroy(ItemPrices[Index]);
    }

    public void MouseOverItem(int Index)
    {
        ItemSpotlights[Index].gameObject.SetActive(true);

    }

    public void MouseLeftItem(int Index)
    {
        if (!ItemSpotlights[Index].IsDestroyed()) { ItemSpotlights[Index].gameObject.SetActive(false); }      
    }
}