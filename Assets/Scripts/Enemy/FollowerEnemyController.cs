using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class FollowerEnemyController : EnemyControllerBase
{
    // ignore layermask
    [Header("RayCasting")]
    public LayerMask ignoreLayer;

    private RoomGridManager _gridManager;
    private int targetIndex;
    private List<Node> currentPath;

    private void Awake()
    {
        _gridManager = transform.parent.GetComponent<RoomGridManager>();
    }

    // find the intended direction of movement
    protected override Vector3 GetDirection()
    {
        /*
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
        */
        
        // Step 1.2: No player found, travel shortest route
        if (_gridManager.path != null && _gridManager.path.Count > 0)
        {
            StopAllCoroutines();
            StartCoroutine(Follow());
        }        
        
        return Vector3.zero;
    }

    // finding the shortest path to the player
    protected override void FindPath()
    {
        Vector2 start = _transform.position;
        Vector2 end = _playerTransform.position;
        
        // Step 1: set start and end nodes
        Node startNode = _gridManager.NodeFromWorldPoint(start);
        Node endNode = _gridManager.NodeFromWorldPoint(end);
        
        Debug.Log(endNode.walkable);
        Debug.Log(endNode.worldPosition);
        
        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        
        openSet.Add(startNode);

        // Step 2: iterate using A*
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
                return;
            }

            foreach (Node neighbor in _gridManager.GetNeighbours(currentNode))
            {
                if (!neighbor.walkable || closedSet.Contains(neighbor))
                {
                    continue;
                }

                int MovementCost = currentNode.gCost + GetDistance(currentNode, neighbor);

                if (MovementCost < neighbor.hCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = MovementCost;
                    neighbor.hCost = GetDistance(neighbor, endNode);
                    neighbor.parent = currentNode;

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }
    }

    // take the discovered most efficient path and reverse it so enemy can travel to the player
    private void RetracePath(Node start, Node end)
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
    
    private int GetDistance(Node a, Node b)
    {
        int x = Mathf.Abs(a.gridX - b.gridX);
        int y = Mathf.Abs(a.gridY - b.gridY);

        if (x > y)
        {
            return 14 * y + 10 * (x - y);
        }
        
        return 14 * x + 10 * (y - x);
    }
    
    // follow the path as set out by A*
    IEnumerator Follow()
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
    
    // grabs stats from .csv doc
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