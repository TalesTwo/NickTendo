using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = System.Random;

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

    public enum RocketDirection
    {
        Up,
        Down,
        Left,
        Right
    }
    
    public Direction side;
    public int DirectionModifier;

    public float offScreenXCoordinate = 30f;
    private Vector2 _velocity = Vector2.zero;
    public float smoothDampTime = 0.2f;
    public float moveSpeedAttack = 10f;
    
    [Header("Attack Coordinates")]
    private bool _rocketReady = false;
    public float maxXCoordinate = 10f;
    public float minXCoordinate = -10f;
    public float maxYCoordinate = 0f;
    public float minYCoordinate = -20f;
    public float startXCoordinateRight = 30f;
    public float startXCoordinateLeft = -30f;
    public float startYCoordinateBottom = -30f;
    public float startYCoordinateTop = 15f;
    
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
        StartCoroutine(MoveArmOffScreen(destination));
        StartCoroutine(RocktAttack());
    }

    private IEnumerator MoveArmOffScreen(Vector2 destination)
    {
        while (true)
        {
            transform.position = Vector2.SmoothDamp(transform.position, destination, ref _velocity, smoothDampTime);
            if (Vector2.Distance(new Vector2(transform.position.x, transform.position.y), destination) < 1.0f)
            {
                _rocketReady = true;
                break;
            }
            yield return null;
        }
    }

    private IEnumerator RocktAttack()
    {
        Vector2 start = Vector2.zero;
        
        // Step 1: waiting for arm to get in position
        while (!_rocketReady)
        {
            yield return null;
        }
        
        // Step 2: adjust piece rotations
        arm.transform.rotation = Quaternion.Euler(0, 0, 0);
        forearm.transform.rotation = Quaternion.Euler(0, 0, 0);
        hand.transform.rotation = Quaternion.Euler(0, 0, 0);
        
        // Step 3: determine launch spot and teleport there (adjust whole rotation)
        RocketDirection[] directions = (RocketDirection[])Enum.GetValues(typeof(RocketDirection));
        Random random = new Random();
        int randomIndex = random.Next(directions.Length);
        RocketDirection direction = directions[randomIndex];

        float x;
        float y;
        switch (direction)
        {
            case RocketDirection.Up:
                x = (float) (minXCoordinate + (random.NextDouble() * (maxXCoordinate - minXCoordinate)));
                start = new Vector2(x, startYCoordinateBottom);
                transform.rotation = Quaternion.Euler(0, 0, 180);
                break;
            case RocketDirection.Down:
                x = (float) (minXCoordinate + (random.NextDouble() * (maxXCoordinate - minXCoordinate)));
                start = new Vector2(x, startYCoordinateTop);
                transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case RocketDirection.Left:
                y = (float) (minYCoordinate + (random.NextDouble() * (maxYCoordinate - minYCoordinate)));
                start = new Vector2(startXCoordinateRight, y);
                transform.rotation = Quaternion.Euler(0, 0, -90);
                break;
            case RocketDirection.Right:
                y = (float) (minYCoordinate + (random.NextDouble() * (maxYCoordinate - minYCoordinate)));
                start = new Vector2(startXCoordinateLeft, y);
                transform.rotation = Quaternion.Euler(0, 0, 90);
                break;
            default:
                break;
        }
        
        
        transform.localPosition = start;
        
        // Step 4: fly across the screen until max coordinate is reached

        // Step 5: repeat until phase is over
    }
}
