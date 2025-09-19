using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    // DoorOne = North, DoorTwo = East, DoorThree = South, DoorFour = West
    
    
    // A list of doors that can be used to enter/exit the room
    [Header("Room Configuration")]
    [SerializeField] public Types.DoorConfiguration configuration;
    [SerializeField] private GameObject doors;
    
    [Space(10f)]
    [Header("Room Type")]
    [SerializeField] private Types.RoomType roomType;
    


    public void Awake()
    {
        InitializeRoom();
    }
    
    public void InitializeRoom()
    {
        // Initialize room logic here
        ApplyDoorConfiguration();
    }

    
    public void SetRoomEnabled(bool bEnabled)
    {
        Action action = bEnabled ? EnableRoom : DisableRoom;
        action();
    }

    private void EnableRoom()
    {
        // disable the room here
        gameObject.SetActive(true);
        
        // this is a separate function, incase we need to do more complex logic in the future
    }
    private void DisableRoom()
    {
        // disable the room here
        gameObject.SetActive(false);
        
        // this is a separate function, incase we need to do more complex logic in the future
    }
    
    private void ApplyDoorConfiguration()
    {
        // Apply the door configuration to the doors in the room
        
        // get a list of all of the children of the doors object
        List<DoorTriggerInteraction> doorObjects = new List<DoorTriggerInteraction>(doors.GetComponentsInChildren<DoorTriggerInteraction>());
        
        for (int i = 0; i < doorObjects.Count; i++)
        {
            switch (i)
            {
                case 0:
                    doorObjects[i].gameObject.SetActive(configuration.doorOneActive);
                    break;
                case 1:
                    doorObjects[i].gameObject.SetActive(configuration.doorTwoActive);
                    break;
                case 2:
                    doorObjects[i].gameObject.SetActive(configuration.doorThreeActive);
                    break;
                case 3:
                    doorObjects[i].gameObject.SetActive(configuration.doorFourActive);
                    break;
            }
        }
    }
}
