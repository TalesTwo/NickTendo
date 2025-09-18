using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    
    
    public void Awake()
    {
        InitializeRoom();
    }
    
    public void InitializeRoom()
    {
        // Initialize room logic here
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
}
