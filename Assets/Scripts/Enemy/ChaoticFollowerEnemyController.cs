using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaoticFollowerEnemyController : FollowerEnemyController
{
    [Header("Chaotic Pathfinding Settings")]
    public float chaosRadius = 4f; // How far from the player to wander
    public float chaosUpdateInterval = 1.5f; // How often to pick a new random target

    protected override void FindPath()
    {
        if (_playerTransform == null || _gridManager == null)
            return;

        // Pick random target around the player
        Vector2 playerPos = _playerTransform.position;
        Vector2 randomOffset = Random.insideUnitCircle * chaosRadius;
        Vector2 chaoticTargetPos = playerPos + randomOffset;

        Vector2 start = _transform.position;
        Node startNode = _gridManager.NodeFromWorldPoint(start, false);
        Node endNode = _gridManager.NodeFromWorldPoint(chaoticTargetPos, false);
        
        if (endNode == null || !endNode.walkable)
            endNode = _gridManager.GetNearestWalkableNode(chaoticTargetPos, false);

        if (endNode == null)
            return;

        // Normal A* logic from FollowerEnemyController
        List<Node> openSet = new List<Node> { startNode };
        HashSet<Node> closedSet = new HashSet<Node>();

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].FCost < currentNode.FCost ||
                    (openSet[i].FCost == currentNode.FCost && openSet[i].hCost < currentNode.hCost))
                    currentNode = openSet[i];
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == endNode)
            {
                RetracePath(startNode, endNode);
                return;
            }

            foreach (Node neighbor in _gridManager.GetNeighbours(currentNode, false))
            {
                if (!neighbor.walkable || closedSet.Contains(neighbor))
                    continue;

                int moveCost = currentNode.gCost + GetDistance(currentNode, neighbor);
                if (moveCost < neighbor.hCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = moveCost;
                    neighbor.hCost = GetDistance(neighbor, endNode);
                    neighbor.parent = currentNode;

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }
    }
    
    
    protected override void GetStats(string statLine)
    {
        // we will add slight variants to this
        string[] stats = statLine.Split(',');
        health = float.Parse(stats[0]);
        speed = float.Parse(stats[1]) + UnityEngine.Random.Range(-0.5f, 0.5f);
        damage = float.Parse(stats[2]);
        knockBackSpeed = float.Parse(stats[3]);
        knockBackTime = float.Parse(stats[4]);
        knockbackForce =  float.Parse(stats[5]);
        stunTimer = float.Parse(stats[6]);
        _playerHitTimer = float.Parse(stats[7]);
        findPathCooldown = chaosUpdateInterval;
    }

}
