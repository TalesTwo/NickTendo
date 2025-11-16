using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class FollowerEnemyController : EnemyControllerBase
{
    private bool _playerHit;
    protected float _playerHitTimer;
    
    // Variables for the pits
    // these could be reduced if we make a co-routine work???? but they are so dumb
    // that i chose to make this mess instead
    private bool hasFallenInPit = false;
    private bool isShrinking = false;
    private float shrinkTimer = 0f;
    private const float shrinkDuration = 0.5f;
    private Vector3 startScale;
    private Vector3 startPos;
    private Vector3 pitTarget;
    private bool _hasFallenInPit = false;
    
    private float _storedFindPathCooldown = 0f;
    private bool _pathfindingPaused = false;
    
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
        Node startNode = _gridManager.NodeFromWorldPoint(start, false);
        Node endNode = _gridManager.NodeFromWorldPoint(end, false);
        
        // we will now do a check, if we cannot reach the player, we will instead just pick a random nearby node to walk to
        // (this is the case where the player is seperated via a pit or obstacle)
        
        
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

            foreach (Node neighbor in _gridManager.GetNeighbours(currentNode, false))
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
            _pathfindingPaused = false;
        }
        
        
        // NO PATH FOUND (probably due to a pit) move to random nearby walkable node)
        /*
        if (!_pathfindingPaused)
        {
            _storedFindPathCooldown = findPathCooldown;
            _pathfindingPaused = true;
        }

        findPathCooldown = 3f; // slow down pathfinding attempts while stuck
        
        List<Node> fallbackNodes = _gridManager.GetNeighbours(startNode, false);

        List<Node> validFallbacks = new List<Node>();
        foreach (Node n in fallbackNodes)
        {
            if (n.walkable)
                validFallbacks.Add(n);
        }

        if (validFallbacks.Count > 0)
        {
            Node fallback = validFallbacks[UnityEngine.Random.Range(0, validFallbacks.Count)];
            RetracePath(startNode, fallback);
        }
        return;
        */
    }

    // take the discovered most efficient path and reverse it so enemy can travel to the player
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
        findPathCooldown = 1f / (speed*2f);
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
    
    protected override void OnFellInPit(GameObject obj, Vector3 pitCenter)
    {
        if (obj != gameObject || hasFallenInPit)
            return;

        hasFallenInPit = true;
        isShrinking = true;
        shrinkTimer = 0f;
        startScale = transform.localScale;
        startPos = transform.position;
        pitTarget = pitCenter;
    }

    private void Update()
    {
        base.Update();
        if (!isShrinking)
            return;

        shrinkTimer += Time.deltaTime;
        float time = Mathf.Clamp01(shrinkTimer / shrinkDuration);

        // Move and shrink simultaneously
        transform.position = Vector3.Lerp(startPos, pitTarget, time);
        transform.localScale = Vector3.Lerp(startScale, Vector3.zero, time);

        if (time >= 1f)
        {
            isShrinking = false;
            transform.localScale = Vector3.zero;
            transform.position = pitTarget;
            Deactivate();
        }
    }
    
}