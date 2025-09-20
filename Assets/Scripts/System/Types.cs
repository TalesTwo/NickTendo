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
        End,
        
    }
    
    [Serializable]
    public struct DoorConfiguration
    {
        public bool NorthDoorActive;
        public bool EastDoorActive;
        public bool SouthDoorActive;
        public bool WestDoorActive;
        
        public override string ToString()
        {
            return $"N:{NorthDoorActive}, E:{EastDoorActive}, S:{SouthDoorActive}, W:{WestDoorActive}";
        }
        public int ActiveDoorCount()
        {
            int count = 0;
            if (NorthDoorActive) count++;
            if (EastDoorActive) count++;
            if (SouthDoorActive) count++;
            if (WestDoorActive) count++;
            return count;
        }
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
