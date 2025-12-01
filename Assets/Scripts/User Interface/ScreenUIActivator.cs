using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenUIActivator : Singleton<ScreenUIActivator>
{

    [SerializeField] private GameObject deathScreen;
    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject MiniMap;
    [SerializeField] private GameObject MiniMap_Corner;
    
    
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
        
        MiniMapUI miniMapUI_Corner = MiniMap_Corner.GetComponent<MiniMapUI>();
        if (miniMapUI_Corner != null)
        {
            miniMapUI_Corner.ConnectToBroadcaster();
        }
    }
    
    public void ToggleMiniMap()
    {
        MiniMap.SetActive(!MiniMap.activeSelf);
        MiniMap_Corner.SetActive(!MiniMap_Corner.activeSelf);
    }

}
