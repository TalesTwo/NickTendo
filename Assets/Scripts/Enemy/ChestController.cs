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
            Debug.Log("Chest destroyed");
        }
        if(enemyType == Types.EnemyType.PotEnemy)
        {
            // pot logic here
            Debug.Log("Pot destroyed");
        }
        
    }
}
