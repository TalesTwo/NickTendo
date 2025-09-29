using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGridManager : MonoBehaviour
{
    [Header("Room Grid Details")]
    public LayerMask unwalkableLayer;
    public Vector2 gridRoomSize;
    public float nodeRadius = 0.5f;

    private Node[,] _grid;
    private int gridSizeX, gridSizeY;
    
    [HideInInspector]
    public List<Node> path;
    
    // Start is called before the first frame update
    private void Awake()
    {
        gridSizeX = Mathf.RoundToInt(gridRoomSize.x);
        gridSizeY = Mathf.RoundToInt(gridRoomSize.y);
        
        CreateGrid();
    }

    private void CreateGrid()
    {
        _grid = new Node[gridSizeX, gridSizeY];
        Vector2 bottomLeft = (Vector2)transform.position - Vector2.right * gridRoomSize.x / 2 - Vector2.up * gridRoomSize.y / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector2 roomPoint = new Vector2(bottomLeft.x + x + nodeRadius, bottomLeft.y + y + nodeRadius);
                bool walkable = !Physics2D.OverlapCircle(roomPoint, nodeRadius, unwalkableLayer);
                _grid[x, y] = new Node(walkable, roomPoint, x, y);
            }
        }
    }
    
    public Node NodeFromWorldPoint(Vector2 worldPoint)
    {
        float percentX = Mathf.Clamp01((worldPoint.x - transform.position.x + gridRoomSize.x / 2) / gridRoomSize.x);
        float percentY = Mathf.Clamp01((worldPoint.y - transform.position.y + gridRoomSize.y / 2) / gridRoomSize.y);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        return _grid[x, y];
    }


    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                {
                    continue;
                }
                
                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(_grid[checkX, checkY]);
                }
            }
        }
        
        return neighbours;
    }
}

