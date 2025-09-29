using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FollowerEnemyController : EnemyControllerBase
{
    // ignore layermask
    [Header("RayCasting")]
    public LayerMask ignoreLayer;
    public float precision;
    public float wallBuffer;
    
    
    // find the intended direction of movement
    protected override Vector3 GetDirection()
    {
        // Step 1: RayCast towards the player
        Vector3 dir = (_player.transform.position - transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, Mathf.Infinity, ~ignoreLayer);
        if (hit.collider != null)
        {
            // Step 1.1: Player Found, go to Player
            if (hit.collider.gameObject.CompareTag("Player"))
            {
                return dir;
            }
            
        }

        return Vector3.zero;
    }
    
    
    
    /*
    private bool IsTooCloseToWall(Vector2 cellPosition)
    {
        Vector2[] directions = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

        foreach (Vector2 dir in directions)
        {
            foreach (Vector2 dir2 in directions)
            {
                if (dir != dir2)
                {
                    RaycastHit2D hit = Physics2D.Raycast(cellPosition, dir+dir2, wallBuffer, ~ignoreLayer);
                    if (hit.collider != null)
                    {
                        if (!hit.collider.gameObject.CompareTag("Player"))
                        {
                            return true;
                        }
                    }                    
                }
            }

        }

        return false;
    }
    */
    
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