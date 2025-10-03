using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{

    public bool walkable;
    public Vector2 worldPosition;
    public int gridX;
    public int gridY;
    
    public int gCost; // Cost from start node
    public int hCost; // Heuristic cost to target node
    public int FCost { get { return gCost + hCost; } }

    public Node parent;

    public Node(bool walkable, Vector2 worldPos, int gridX, int gridY)
    {
        this.walkable = walkable;
        this.worldPosition = worldPos;
        this.gridX = gridX;
        this.gridY = gridY;
    }

}
