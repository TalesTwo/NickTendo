using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestController : EnemyControllerBase
{
    protected override void GetStats(string statLine)
    {
        health = float.Parse(statLine);
    }
    
    protected override void Deactivate() 
    {
        base.Deactivate();
        // specific to ranged enemy deactivation logic can go here
        if(enemyType == Types.EnemyType.ChestEnemy)
        {
            // chest logic here
            Managers.AudioManager.Instance.PlayCrateBreakSound(1,0.1f);
            Debug.Log("Chest destroyed");
        }
        if(enemyType == Types.EnemyType.PotEnemy)
        {
            // pot logic here
            Debug.Log("Pot destroyed");
            Managers.AudioManager.Instance.PlayCrateBreakSound(1, 0.1f);
        }
        
        EventBroadcaster.Broadcast_EnemyDeath(this, GetComponentInParent<Room>());
        
    }
}
