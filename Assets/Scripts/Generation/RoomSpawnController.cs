using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSpawnController : MonoBehaviour
{
    // reference to the room that we are attached to
    [SerializeField]
    private Room room;
    // Start is called before the first frame update
    private void Awake()
    {
        room = GetComponentInParent<Room>();
    }
    
    
    
    
}
