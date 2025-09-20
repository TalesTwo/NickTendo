using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;

public class DoorTriggerInteraction : TriggerInteractBase
{

    
    [Header("Spawn To")]
    [SerializeField] private SceneField _sceneToLoad;
    [SerializeField] private Types.DoorClassification DoorToSpawnTo= Types.DoorClassification.None;
    
    [Space(10f)]
    [Header("Door Settings")]
    [SerializeField] public Types.DoorClassification CurrentDoorPosition = Types.DoorClassification.None;

    public override void Interact()
    {
        SceneSwapManager.SwapSceneFromDoorUse(_sceneToLoad, DoorToSpawnTo);
    }
}
