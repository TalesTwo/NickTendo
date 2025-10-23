using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestController : EnemyControllerBase
{
    protected override void GetStats(string statLine)
    {
        health = float.Parse(statLine);
    }
}
