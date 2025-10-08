using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonController : MonoBehaviour
{
    /*
     * This will be in charge of over-arching things that need to happen for the entire dungeon
     * Specifically during gameplay, this will be in charge of:
     * Ensuring that all rooms are disabled untill the player is about to enter them
     * 
     */
    
    private (int row, int col) CurrentRoomCoords = (-1, -1); // what are the current coordinates of the player in the dungeon?
    public void SetCurrentRoomCoords(int row, int col) { CurrentRoomCoords = (row, col); }


    public void Update()
    {

    }
}
