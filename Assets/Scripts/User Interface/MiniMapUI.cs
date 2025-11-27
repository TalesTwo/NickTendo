using System;
using System.Collections.Generic;
using Managers;
using UnityEngine;
using UnityEngine.UI;

public class MiniMapUI : MonoBehaviour
{
    [Header("MiniMap Setup")]
    public Transform contentParent;      
    public GameObject cellPrefab;

    private List<List<GameObject>> grid;
    private bool[,] discovered;          // tracks visited rooms
    private bool _isInitialized = false;

    private void Start()
    {
        EventBroadcaster.PlayerChangedRoom += OnPlayerChangedRoom;
        EventBroadcaster.GameStarted += GameStartedHandler;
        EventBroadcaster.GameRestart += GameStartedHandler;
        EventBroadcaster.PlayerDeath += OnPlayerDeath;
    }

    private void OnEnable()
    {
        if (!_isInitialized)
        {
            int cols = DungeonGeneratorManager.Instance.GetCols();
            int rows = DungeonGeneratorManager.Instance.GetRows();

            discovered = new bool[rows, cols];
            InitializeMiniMap(rows, cols);
        }
    }
    

    private void OnPlayerDeath()
    {
        // clear our the minimap, along with all discovered rooms
        int cols = DungeonGeneratorManager.Instance.GetCols();
        int rows = DungeonGeneratorManager.Instance.GetRows();
        discovered = new bool[rows, cols];
        // Hide all cells
        foreach (var row in grid)
        {
            foreach (var cell in row)
            {
                cell.SetActive(false);
            }
        }
    }
    
    private void GameStartedHandler()
    {
        _isInitialized = false;
    }

    // ----------------------------------------------------------------------
    // Called whenever the player enters a new room
    // ----------------------------------------------------------------------
    private void OnPlayerChangedRoom((int newRow, int newCol) obj)
    {
        int row = obj.newRow;
        int col = obj.newCol;

        // mark as discovered forever
        discovered[row, col] = true;

        // update all cell visuals
        RefreshMiniMap(row, col);
    }

    // ----------------------------------------------------------------------
    // Update visibility & colors of all cells
    // ----------------------------------------------------------------------
    private void RefreshMiniMap(int playerRow, int playerCol)
    {
        var dungeon = DungeonGeneratorManager.Instance.GetDungeonRooms();

        for (int r = 0; r < grid.Count; r++)
        {
            for (int c = 0; c < grid[r].Count; c++)
            {
                GameObject cell = grid[r][c];

                // Hide undiscovered rooms
                cell.SetActive(discovered[r, c]);

                if (!discovered[r, c])
                    continue;

                Image img = cell.GetComponent<Image>();
                Room room = dungeon[r][c];
                Types.RoomClassification rc = room.GetRoomClassification();
                // Get the Active doors for the room
                room.GetRoomClassification();
                // now enable the (North, East, South, West) doors based on the room's active doors
                var roomConfig = room.configuration;
                if (roomConfig.NorthDoorActive){
                    cell.transform.Find("North").gameObject.SetActive(true);
                } else {
                    cell.transform.Find("North").gameObject.SetActive(false);
                }
                if (roomConfig.EastDoorActive){
                    cell.transform.Find("East").gameObject.SetActive(true);
                } else {
                    cell.transform.Find("East").gameObject.SetActive(false);
                }
                if (roomConfig.SouthDoorActive){
                    cell.transform.Find("South").gameObject.SetActive(true);
                } else {
                    cell.transform.Find("South").gameObject.SetActive(false);
                }
                if (roomConfig.WestDoorActive){
                    cell.transform.Find("West").gameObject.SetActive(true);
                } else {
                    cell.transform.Find("West").gameObject.SetActive(false);
                }
                

                // Player location always overrides
                if (r == playerRow && c == playerCol)
                {
                    img.color = Color.green;
                    continue;
                }

                // coloring based on classification
                switch (rc)
                {
                    case Types.RoomClassification.Spawn:
                        img.color = Color.blue;
                        break;
                    case Types.RoomClassification.Boss:
                        img.color = Color.red;
                        break;
                    case Types.RoomClassification.Shop:
                        img.color = Color.yellow;
                        break;
                    default:
                        img.color = Color.white;
                        break;
                }
            }
        }
    }

    // ----------------------------------------------------------------------
    // Build the minimap grid
    // ----------------------------------------------------------------------
    public void InitializeMiniMap(int rows, int cols)
    {
        if (grid != null)
        {
            foreach (var row in grid)
                foreach (var cell in row)
                    Destroy(cell);
        }

        grid = new List<List<GameObject>>();

        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        for (int r = 0; r < rows; r++)
        {
            GameObject rowObj = new GameObject($"Row_{r}", typeof(RectTransform));
            rowObj.transform.SetParent(contentParent, false);

            RectTransform rowRT = rowObj.GetComponent<RectTransform>();
            rowRT.anchorMin = new Vector2(0, 1);
            rowRT.anchorMax = new Vector2(0, 1);
            rowRT.pivot = new Vector2(0, 1);
            rowRT.anchoredPosition = new Vector2(0, -r * 50);

            List<GameObject> rowList = new List<GameObject>();

            for (int c = 0; c < cols; c++)
            {
                GameObject cell = Instantiate(cellPrefab, rowRT);
                RectTransform cellRT = cell.GetComponent<RectTransform>();

                // kept your existing sizing
                cellRT.sizeDelta = new Vector2(50, 50);

                // kept your existing spacing
                cellRT.anchoredPosition = new Vector2(c * 100, 0);

                // start hidden until discovered
                cell.SetActive(false);

                rowList.Add(cell);
            }

            grid.Add(rowList);
        }
        
        // auto detect start position (spawn)
        //Vector2 start = DungeonGeneratorManager.Instance.GetStartPos();
        //int startRow = Mathf.RoundToInt(start.y);
        //int startCol = Mathf.RoundToInt(start.x);
        
        //discovered[startRow, startCol] = true;

        //RefreshMiniMap(startRow, startCol);

        _isInitialized = true;
    }
}
