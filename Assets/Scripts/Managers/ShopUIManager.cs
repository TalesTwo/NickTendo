using Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEditor.Rendering.Universal;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopUIManager : MonoBehaviour
{
    // The canvas object for the ShopUI
    public GameObject ShopUI;

    // Lists that store all the UI elements that have need functionality
    // (maybe change from public to serialized field)
    public Image[] ItemImages;
    public Button[] ItemButtons;
    public TextMeshProUGUI[] ItemNames;
    public string[] ItemDescriptions;
    public string[] ItemFlavorText;
    public Image[] ItemSpotlights;

    public Button CloseButton;
    public Button RerollButton;
    public int RerollCost = 50;
    public GameObject Tooltip;
    public GameObject BuddeeUI;
    public bool IsInShop;

    private ShopManager ShopM;
    private bool IsRerollButton;
    private int TooltipIndex;
    private bool IsFirstOpen;

    // Start is called before the first frame update
    void Start()
    {
        // Adding all the click events 
        CloseButton.onClick.AddListener(CloseShop);
        RerollButton.onClick.AddListener(RerollClick);
        ItemButtons[0].onClick.AddListener(delegate { AttemptBuyItem(0); });
        ItemButtons[1].onClick.AddListener(delegate { AttemptBuyItem(1); });
        ItemButtons[2].onClick.AddListener(delegate { AttemptBuyItem(2); });

        IsInShop = false;
        IsRerollButton = false;
        TooltipIndex = 0;
        ItemDescriptions = new string[3];
        ItemFlavorText = new string[3];
        IsFirstOpen = true;

        ShopM = gameObject.GetComponent<ShopManager>();
    }

    public void OpenShop()
    {
        if (ShopUI != null)
        {
            EventBroadcaster.Broadcast_PlayerOpenMenu();
            ShopUI.SetActive(true);
            if (IsFirstOpen)
            {
                RerollItems(true);
                IsFirstOpen = false;
            }
            PlayerUIManager.Instance.ToggleHUD();
            EventBroadcaster.Broadcast_StartStopAction();
            IsInShop = true;
            BuddeeUI.GetComponent<BUDDEEUI>().SetDialogue("Hover over the items to learn more!");
        }
    }

    void CloseShop()
    {
        EventBroadcaster.Broadcast_PlayerCloseMenu();
        ShopUI.SetActive(false);
        PlayerUIManager.Instance.ToggleHUD();
        EventBroadcaster.Broadcast_StartStopAction();
        IsInShop = false;
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
        Destroy(ItemSpotlights[Index]);
        Tooltip.GetComponent<TooltipUI>().HideTooltip();
    }

    public void MouseOverItem(int Index)
    {
        ItemSpotlights[Index].gameObject.SetActive(true);
        BuddeeUI.GetComponent<BUDDEEUI>().StopCR();
        BuddeeUI.GetComponent<BUDDEEUI>().SetDialogue(ItemFlavorText[Index]);
    }

    public void MouseLeftItem(int Index)
    {
        if (!ItemSpotlights[Index].IsDestroyed())
        {
            ItemSpotlights[Index].gameObject.SetActive(false);
        }

        BuddeeUI.GetComponent<BUDDEEUI>().StopCR();
        BuddeeUI.GetComponent<BUDDEEUI>().SetDialogue("Hover over the items to learn more!");        
    }

    public void ShowTooltip(bool status)
    {
        if (status)
        {
            if (IsRerollButton)
            {
                Tooltip.GetComponent<TooltipUI>().SetXPadding(75);
                Tooltip.GetComponent<TooltipUI>().SetYPadding(-50);
                Tooltip.GetComponent<TooltipUI>().ShowTooltip("Click to reroll shop items!\nCost: " + RerollCost + "\n\n");
            }
            else
            {
                Tooltip.GetComponent<TooltipUI>().ResetPadding();
                Tooltip.GetComponent<TooltipUI>().ShowTooltip(ItemDescriptions[TooltipIndex]);
            }
        }
        else
        {
            Tooltip.GetComponent<TooltipUI>().HideTooltip();
        }
    }

    public void SetIsReroolBool(bool status)
    {
        if (status) { IsRerollButton = true; }
        else { IsRerollButton = false; }
    }

    public void SetShopItemIndex(int Index)
    {
        TooltipIndex = Index;
    }

    public void RerollClick()
    {
        RerollItems(false);
    }

    void RerollItems(bool DontCareAboutMoney)
    {   
        if (DontCareAboutMoney || PlayerStats.Instance.GetCoins() >= RerollCost)
        {
            if(!DontCareAboutMoney) PlayerStats.Instance.ApplyItemBuffs(PlayerStatsEnum.Coins,-RerollCost);
            ShopM.GetRandomShopList();
            ShopM.SetItems();
        }
        else
        {
            NotEnoughMoney();
        }        
    }

    public void NotEnoughMoney()
    {
        BuddeeUI.GetComponent<BUDDEEUI>().StopCR();
        BuddeeUI.GetComponent<BUDDEEUI>().SetDialogue("Looks like you don't have enough money...");
    }
}