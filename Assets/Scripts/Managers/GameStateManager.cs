using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : Singleton<GameStateManager>
{
    /*
     * this class is designed to track the current state of the game.
     */

    public string buddeeDialogState = "Introyell";
    private int _playerDeathCount = 0; public int GetPlayerDeathCount() { return _playerDeathCount; }
    private int _timesTalkedToShopkeeper = 0; public void UpdateNumberOfTimesTalkedToShopkeeper() { _timesTalkedToShopkeeper++; } public int GetNumberOfTimesTalkedToShopkeeper() { return _timesTalkedToShopkeeper; }
    private List<ShopTriggerInteraction> _ShopKeepersTalkedTo = new List<ShopTriggerInteraction>(); public void AddShopKeeperTalkedTo(ShopTriggerInteraction shopKeeper) { if (!_ShopKeepersTalkedTo.Contains(shopKeeper)) { _ShopKeepersTalkedTo.Add(shopKeeper); UpdateNumberOfTimesTalkedToShopkeeper(); } } public List<ShopTriggerInteraction> GetShopKeepersTalkedTo() { return _ShopKeepersTalkedTo; }

    public bool hasOpenedLauncher;

    // getting and setting BUDDEE state
    public string GetBuddeeDialogState()
    {
        return buddeeDialogState;
    }

    public void SetBuddeeDialogState(string newState)
    {
        buddeeDialogState = newState;
    }

    public void PlayerDeath()
    {
        _playerDeathCount++;
        if (_playerDeathCount == 1)
        {
            buddeeDialogState = "Run2";
        }
        if (_playerDeathCount == 2)
        {
            buddeeDialogState = "Run3";
        }
        if (_playerDeathCount == 3)
        {
            buddeeDialogState = "Run4";
        }
        if (_playerDeathCount == 4)
        {
            buddeeDialogState = "Run5";
        }

    }

    public void Dialogue(string npcName)
    {
        if (npcName == "BUDDEE")
        {
            if (buddeeDialogState == "Introyell")
            {
                buddeeDialogState = "TutorialIntro";
            }
            else if (buddeeDialogState == "TutorialIntro")
            {
                buddeeDialogState = "HubRandom";
            }
            else if (buddeeDialogState == "Vertwin")
            {
                CreditsManager.Instance.BeginCredits();
            }
            else
            {
                buddeeDialogState = "HubRandom";
            }
            //Debug.Log(npcName);
        }
    }

    public void SetEndGameFlag()
    {
        buddeeDialogState = "End";
    }

    public void Start()
    {
        EventBroadcaster.GameRestart += OnGameRestart;
        EventBroadcaster.GameStarted += OnGameStarted;
        hasOpenedLauncher = false;
    }
    private void OnGameRestart()
    {
        buddeeDialogState = "Introyell";
        _playerDeathCount = 0;
    }

    private void OnGameStarted()
    {
        buddeeDialogState = "Introyell";
        _playerDeathCount = 0;
        EventBroadcaster.Broadcast_StartDialogue("BUDDEE");
    }
}
