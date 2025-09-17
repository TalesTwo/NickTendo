using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTriggerInteraction : TriggerInteractBase
{

    [SerializeField] private Types.DoorConfiguration currentConfiguration;
    public enum DoorToSpawnAt
    {
        None,
        One,
        Two,
        Three,
        Four
    }
    
    
    [Header("Spawn To")]
    [SerializeField] private SceneField _sceneToLoad;
    [SerializeField] private DoorToSpawnAt DoorToSpawnTo= DoorToSpawnAt.None;
    
    [Space(10f)]
    [Header("Door Settings")]
    [SerializeField] public DoorToSpawnAt CurrentDoorPosition = DoorToSpawnAt.None;

    public override void Interact()
    {
        SceneSwapManager.SwapSceneFromDoorUse(_sceneToLoad, DoorToSpawnTo);
    }
}
