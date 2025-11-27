using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenUIActivator : Singleton<ScreenUIActivator>
{

    [SerializeField] private GameObject deathScreen;
    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject MiniMap;
    
    
    public void Start()
    {
        EventBroadcaster.DungeonGenerationComplete += OnDungeonGenerationComplete;
    }
    

    public void SetDeathScreen()
    {
        deathScreen.SetActive(true);
    }
    public void SetWinScreen()
    {
        winScreen.SetActive(true);
    }
    
    
    // MINIMAP FUNCTIONS
    
    private void OnDungeonGenerationComplete()
    {
        MiniMapUI miniMapUI = MiniMap.GetComponent<MiniMapUI>();
        if (miniMapUI != null)
        {
            miniMapUI.ConnectToBroadcaster();
        }
    }
    
    public void ToggleMiniMap()
    {
        MiniMap.SetActive(!MiniMap.activeSelf);
    }

}
