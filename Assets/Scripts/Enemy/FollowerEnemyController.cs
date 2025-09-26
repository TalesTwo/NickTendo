using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowerEnemyController : EnemyControllerBase
{
    // find the intended direction of movement
    protected override Vector3 GetDirection()
    {
        return (_player.transform.position - transform.position).normalized;
    }
    
    protected override void GetStats(string statLine)
    {
        string[] stats = statLine.Split(',');
        health = float.Parse(stats[0]);
        speed = float.Parse(stats[1]);
        damage = float.Parse(stats[2]);
        knockBackSpeed = float.Parse(stats[3]);
        knockBackTime = float.Parse(stats[4]);
    }
}