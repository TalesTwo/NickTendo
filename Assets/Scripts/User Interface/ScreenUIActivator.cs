using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenUIActivator : Singleton<ScreenUIActivator>
{

    [SerializeField] private GameObject deathScreen;
    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject MiniMap;
    

    public void SetDeathScreen()
    {
        deathScreen.SetActive(true);
    }
    public void SetWinScreen()
    {
        winScreen.SetActive(true);
    }
    public void SetMiniMapActive(bool isActive)
    {
        MiniMap.SetActive(isActive);
    }
    
    public void ToggleMiniMap()
    {
        MiniMap.SetActive(!MiniMap.activeSelf);
    }

}
