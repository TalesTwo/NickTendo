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
    public int DirectionModifier;

    public float offScreenXCoordinate = 30f;
    private Vector2 _velocity = Vector2.zero;
    public float smoothDampTime = 0.2f;
    public float moveSpeedAttack = 10f;
    
    private void Start() {
        if (side == Direction.Right)
        {
            DirectionModifier = 1;
        }
        else
        {
            DirectionModifier = -1;
        }
    }
    
    public void LaunchAttack()
    {
        Vector2 destination = new Vector2(offScreenXCoordinate * DirectionModifier, transform.position.y);
        StartCoroutine(MoveArm(destination));
    }

    private IEnumerator MoveArm(Vector2 destination)
    {
        while (true)
        {
            //transform.position = Vector2.MoveTowards(transform.position, destination, moveSpeedSafe * Time.deltaTime);
            //transform.position = Vector3.Lerp(transform.position, destination,  Time.deltaTime * moveSpeedSafe);
            transform.position = Vector2.SmoothDamp(transform.position, destination, ref _velocity, smoothDampTime);
            yield return null;            
        }
    }
}
