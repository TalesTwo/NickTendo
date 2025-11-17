using JetBrains.Annotations;
using Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : BaseItem
{
    public int itemValue = 100;
    public float buffValue = 10;
    public PlayerStatsEnum buffType;
    public string flavorText;
    public bool isPermaUpgrade = false;
    // Just returns the desctiption for the item
    public string GetTooltipText()
    {
        string itemDescription = "";
        itemDescription += "Stat: " + AddSpace(buffType) + "\n";
        itemDescription += BuffText();
        if (isPermaUpgrade)
        {
            itemDescription += "Cost: " + itemValue + " chip\n";
        }
        else
        {
            itemDescription += "Cost: " + itemValue + " coins\n";
        }
        itemDescription += "Click to buy!\n\n";
        return itemDescription;
    }

    // Replaces the '_' with a space for a better look
    string AddSpace(PlayerStatsEnum nameInEnum)
    {
        string[] splitName = nameInEnum.ToString().Split('_');
        if (splitName[0] == "CarryOver")
        {
            splitName[0] = splitName[1];
            splitName[1] = splitName[2];
            splitName[2] = "";
        }
        return String.Join(" ", splitName);
    }

    string BuffText()
    {
        string returnText = "";
        if (buffType == PlayerStatsEnum.Current_Health) 
        { 
            returnText = "Heal by: " + Mathf.RoundToInt((buffValue / PlayerStats.Instance.GetMaxHealth() * 100)) + "%\n";
        }
        else if (buffType == PlayerStatsEnum.Max_Health)
        {
            returnText = "Increase by: " + Mathf.RoundToInt((buffValue / PlayerStats.Instance.GetMaxHealth() * 100)) + "%\n";
        }
        else if (buffType == PlayerStatsEnum.Movement_Speed)
        {
            returnText = "Increase by: " + Mathf.RoundToInt((buffValue / PlayerStats.Instance.GetMovementSpeed() * 100)) + "%\n";
        }
        else if (buffType == PlayerStatsEnum.Dash_Speed)
        {
            returnText = "Increase by: " + Mathf.RoundToInt((buffValue / PlayerStats.Instance.GetDashSpeed() * 100)) + "%\n";
        }
        else if (buffType == PlayerStatsEnum.Attack_Damage)
        {
            returnText = "Increase by: " + Mathf.RoundToInt((buffValue / PlayerStats.Instance.GetAttackDamage() * 100)) + "%\n";
        }
        else if (buffType == PlayerStatsEnum.Dash_Damage)
        {
            returnText = "Increase by: " + Mathf.RoundToInt((buffValue / PlayerStats.Instance.GetDashDamage() * 100)) + "%\n";
        }
        else if (buffType == PlayerStatsEnum.Attack_Cooldown)
        {
            returnText = "Decrease by: " + buffValue + " sec\n";
        }
        else if (buffType == PlayerStatsEnum.Dash_Cooldown)
        {
            returnText = "Decrease by: " + buffValue + " sec\n";
        }
        else if (buffType == PlayerStatsEnum.CarryOver_Max_Health)
        {
            returnText = "Permanently increase by: " + Mathf.RoundToInt((buffValue / PlayerStats.Instance.GetMaxHealth() * 100)) + "%\n";
        }
        else if (buffType == PlayerStatsEnum.CarryOver_Attack_Damage)
        {
            returnText = "Permanently increase by: " + Mathf.RoundToInt((buffValue / PlayerStats.Instance.GetAttackDamage() * 100)) + "%\n";
        }
        else if (buffType == PlayerStatsEnum.CarryOver_Movement_Speed)
        {
            returnText = "Permanently increase by: " + Mathf.RoundToInt((buffValue / PlayerStats.Instance.GetMovementSpeed() * 100)) + "%\n";
        }
        else if (buffType == PlayerStatsEnum.CarryOver_Dash_Damage)
        {
            returnText = "Permanently increase by: " + Mathf.RoundToInt((buffValue / PlayerStats.Instance.GetDashDamage() * 100)) + "%\n";
        }
        else if (buffType == PlayerStatsEnum.CarryOver_Dash_Speed)
        {
            returnText = "Permanently increase by: " + Mathf.RoundToInt((buffValue / PlayerStats.Instance.GetDashSpeed() * 100)) + "%\n";
        }
        else if (buffType == PlayerStatsEnum.CarryOver_Attack_Cooldown)
        {
            returnText = "Permanently decrease by: " + buffValue + " sec\n";
        }
        else if (buffType == PlayerStatsEnum.CarryOver_Dash_Cooldown)
        {
            returnText = "Permanently decrease by: " + buffValue + " sec\n";
        }

        return returnText;
    }
}
