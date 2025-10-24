using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class FollowerEnemyController : EnemyControllerBase
{
    private bool _playerHit;
    private float _playerHitTimer;
    
    // invoke player damage and freeze to avoid chaining the player
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _playerHit = true;
            Invoke(nameof(PlayerHitCooldown), _playerHitTimer);
            DoDamage();
        }
    }

    // finding the shortest path to the player
    protected override void FindPath()
    {
        Vector2 start = _transform.position;
        Vector2 end = _playerTransform.position;
        
        // Step 1: set start and end nodes
        Node startNode = _gridManager.NodeFromWorldPoint(start);
        Node endNode = _gridManager.NodeFromWorldPoint(end);
        
        // Tick logs make me crash out LOL
        //Debug.Log(endNode.walkable);
        //Debug.Log(endNode.worldPosition);
        
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

            if (!_playerHit)
            {
                _transform.position = Vector2.MoveTowards(_transform.position, currentWaypoint, speed * Time.deltaTime);
            }
            yield return null;
        }
    }



    // do damage to and knockback the player
    private void DoDamage()
    {
        Vector2 direction = new Vector2(_player.transform.position.x - transform.position.x, _player.transform.position.y - transform.position.y).normalized;
        _playerController.KnockBack(knockbackForce, direction, stunTimer);
        _playerController.HitEffect(transform.position);
        Managers.AudioManager.Instance.PlayFollowerHitSound(1, 0);
        PlayerStats.Instance.UpdateCurrentHealth(-damage);
    }

    private void PlayerHitCooldown()
    {
        _playerHit = false;
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
        knockbackForce =  float.Parse(stats[5]);
        stunTimer = float.Parse(stats[6]);
        _playerHitTimer = float.Parse(stats[7]);
        findPathCooldown = 1f / (speed*2f);
    }
    
    protected override void Deactivate() 
    {
        base.Deactivate();
        EventBroadcaster.Broadcast_EnemyDeath(this, GetComponentInParent<Room>());
        // specific to ranged enemy deactivation logic can go here
        Managers.AudioManager.Instance.PlayEnemyDeathSound();
        Debug.Log("Follower Enemy destroyed");
        
    }
}