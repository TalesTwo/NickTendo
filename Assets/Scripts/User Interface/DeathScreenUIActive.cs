using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathScreenUIActive : Singleton<DeathScreenUIActive>
{

    public GameObject deathScreen;

    public void SetDeathScreen()
    {
        deathScreen.SetActive(true);
    }

}
