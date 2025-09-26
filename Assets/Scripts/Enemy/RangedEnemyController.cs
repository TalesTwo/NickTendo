using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemyController : EnemyControllerBase
{
    [Header("distance from player")]
    private float _distanceToPlayer;
    
    [Header("Safe Distance from Player")]
    private float _minDistanceToPlayer;
    private float _maxDistanceToPlayer;

    [Header("Strafing")]
    private float _strafingInterval;
    private float _strafingTimer;
    private int _strafingDirection = 1;
    private bool _isStrafing = false;
    
    protected override Vector3 GetDirection()
    {
        Vector3 direction = Vector3.zero;
        Vector3 directionToPlayer = (_player.transform.position - transform.position).normalized;
        GetDistanceToPlayer();

        if (_distanceToPlayer < _minDistanceToPlayer)
        {
            direction = -directionToPlayer;
            _isStrafing = false;
        }
        else if (_distanceToPlayer > _maxDistanceToPlayer)
        {
            direction = directionToPlayer;
            _isStrafing = false;
        }
        else
        {
            direction = StrafeDirection(directionToPlayer);
            _isStrafing = true;
        }
        
        return direction;
    }

    protected override void Move()
    {
        
        if (!_isStrafing)
        {
            transform.Translate(_direction * (speed * Time.deltaTime), Space.World);
        }
        else
        {
            transform.Translate(_direction * (_strafingDirection * speed * Time.deltaTime), Space.World);
        }
        
    }

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
        _strafingInterval = float.Parse(stats[7]);
    }

    private void GetDistanceToPlayer()
    {
        _distanceToPlayer = Vector3.Distance(transform.position, _player.transform.position);
    }

    private Vector3 StrafeDirection(Vector3 directionToPlayer)
    {
        // do not allow the enemy to strafe in any one direction for too long
        _strafingTimer += Time.deltaTime;
        if (_strafingTimer >= _strafingInterval)
        {
            _strafingDirection *= -1;
            _strafingTimer = 0;
        }
        
        Vector3 direction = new Vector3(-directionToPlayer.y, directionToPlayer.x, 0);
        
        return direction;
    }
}