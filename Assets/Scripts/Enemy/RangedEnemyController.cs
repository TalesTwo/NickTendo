using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemyController : EnemyControllerBase
{
    [Header("Safe Distance from Player")]
    private float _minDistanceToPlayer;
    private float _maxDistanceToPlayer;

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

    // follow the path
    protected override IEnumerator Follow()
    {
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
        findPathCooldown = 1f / speed;
    }
}