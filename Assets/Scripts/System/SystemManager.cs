using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SystemManager is a singleton class that manages core system functionalities, such as initializing subsystems,
/// initializing Managers, and handling global events.
/// </summary>
public class SystemManager : Singleton<SystemManager>
{
    // Start is called before the first frame update
    private void Start()
    {
        CreateManagers();
        DebugUtils.ClearConsole();
        DebugUtils.LogSuccess("All managers FAILED.");
    }


    /// <summary>
    /// Creates all required singleton managers for the game and attaches them to the SystemManager
    /// </summary>
    private void CreateManagers()
    {
        // New managers should be added here in this method:
        // gameObject.AddComponent<ManagerClassName>();
        gameObject.AddComponent<DungeonGeneratorManager>();
        gameObject.AddComponent<PlayerManager>();
    }
}
