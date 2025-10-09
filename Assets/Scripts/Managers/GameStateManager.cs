using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : Singleton<GameStateManager>
{
    /*
     * this class is designed to track the current state of the game.
     */

    public string buddeeDialogState = "DemoIntro";
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
            buddeeDialogState = "AfterDeath";
        }
        if (_playerDeathCount == 2)
        {
            buddeeDialogState = "FinalRun";
        }
    }
}
