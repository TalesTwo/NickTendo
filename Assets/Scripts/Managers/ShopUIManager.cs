using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopUIManager : MonoBehaviour
{
    // The canvas object for the ShopUI
    public GameObject ShopUI;

    // Lists that store all the UI elements that have need functionality
    // (maybe change from public to serialized field
    public Image[] ItemImages;
    public Button[] ItemButtons;
    public TextMeshProUGUI[] ItemNames;
    public TextMeshProUGUI[] ItemDescriptions;
    public Image[] ItemSpotlights;
    public TextMeshProUGUI[] ItemPrices;

    public Button CloseButton;

    private ShopManager ShopM;

    // Start is called before the first frame update
    void Start()
    {
        // Adding all the click events 
        CloseButton.onClick.AddListener(CloseShop);
        ItemButtons[0].onClick.AddListener(delegate { AttemptBuyItem(0); });
        ItemButtons[1].onClick.AddListener(delegate { AttemptBuyItem(1); });
        ItemButtons[2].onClick.AddListener(delegate { AttemptBuyItem(2); });

        ShopM = gameObject.GetComponent<ShopManager>();
    }

    public void OpenShop()
    {
        if (ShopUI != null)
        {
            ShopUI.SetActive(true);
            PlayerUIManager.Instance.ToggleHUD();
            EventBroadcaster.Broadcast_StartStopAction();
        }
    }

    void CloseShop()
    {
        ShopUI.SetActive(false);
        PlayerUIManager.Instance.ToggleHUD();
        EventBroadcaster.Broadcast_StartStopAction();
    }

    void AttemptBuyItem(int index)
    {
        ShopM.AttemptBuy(index);
    }

    public void RemoveItemFromShop(int Index)
    {
        // Removes all the info for a bought item
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