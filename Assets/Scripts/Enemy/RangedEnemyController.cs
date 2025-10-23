using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RangedEnemyController : EnemyControllerBase
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

    // this method handles ranged enemies combat attacks
    protected override void Attack()
    {
        _attackTimer += Time.deltaTime;

        // raycast for player, do not shoot unless you can see him (we also will ignore pits)
        int finalMask = doNotHit | LayerMask.GetMask("Pits");
        RaycastHit2D hit = Physics2D.Raycast(_transform.position, _direction, float.MaxValue, ~finalMask);


        if (hit.collider != null)
        {
            
            if (hit.collider.CompareTag("Player"))
            {
                
                if (_attackTimer > _attackCooldown)
                {
                    // instantiate a projectile and give it velocity
                    Vector2 attackPosition = new Vector2(_transform.position.x + _direction.x * attackSpawnDistance, _transform.position.y + _direction.y * attackSpawnDistance);
                    GameObject newProjectile = Instantiate(projectile, attackPosition, Quaternion.identity);
                    Rigidbody2D ProjectileRb = newProjectile.GetComponent<Rigidbody2D>();
                    ProjectileRb.velocity = _direction * _projectileSpeed;
                    newProjectile.GetComponent<EnemyProjectileController>().SetAngle(_direction);
                    Managers.AudioManager.Instance.PlayEnemyShotSound();
                    
                    // set damage of projectile
                    EnemyProjectileController controller = newProjectile.GetComponent<EnemyProjectileController>();
                    controller.SetDamage(damage, knockbackForce, stunTimer);
                    
                    // reset timer and cooldown for attack
                    _attackTimer = 0;
                    _attackCooldown = Random.Range(_attackCooldownMin, _attackCooldownMax);
                }                   
            }
        }
    }
    
    // path finding
    protected override void FindPath()
    {
        Vector2 start = _transform.position;
        Vector2 playerPos = _playerTransform.position;
        
        Node currentNode = _gridManager.NodeFromWorldPoint(start);
        List<Node> nextNode = new List<Node>();
        float distance = Vector2.Distance(currentNode.worldPosition, playerPos);
        
        List<Node> neighbors = _gridManager.GetNeighbours(currentNode);
        Node closestNode = null;
        float closestDistance = float.MaxValue;
        Node farthestNode = null;
        float farthestDistance = float.MinValue;
        Node strafeNode = null;
        float strafeDistance = _maxDistanceToPlayer;
        
        // check all neighbors for three pathing options
        foreach (Node neighbor in neighbors)
        {
            if (neighbor.walkable)
            {
                float neighborDistance = Vector2.Distance(neighbor.worldPosition, playerPos);
                if (neighborDistance < closestDistance)
                {
                    closestDistance = neighborDistance;
                    closestNode = neighbor;
                } 
                if (neighborDistance > farthestDistance)
                {
                    farthestDistance = neighborDistance;
                    farthestNode = neighbor;
                } 
                if (neighborDistance < strafeDistance && neighborDistance > _minDistanceToPlayer)
                {
                    strafeDistance = neighborDistance;
                    strafeNode = neighbor;
                }                
            }
        }
        
        // step 1: is current node within legal range -> strafe
        if (distance < _maxDistanceToPlayer && distance > _minDistanceToPlayer)
        {
            nextNode.Add(strafeNode);
            currentPath = nextNode;
        }
        // step 2: is current too close -> retreat
        else if (distance < _minDistanceToPlayer)
        {
            nextNode.Add(farthestNode);
            currentPath = nextNode;
        }
        // step 3: is current node too far -> advance
        else if (distance > _maxDistanceToPlayer)
        {
            nextNode.Add(closestNode);
            currentPath = nextNode;
        }

    }
    private int count = 0;
    // follow the path
    protected override IEnumerator Follow()
    {
        // error check for while we are in a pit
        if(currentPath == null || currentPath.Count == 0)
            yield break;
        Vector3 currentWaypoint = currentPath[0].worldPosition;
        targetIndex = 0;


        // iterate though the path as it updates
        while (true)
        {
            if ((Vector2)_transform.position == (Vector2)currentWaypoint)
            {
                targetIndex++;
                if (targetIndex >= currentPath.Count)
                {
                    yield break;
                }
                currentWaypoint = currentPath[targetIndex].worldPosition;
            }
            if (count == 222)
            {
                Managers.AudioManager.Instance.PlayRangedEnemyMovementSound(1, 0);
                count = 0;
            }
            ++count;

            _transform.position = Vector2.MoveTowards(_transform.position, currentWaypoint, speed * Time.deltaTime);
            yield return null;
        }
    }

    // function grabs and allocates stats for enemy
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
        findPathCooldown = 1f / speed;
    }
}