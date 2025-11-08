using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : Singleton<GameStateManager>
{
    /*
     * this class is designed to track the current state of the game.
     */

    public string buddeeDialogState = "vertIntroyell";
    private int _playerDeathCount = 0;
    
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
            buddeeDialogState = "vertDeath1";
        }
        if (_playerDeathCount == 2)
        {
            buddeeDialogState = "vertDeath2";
        }
        
    }

    public void Dialogue(string npcName)
    {
        if (npcName == "BUDDEE")
        {
            if (buddeeDialogState == "vertIntroyell")
            {
                buddeeDialogState = "VertIntro";
            } else if (buddeeDialogState == "VertIntro")
            {
                buddeeDialogState = "vertIntroinfo";
            } else if (buddeeDialogState == "vertIntroinfo")
            {
                buddeeDialogState = "HubRandom";
            } else if (buddeeDialogState == "Vertwin")
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
        buddeeDialogState = "Vertwin";
    }

    public void Start()
    {
        EventBroadcaster.GameRestart += OnGameRestart;
    }
    private void OnGameRestart()
    {
        buddeeDialogState = "vertIntroyell";
        _playerDeathCount = 0;
    }
}
