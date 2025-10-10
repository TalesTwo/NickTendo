using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenUIActivator : Singleton<ScreenUIActivator>
{

    [SerializeField] private GameObject deathScreen;
    [SerializeField] private GameObject winScreen;
    

    public void SetDeathScreen()
    {
        deathScreen.SetActive(true);
    }
    public void SetWinScreen()
    {
        winScreen.SetActive(true);
    }

}
