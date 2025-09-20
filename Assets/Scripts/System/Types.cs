using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Types
{
    /* ----------------- GENERATION TYPES ----------------- */

    public enum RoomType
    {
        Spawn,
        N,
        E,
        S,
        W,
        
    }
    
    [Serializable]
    public struct DoorConfiguration
    {
        public bool NorthDoorActive;
        public bool EastDoorActive;
        public bool SouthDoorActive;
        public bool WestDoorActive;
    }
    
    public enum DoorClassification
    {
        None,
        North,
        East,
        South,
        West
    }
    
    /* ----------------- END GENERATION TYPES ----------------- */
    

}
