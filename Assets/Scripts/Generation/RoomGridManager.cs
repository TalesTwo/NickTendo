using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomGridManager : MonoBehaviour
{
    [Header("Room Grid Details")]
    public LayerMask unwalkableLayer;
    public LayerMask pitLayer;
    public LayerMask spawnLayer;
    private Vector2 gridRoomSize;
    public float nodeRadius = 0.5f;

    // for walking paths
    private Node[,] _walkGrid;
    // for flying paths
    private Node[,] _flyGrid;
    // Spawn locations
    private List<Node> _spawnableNodes = new List<Node>();

    // pit locations
    private List<Vector2> _pitLocations = new List<Vector2>();
    private Vector2 _bottomLeft;
    private int gridSizeX, gridSizeY;
    
    [HideInInspector]
    public List<Node> path; // path from enemy to player
    
    private Transform _player;  // <-- Drag your player here in the Inspector
    // Minimum distance from any door in world units
    const float minDistanceFromDoor = 4f;
    // the extra grid resolution multiplier (2 = 4 subcells per cell, 3 = 9 subcells per cell..)
    const int resolutionMultiplier = 2;
    
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

            // Convert to world space for correct coverage
            Vector3Int minCell = bounds.min;
            Vector3Int maxCell = bounds.max;

            Vector3 worldMin = tilemap.CellToWorld(minCell);
            Vector3 worldMax = tilemap.CellToWorld(maxCell);

            gridRoomSize = new Vector2(worldMax.x - worldMin.x, worldMax.y - worldMin.y);
            _bottomLeft = worldMin;
        }

        if (GameObject.FindGameObjectWithTag("Player"))
        {
            _player = GameObject.FindGameObjectWithTag("Player").transform;
        }
        else
        {
            _player = null;
        }
        
        gridSizeX = Mathf.RoundToInt(gridRoomSize.x);
        gridSizeY = Mathf.RoundToInt(gridRoomSize.y);
        
        CreateGrids();
    }
    private void Start()
    {
        // Subscribe to enemy death event to update grid
        EventBroadcaster.EnemyDeath += OnEnemyDeath;
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.G))
        {
            RegenerateGrids();
        }
    }


    // we will call this anytime an enemy dies, or a chest/pot is placed to update the grid
    private void OnEnemyDeath(EnemyControllerBase enemy, Room room = null)
    {
        // check to see if its type is chest or pot
        if (enemy.enemyType == Types.EnemyType.ChestEnemy || enemy.enemyType == Types.EnemyType.PotEnemy)
        {
            RegenerateGrids();
        }
    }
    public void RegenerateGrids()
    {
        CreateGrids();
        RemoveOverlappingWithChestsAndPots();
    }

    private void RemoveOverlappingWithChestsAndPots()
    {
        // Find all relevant enemies
        EnemyControllerBase[] allEnemies = FindObjectsByType<EnemyControllerBase>(FindObjectsSortMode.None);

        List<EnemyControllerBase> blockingEnemies = new List<EnemyControllerBase>();

        foreach (EnemyControllerBase e in allEnemies)
        {
            if (e.enemyType == Types.EnemyType.ChestEnemy || e.enemyType == Types.EnemyType.PotEnemy)
                blockingEnemies.Add(e);
        }

        foreach (EnemyControllerBase chets_pot in blockingEnemies)
        {
            Collider2D chets_pot_collider = chets_pot.GetComponent<Collider2D>();
            if (chets_pot_collider == null) continue;

            // Get the world bounds of the object
            Bounds bounds = chets_pot_collider.bounds;

            // Iterate through grid and disable overlapping nodes
            for (int x = 0; x < _walkGrid.GetLength(0); x++)
            {
                for (int y = 0; y < _walkGrid.GetLength(1); y++)
                {
                    Node node = _walkGrid[x, y];
                    if (!node.walkable) continue;

                    // check overlap
                    if (bounds.Contains(node.worldPosition))
                    {
                        node.walkable = false;
                    }
                }
            }
        }
    }

    // create the grid starting from the bottom left of the room
    private void CreateGrids()
    {
        _walkGrid = new Node[gridSizeX * resolutionMultiplier, gridSizeY * resolutionMultiplier];
        _flyGrid  = new Node[gridSizeX * resolutionMultiplier, gridSizeY * resolutionMultiplier];
        _spawnableNodes.Clear();
        float scaledRadius = nodeRadius / resolutionMultiplier;

        for (int x = 0; x < gridSizeX * resolutionMultiplier; x++)
        {
            for (int y = 0; y < gridSizeY * resolutionMultiplier; y++)
            {
                Vector2 worldPoint = new Vector2(
                    _bottomLeft.x + (x + 0.5f) * (scaledRadius * 2),
                    _bottomLeft.y + (y + 0.5f) * (scaledRadius * 2)
                );

                Vector2 boxSize = Vector2.one * (scaledRadius * 2 * 0.96f);

                bool hasWall = Physics2D.OverlapBox(worldPoint, boxSize, 0f, unwalkableLayer);
                bool isPit = Physics2D.OverlapBox(worldPoint, boxSize, 0f, pitLayer);
                bool isSpawnable = Physics2D.OverlapBox(worldPoint, boxSize, 0f, spawnLayer);

                // Ground grid (can't walk on pits)
                bool groundWalkable = (!hasWall && !isPit) || isSpawnable;
                // Flying grid (ignores pits)
                bool airWalkable = !hasWall;

                _walkGrid[x, y] = new Node(groundWalkable, worldPoint, x, y);
                _flyGrid[x, y] = new Node(airWalkable, worldPoint, x, y);
                // Record pit locations
                if (isPit)
                {
                    _pitLocations.Add(worldPoint);
                }
                if (isSpawnable)
                {
                    _spawnableNodes.Add(_walkGrid[x, y]);
                }
            }
        }
    }

    
    // calculate the nearest node based on the current position
    public Node NodeFromWorldPoint(Vector2 worldPoint, bool isFlying)
    {
        float percentX = Mathf.Clamp01((worldPoint.x - _bottomLeft.x) / gridRoomSize.x);
        float percentY = Mathf.Clamp01((worldPoint.y - _bottomLeft.y) / gridRoomSize.y);

        int x = Mathf.RoundToInt((gridSizeX * resolutionMultiplier - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY * resolutionMultiplier - 1) * percentY);

        return isFlying ? _flyGrid[x, y] : _walkGrid[x, y];
    }

    // find all neighboring cells
    public List<Node> GetNeighbours(Node node, bool isFlying)
    {
        Node[,] grid = isFlying ? _flyGrid : _walkGrid;
        List<Node> neighbours = new List<Node>();

        int maxX = gridSizeX * resolutionMultiplier;
        int maxY = gridSizeY * resolutionMultiplier;

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if ((x == 0 && y == 0) || (Mathf.Abs(x) == 1 && Mathf.Abs(y) == 1))
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < maxX && checkY >= 0 && checkY < maxY)
                    neighbours.Add(grid[checkX, checkY]);
            }
        }

        return neighbours;
    }
    
    
    public Transform FindValidSpawnableCell()
    {
        if (_spawnableNodes == null || _spawnableNodes.Count == 0)
        {
            Debug.LogWarning($"No cached spawnable nodes found for {name}");
            return null;
        }

        Node chosenNode = _spawnableNodes[UnityEngine.Random.Range(0, _spawnableNodes.Count)];

        GameObject temp = new GameObject("TempSpawnPoint");
        temp.transform.position = chosenNode.worldPosition;
        temp.transform.SetParent(transform);
        Destroy(temp, 0.5f);

        return temp.transform;
    }

    public Transform FindValidWalkableCell(bool isFlying = false)
    {
        // Pick the correct grid
        Node[,] grid = isFlying ? _flyGrid : _walkGrid;
        if (grid == null)
        {
            Debug.LogWarning($"Grid not initialized in {name}");
            return null;
        }

        // Get all door positions from the "Doors" object
        Transform doorsParent = transform.Find("Doors");
        List<Transform> doorPoints = new List<Transform>();
        if (doorsParent != null)
        {
            foreach (Transform child in doorsParent)
                doorPoints.Add(child);
        }

        // Collect all valid nodes
        List<Node> validNodes = new List<Node>();

        foreach (Node node in grid)
        {
            if (!node.walkable) continue;

            bool tooCloseToDoor = false;

            foreach (Transform door in doorPoints)
            {
                float distance = Vector2.Distance(door.position, node.worldPosition);
                if (distance < minDistanceFromDoor)
                {
                    tooCloseToDoor = true;
                    break;
                }
            }

            if (tooCloseToDoor)
                continue;

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
        Destroy(temp, 0.5f);

        return temp.transform;
    }
    
    // Find the nearest walkable node to a world position
    public Node GetNearestWalkableNode(Vector2 worldPos, bool isFlying = false, int maxSearchDistance = 5)
    {
        Node startNode = NodeFromWorldPoint(worldPos, isFlying);
        if (startNode.walkable)
            return startNode;

        Node[,] grid = isFlying ? _flyGrid : _walkGrid;
        int maxX = grid.GetLength(0);
        int maxY = grid.GetLength(1);

        for (int radius = 1; radius <= maxSearchDistance; radius++)
        {
            for (int dx = -radius; dx <= radius; dx++)
            {
                for (int dy = -radius; dy <= radius; dy++)
                {
                    int checkX = startNode.gridX + dx;
                    int checkY = startNode.gridY + dy;

                    if (checkX < 0 || checkX >= maxX || checkY < 0 || checkY >= maxY)
                        continue;

                    Node node = grid[checkX, checkY];
                    if (node.walkable)
                        return node;
                }
            }
        }

        return startNode; // fallback to original even if blocked
    }

    public Vector3 GetNearestPitToLocation(Vector3 position)
    {
        // Given a particular position, find the nearest pit location from the _pitLocations list
        Vector3 nearestPit = Vector3.zero;
        float nearestDistance = float.MaxValue;

        foreach (Vector2 pitPos in _pitLocations)
        {
            // Compare in 2D but output a 3D position (assuming pits are stored as Vector2)
            float distance = Vector2.Distance(new Vector2(position.x, position.y), pitPos);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestPit = new Vector3(pitPos.x, pitPos.y, position.z);
            }
        }

        return nearestPit;
    }
    
    // useful for debugging and finding legal and illegal spots, as well as current path for entity.
    
    private void OnDrawGizmos()
    {
        // Draw the boundary of the grid
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(_bottomLeft + (Vector2)gridRoomSize / 2, new Vector3(gridRoomSize.x, gridRoomSize.y, 1));

        if (_walkGrid != null)
        {
            // Match whatever multiplier you used when generating the grid
            float scaledRadius = nodeRadius / resolutionMultiplier;

            foreach (Node n in _walkGrid)
            {
                // Default color based on walkability
                Gizmos.color = n.walkable ? Color.white : Color.red;

                // Highlight the node the player is standing on
                if (_player != null && Vector2.Distance(n.worldPosition, _player.position) < scaledRadius)
                    Gizmos.color = Color.green;

                // If this node is part of the current path, override color to black
                if (path != null && path.Contains(n))
                    Gizmos.color = Color.black;

                // Use the scaled radius for correct cube size
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (scaledRadius * 2 - 0.05f));
            }
        }
    }
    
    
}



    
    
    

    