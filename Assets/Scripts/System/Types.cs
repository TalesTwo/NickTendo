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
        NE,
        NS,
        NW,
        ES,
        EW,
        SW,
        NES,
        NEW,
        NSW,
        ESW,
        NESW,
        End,
        DEFAULT,
        TutorialOne,
        TutorialTwo,
        TutorialThree
        
    }

    
    [Serializable]
    public struct DoorConfiguration
    {
        public bool NorthDoorActive;
        public bool EastDoorActive;
        public bool SouthDoorActive;
        public bool WestDoorActive;
        
        
        // A constructor to easily create configurations
        public DoorConfiguration(bool north, bool east, bool south, bool west)
        {
            NorthDoorActive = north;
            EastDoorActive = east;
            SouthDoorActive = south;
            WestDoorActive = west;
        }
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
    
    public enum SpawnableType
    {
        Enemy,
        Item,
        Trap
    }
    
    public enum PersonaState
    {
        Available,
        Selected,
        Locked,
        Lost
    }
    public enum Persona
    {
        Normal,
        Warrior,
        Speedster,
        Charmer,
        Tank,
        Assassin,
        None
    }

    public enum EnemyType
    {
        FollowerEnemy,
        RangedEnemy,
        ChestEnemy,
        PotEnemy,
        ChaoticFollowerEnemy,
        BOSS_FollowerEnemy,
        BOSS_RangedEnemy,
        BOSS_ChaoticFollowerEnemy,
    }
    
    /* ----------------- END GENERATION TYPES ----------------- */
    

}
