using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemyController : EnemyControllerBase
{
    private bool _isStraffing = false;
    private float _distanceToPlayer;
    
    public float minDistanceToPlayer;
    public float maxDistanceToPlayer;

    public float strafingInterval;
    private float _strafingTimer;
    private int _strafingDirection = 1;
    
    protected override Vector3 GetDirection()
    {
        Vector3 direction = Vector3.zero;
        Vector3 directionToPlayer = (_player.transform.position - transform.position).normalized;
        GetDistanceToPlayer();

        if (_distanceToPlayer < minDistanceToPlayer)
        {
            direction = -directionToPlayer;
            _isStraffing = false;
        }
        else if (_distanceToPlayer > maxDistanceToPlayer)
        {
            direction = directionToPlayer;
            _isStraffing = false;
        }
        else
        {
            direction = StrafeDirection(directionToPlayer);
            _isStraffing = true;
        }
        
        return direction;
    }

    protected override void Move()
    {
        
        if (!_isStraffing)
        {
            transform.Translate(_direction * (speed * Time.deltaTime), Space.World);
        }
        else
        {
            transform.Translate(_direction * (_strafingDirection * speed * Time.deltaTime), Space.World);
        }
        
    }

    private void GetDistanceToPlayer()
    {
        _distanceToPlayer = Vector3.Distance(transform.position, _player.transform.position);
    }

    private Vector3 StrafeDirection(Vector3 directionToPlayer)
    {
        // do not allow the enemy to strafe in any one direction for too long
        _strafingTimer += Time.deltaTime;
        if (_strafingTimer >= strafingInterval)
        {
            _strafingDirection *= -1;
            _strafingTimer = 0;
        }
        
        Vector3 direction = new Vector3(-directionToPlayer.y, directionToPlayer.x, 0);
        
        return direction;
    }
}