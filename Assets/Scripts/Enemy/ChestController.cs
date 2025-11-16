using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestController : EnemyControllerBase
{
    protected override void GetStats(string statLine)
    {
        health = 1; //float.Parse(statLine);
        // provide it some default stats, like knockback
        //knockBackSpeed = 1500f;
        //knockBackTime = 0.2f;
        //knockbackForce = 500f;
        //stunTimer = 0.5f;
    }
    protected override void Deactivate()
    {
        StartCoroutine(DelayedDestroy());
    }
    private IEnumerator DelayedDestroy()
    {
        // Optional: play pot break sound here
        Managers.AudioManager.Instance.PlayCrateBreakSound(1, 0.1f);

        yield return null;  // waits EXACTLY one frame
        base.Deactivate();
        Destroy(gameObject);
    }
}
