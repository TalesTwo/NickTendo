using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaoticFollowerEnemyController : FollowerEnemyController
{
    [Header("Chaotic Pathfinding Settings")]
    public float pathRecalcInterval = 2.5f;   // How often to pick a new path
    public float chaosRadius = 4f;            // How far from the player to wander

    private Node _chaoticTargetNode;
    private float _timer;

    protected override void Update()
    {
        base.Update();

        // Accumulate time and recalculate on interval
        _timer += Time.deltaTime;
        if (_timer >= pathRecalcInterval)
        {
            _timer = 0f;
            FindPath();
        }
    }

    protected override void FindPath()
    {
        if (_playerTransform == null || _gridManager == null)
            return;

        // pick a random offset around the player
        Vector2 playerPos = _playerTransform.position;
        Vector2 randomOffset = Random.insideUnitCircle * chaosRadius;
        Vector2 chaoticTargetPos = playerPos + randomOffset;

        // regular A* path logic from FollowerEnemyController
        Vector2 start = _transform.position;
        Node startNode = _gridManager.NodeFromWorldPoint(start, false);
        Node endNode = _gridManager.NodeFromWorldPoint(chaoticTargetPos, false);

        if (endNode == null || !endNode.walkable)
            return;

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
                RetracePath(startNode, endNode);
                _chaoticTargetNode = endNode;
                return;
            }

            foreach (Node neighbor in _gridManager.GetNeighbours(currentNode, false))
            {
                if (!neighbor.walkable || closedSet.Contains(neighbor))
                {
                    continue;
                }

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
    }
    
}
