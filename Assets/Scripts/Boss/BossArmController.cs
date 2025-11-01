using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/*
 * This script controls how the arms interact with the world
 *
 * This script is responsible for:
 * * controlling the arms general movement
 * * controlling the arms movement during rocket mode (called by boss controller)
 * * controlling the arms movement during swing mode (called by boss controller)
 *
 * * calling for the hands to perform actions (if the hands are animated)
 */
public class BossArmController : MonoBehaviour
{
    [Header("arm parts")]
    public GameObject arm;
    public GameObject forearm;
    public GameObject hand;
    
    public enum Direction
    {
        Right,
        Left
    }
    
    public Direction side;
    
    private void Start()
    {
        
    }
}
