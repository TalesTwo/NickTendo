using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : Singleton<GameStateManager>
{
    /*
     * this class is designed to track the current state of the game.
     */

    public string buddeeDialogState = "Example1";
    
    // getting and setting BUDDEE state
    public string GetBuddeeDialogState() { return buddeeDialogState; }
    public void SetBuddeeDialogState(string newState) { buddeeDialogState = newState; }
}
