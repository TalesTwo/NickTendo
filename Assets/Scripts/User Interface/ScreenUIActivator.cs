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
    
    private bool _bHasFlickedMiniMap = false;
    
    public void Start()
    {
        EventBroadcaster.DungeonGenerationComplete += OnDungeonGenerationComplete;
        EventBroadcaster.ReturnToMainMenu += OnReturnToMainMenu;
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
    private void OnReturnToMainMenu()
    {
        // put back to default state
        MiniMap.SetActive(false);
        MiniMap_Corner.SetActive(true);
        _bHasFlickedMiniMap = false;
    }
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
        _bHasFlickedMiniMap = false;
    }
    
    public void ToggleMiniMap()
    {
        //We will check to see if we have flicked yet
        
        // lol flicker fix
        if (!_bHasFlickedMiniMap)
        {
            MiniMap.SetActive(!MiniMap.activeSelf);
            MiniMap_Corner.SetActive(!MiniMap_Corner.activeSelf);
            MiniMap.SetActive(!MiniMap.activeSelf);
            MiniMap_Corner.SetActive(!MiniMap_Corner.activeSelf);
            MiniMap.SetActive(!MiniMap.activeSelf);
            MiniMap_Corner.SetActive(!MiniMap_Corner.activeSelf);
            _bHasFlickedMiniMap = true;
        }
        else
        {
            MiniMap.SetActive(!MiniMap.activeSelf);
            MiniMap_Corner.SetActive(!MiniMap_Corner.activeSelf);
        }

    }

}
