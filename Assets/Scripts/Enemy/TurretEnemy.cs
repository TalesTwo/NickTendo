using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretEnemy : RangedEnemyController
{
    [Header("Safe Distance from Player")]
    private float _minDistanceToPlayer;
    private float _maxDistanceToPlayer;
    
    [Header("Attack")]
    public GameObject projectile;
    public LayerMask doNotHit;
    public float attackSpawnDistance;
    private float _attackCooldownMax;
    private float _attackCooldownMin;
    private float _attackCooldown;
    private float _attackTimer = 0;
    private float _projectileSpeed;
    
    
    protected override void GetStats(string statLine)
    {
        string[] stats = statLine.Split(',');
        health = float.Parse(stats[0]);
        speed = float.Parse(stats[1]);
        damage = float.Parse(stats[2]);
        knockBackSpeed = float.Parse(stats[3]);
        knockBackTime = float.Parse(stats[4]);
        _minDistanceToPlayer = float.Parse(stats[5]);
        _maxDistanceToPlayer = float.Parse(stats[6]);
        _attackCooldownMin = float.Parse(stats[7]);
        _attackCooldownMax = float.Parse(stats[8]);
        _projectileSpeed = float.Parse(stats[9]);
        knockbackForce =  float.Parse(stats[10]);
        stunTimer = float.Parse(stats[11]);
        _attackCooldown = Random.Range(_attackCooldownMin, _attackCooldownMax);
        findPathCooldown = 1f / (speed*2);
        base.GetStats(statLine);
    }

    protected override void FindPath() { return; }
    protected override IEnumerator Follow() { return null; }
}
