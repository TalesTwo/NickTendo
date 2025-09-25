using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemyController : EnemyControllerBase
{
    protected override Vector3 GetDirection()
    {
        return Vector3.zero;
    }
}