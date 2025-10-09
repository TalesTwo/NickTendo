using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomGridManager : MonoBehaviour
{
    [Header("Room Grid Details")]
    public LayerMask unwalkableLayer;
    private Vector2 gridRoomSize;
    public float nodeRadius = 0.5f;

    private Node[,] _grid;
    private Vector2 _bottomLeft;
    private int gridSizeX, gridSizeY;
    
    [HideInInspector]
    public List<Node> path; // path from enemy to player
    
    private Transform _player;  // <-- Drag your player here in the Inspector
    // Minimum distance from any door in world units
    const float minDistanceFromDoor = 4f;
    
    // Start is called before the first frame update
    private void Awake()
    {
        
        // Get access to the tilemap of the walls in the room, and get its x and y size
        Tilemap tilemap = GetComponentInChildren<Tilemap>();
        if (tilemap != null)
        {
            // Force refresh bounds
            tilemap.CompressBounds();
            BoundsInt bounds = tilemap.cellBounds;
            gridRoomSize = new Vector2(bounds.size.x, bounds.size.y);
        }
        
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


    public Transform FindValidWalkableCell()
    {


        if (_grid == null) return null;

        // Get all door positions from the "Doors" object
        Transform doorsParent = transform.Find("Doors");
        List<Transform> doorPoints = new List<Transform>();
        foreach (Transform child in doorsParent)
        {
            doorPoints.Add(child);
        }
        
        // Collect all valid nodes
        List<Node> validNodes = new List<Node>();

        foreach (Node node in _grid)
        {
            if (!node.walkable) continue;

            bool hasLineOfSight = false;
            bool tooCloseToDoor = false;

            foreach (Transform door in doorPoints)
            {
                Vector2 start = door.position;
                Vector2 end = node.worldPosition;
                Vector2 direction = (end - start).normalized;
                float distance = Vector2.Distance(start, end);

                if (distance < minDistanceFromDoor)
                {
                    tooCloseToDoor = true;
                    break;
                }

                RaycastHit2D hit = Physics2D.Raycast(start, direction, distance, unwalkableLayer);
                if (hit.collider == null)
                {
                    hasLineOfSight = true;
                }
            }

            // now check both conditions normally
            if (tooCloseToDoor)
                continue;

            if (hasLineOfSight)
                validNodes.Add(node);
        }
        
        if (validNodes.Count == 0)
        {
            Debug.LogWarning($"No valid walkable nodes found for {name} — returning null.");
            return null;
        }

        
        // Randomly select one valid node
        Node chosenNode = validNodes[UnityEngine.Random.Range(0, validNodes.Count)];

        // Create a temporary transform to mark spawn
        GameObject temp = new GameObject("TempSpawnPoint");
        temp.transform.position = chosenNode.worldPosition;
        temp.transform.SetParent(transform);

        // Auto-cleanup
        Destroy(temp, 0.5f);

        return temp.transform;
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


