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
}