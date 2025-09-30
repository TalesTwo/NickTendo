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
    private Vector2 _bottomLeft;
    private int gridSizeX, gridSizeY;
    
    [HideInInspector]
    public List<Node> path; // path from enemy to player
    
    private Transform _player;  // <-- Drag your player here in the Inspector
    
    // Start is called before the first frame update
    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        
        gridSizeX = Mathf.RoundToInt(gridRoomSize.x);
        gridSizeY = Mathf.RoundToInt(gridRoomSize.y);
        
        CreateGrid();
    }

    // create the grid starting from the bottom left of the room
    private void CreateGrid()
    {
        _grid = new Node[gridSizeX, gridSizeY];
        _bottomLeft = (Vector2)transform.position - Vector2.right * gridRoomSize.x / 2 - Vector2.up * gridRoomSize.y / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                // check for wall, if not, then set new cell to walkable
                Vector2 roomPoint = new Vector2(_bottomLeft.x + x + nodeRadius, _bottomLeft.y + y + nodeRadius);
                Vector2 boxSize = Vector2.one * (nodeRadius * 2 * 0.95999f);
                bool walkable = !(Physics2D.OverlapBox(roomPoint, boxSize, 0f, unwalkableLayer));
                _grid[x, y] = new Node(walkable, roomPoint, x, y);
            }
        }
    }
    
    // calculate the nearest node based on the current position
    public Node NodeFromWorldPoint(Vector2 worldPoint)
    {
        float percentX = Mathf.Clamp01((worldPoint.x - _bottomLeft.x) / gridRoomSize.x);
        float percentY = Mathf.Clamp01((worldPoint.y - _bottomLeft.y) / gridRoomSize.y);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        return _grid[x, y];
    }

    // find all neighboring cells
    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                // filter any spots that are not in the cardinal directions.
                if ((x == 0 && y == 0) || (Mathf.Abs(x) == 1 && Mathf.Abs(y) == 1))
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
    
    /*
    // useful for debugging and finding legal and illegal spots, as well as current path for entity.
    private void OnDrawGizmos()
    {
        // Draw the boundary of the grid
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(gridRoomSize.x, gridRoomSize.y, 1));

        if (_grid != null)
        {
            foreach (Node n in _grid)
            {
                // Default color based on walkability
                Gizmos.color = (n.walkable) ? Color.white : Color.red;

                // Highlight the node the player is standing on
                if (_player != null && Vector2.Distance(n.worldPosition, _player.position) < nodeRadius)
                {
                    Gizmos.color = Color.green;
                }

                // If this node is part of the current path, override color to black
                if (path != null && path.Contains(n))
                {
                    Gizmos.color = Color.black;
                }

                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeRadius * 2 - 0.1f));
            }
        }
    }
    */
}


