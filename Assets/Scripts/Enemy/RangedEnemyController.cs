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
    protected float _projectileSpeed;
    
    private int _forcedWalkableFrequency = 5;
    private int _nodePickCounter = 0;

    // this is the real target when we force touching real ground
    private Node _groundTarget = null;
    private bool _useFlyingNodes = true;



    // this method handles ranged enemies combat attacks
    protected override void Attack()
    {
        _attackTimer += Time.deltaTime;
        // raycast for player, do not shoot unless you can see him (we also will ignore pits)
        int finalMask = doNotHit | LayerMask.GetMask("Pits") | LayerMask.GetMask("Spawning") | LayerMask.GetMask("Loot");
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
    
    // if we have no LOS, use A* from the follower enemy
    bool hasLOS = HasLineOfSight(_transform.position, _playerTransform.position);

    if (!hasLOS)
    {
        AStarToPlayer();
        return;
    }

    
    
    // ======= PHASE 1: If locked onto a ground tile, continue until reached ======= //
    if (_groundTarget != null)
    {
        Node currentFlyNode = _gridManager.NodeFromWorldPoint(_transform.position, true);

        // reached ground target
        if (currentFlyNode.worldPosition == _groundTarget.worldPosition)
        {
            _groundTarget = null;  // return to normal logic
        }
        else
        {
            // continue flying toward the ground tile
            Node next = _gridManager.GetNextNodeToward(currentFlyNode, _groundTarget, true);
            currentPath = new List<Node> { next };
            return;
        }
    }

    // ======= PHASE 2: Normal flying logic ======= //

    Vector2 start = _transform.position;
    Vector2 playerPos = _playerTransform.position;

    bool forceWalkablePick = false;
    if (_nodePickCounter >= _forcedWalkableFrequency)
    {
        forceWalkablePick = true;
        _nodePickCounter = 0;
    }

    bool useFlying = !forceWalkablePick;

    Node currentNode = _gridManager.NodeFromWorldPoint(start, useFlying);
    List<Node> nextNode = new List<Node>();
    float distance = Vector2.Distance(currentNode.worldPosition, playerPos);

    List<Node> neighbors = _gridManager.GetNeighbours(currentNode, useFlying);

    Node closestNode = null;
    float closestDistance = float.MaxValue;

    Node farthestNode = null;
    float farthestDistance = float.MinValue;

    Node strafeNode = null;
    float bestStrafeScore = float.MaxValue;

    Vector2 toPlayer = (playerPos - currentNode.worldPosition).normalized;

    foreach (Node neighbor in neighbors)
    {
        if (!neighbor.walkable) continue;

        float neighDist = Vector2.Distance(neighbor.worldPosition, playerPos);

        if (neighDist < closestDistance)
        {
            closestDistance = neighDist;
            closestNode = neighbor;
        }

        if (neighDist > farthestDistance)
        {
            farthestDistance = neighDist;
            farthestNode = neighbor;
        }

        if (neighDist < _maxDistanceToPlayer && neighDist > _minDistanceToPlayer)
        {
            Vector2 toNeighbor = (neighbor.worldPosition - currentNode.worldPosition).normalized;
            float dot = Mathf.Abs(Vector2.Dot(toNeighbor, toPlayer));

            if (dot < bestStrafeScore)
            {
                bestStrafeScore = dot;
                strafeNode = neighbor;
            }
        }
    }

    Node chosen = null;

    if (distance < _maxDistanceToPlayer && distance > _minDistanceToPlayer)
        chosen = strafeNode;
    else if (distance < _minDistanceToPlayer)
        chosen = farthestNode;
    else if (distance > _maxDistanceToPlayer)
        chosen = closestNode;

    if (chosen != null)
    {
        // If this was a forced-walkable pick:
        if (forceWalkablePick)
        {
            // Pick walkable tile on ground grid
            _groundTarget = _gridManager.GetClosestWalkableGroundNode(currentNode);

            if (_groundTarget != null)
            {
                currentPath = new List<Node> { _groundTarget };
                return;
            }
        }
        

        // normal behavior
        nextNode.Add(chosen);
        currentPath = nextNode;

        _nodePickCounter++;
    }
}
    
    private bool HasLineOfSight(Vector2 from, Vector2 to)
    {
        int mask = doNotHit 
                   | LayerMask.GetMask("Pits") 
                   | LayerMask.GetMask("Spawning") 
                   | LayerMask.GetMask("Loot")
                   | LayerMask.GetMask("Default");

        RaycastHit2D hit = Physics2D.Raycast(from, (to - from).normalized, Vector2.Distance(from, to), ~mask);
        // debug line
        Debug.DrawLine(from, to, Color.red, 0.1f);

        return hit.collider != null && hit.collider.CompareTag("Player");
    }


    // follow the path
    protected override IEnumerator Follow()
    {

        // error check for while we are in a pit
        if(currentPath == null || currentPath.Count == 0 || currentPath[0] == null)
            yield break;
        Vector3 currentWaypoint = currentPath[0].worldPosition;
        targetIndex = 0;


        // iterate though the path as it updates
        while (true)
        {
            if ((Vector2)_transform.position == (Vector2)currentWaypoint)
            {
                targetIndex++;
                if (currentPath == null)
                {
                    yield break;
                }
                if (targetIndex >= currentPath.Count)
                {
                    yield break;
                }
                currentWaypoint = currentPath[targetIndex].worldPosition;
            }

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
        findPathCooldown = 1f / (speed*2);
        base.GetStats(statLine);
    }
    
    protected override void Deactivate() 
    {
        
        EventBroadcaster.Broadcast_EnemyDeath(this, GetComponentInParent<Room>());
        // specific to ranged enemy deactivation logic can go here
        Managers.AudioManager.Instance.PlayEnemyDeathSound();
        
        base.Deactivate();
        Destroy(gameObject);
    }
    
    
    // Helper functions stolen from FollowerEnemyController
    private void AStarToPlayer()
    {
        Vector2 start = _transform.position;
        Vector2 end = _playerTransform.position;

        // Convert to node space (use flying grid so pits are allowed)
        Node startNode = _gridManager.NodeFromWorldPoint(start, true);
        Node endNode = _gridManager.NodeFromWorldPoint(end, true);

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];

            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].FCost < currentNode.FCost ||
                    (openSet[i].FCost == currentNode.FCost && openSet[i].hCost < currentNode.hCost))
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == endNode)
            {
                // Use YOUR version
                RetracePath(startNode, endNode);
                return;
            }

            foreach (Node neighbor in _gridManager.GetNeighbours(currentNode, true))
            {
                if (!neighbor.walkable || closedSet.Contains(neighbor))
                    continue;

                int movementCost = currentNode.gCost + GetDistance(currentNode, neighbor);

                if (movementCost < neighbor.hCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = movementCost;
                    neighbor.hCost = GetDistance(neighbor, endNode);
                    neighbor.parent = currentNode;

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        // No path found (rare)
        currentPath = null;
    }
    
    
    protected void RetracePath(Node start, Node end)
    {
        List<Node> path = new List<Node>();
        Node currentNode = end;

        while (currentNode != start)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        
        path.Reverse();
        
        _gridManager.path = path;
        currentPath = path;
    }
    
    protected int GetDistance(Node a, Node b)
    {
        int x = Mathf.Abs(a.gridX - b.gridX);
        int y = Mathf.Abs(a.gridY - b.gridY);

        if (x > y)
        {
            return 14 * y + 10 * (x - y);
        }
        
        return 14 * x + 10 * (y - x);
    }
}