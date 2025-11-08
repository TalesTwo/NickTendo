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
    [Header("Arm Parts")] 
    public GameObject shoulder;
    public GameObject arm;
    public GameObject elbow;
    public GameObject forearm;
    public GameObject hand;

    private BossColliderController _shoulder;
    private BossColliderController _arm;
    private BossColliderController _elbow;
    private BossColliderController _forearm;
    private BossColliderController _hand;
    
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

    [Header("Rocket Attack")]
    public float offScreenXCoordinate = 30f;
    private Vector2 _velocity = Vector2.zero;
    public float smoothDampTime = 0.2f;
    private Vector2 _offScreenPos;
    private Vector2 _startPos;

    [Header("Exhausted Phase")] 
    public float angleDampTime = 0.2f;
    public float returnAngleDampTime;
    public float targetArmAngle;
    public float targetForearmAngle;
    private float _originalArmAngle;
    private float _originalForearmAngle;
    private float _armVelocity = 0f;
    private float _forearmVelocity = 0f;
    
    private bool _rocketReady = false;
    [Header("Arm Bounds")]
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
        
        _shoulder = shoulder.GetComponent<BossColliderController>();
        _arm = arm.GetComponent<BossColliderController>();
        _elbow = elbow.GetComponent<BossColliderController>();
        _forearm = forearm.GetComponent<BossColliderController>();
        _hand = hand.GetComponent<BossColliderController>();
    }

    public void BecomeTired()
    {
        StartCoroutine(MoveToTiredPosition());
    }

    public void BecomeUntired()
    {
        StartCoroutine(MoveToUntiredPosition());
    }

    private void SetExhaustedBools(bool isExhausted)
    {
        _shoulder.SetIsTired(isExhausted);
        _arm.SetIsTired(isExhausted);
        _elbow.SetIsTired(isExhausted);
        _forearm.SetIsTired(isExhausted);
        _hand.SetIsTired(isExhausted);
    }

    private IEnumerator MoveToUntiredPosition()
    {
        while (Mathf.Abs(arm.transform.eulerAngles.z - _originalArmAngle) > 0.1 ||
               Mathf.Abs(forearm.transform.eulerAngles.z - _originalForearmAngle) > 0.1)
        {
            float armAngle = Mathf.SmoothDampAngle(arm.transform.eulerAngles.z, _originalArmAngle, ref _armVelocity, returnAngleDampTime);
            float forearmAngle = Mathf.SmoothDampAngle(forearm.transform.eulerAngles.z, _originalForearmAngle, ref _forearmVelocity, returnAngleDampTime);

            arm.transform.rotation = Quaternion.Euler(arm.transform.eulerAngles.x, arm.transform.eulerAngles.y, armAngle);
            forearm.transform.rotation = Quaternion.Euler(forearm.transform.eulerAngles.x, forearm.transform.eulerAngles.y, forearmAngle);
            
            yield return null;
        }
        
        SetExhaustedBools(false);
    }

    private IEnumerator MoveToTiredPosition()
    {
        _originalArmAngle = arm.transform.eulerAngles.z;
        _originalForearmAngle = forearm.transform.eulerAngles.z;
        
        SetExhaustedBools(true);

        while (Mathf.Abs(arm.transform.eulerAngles.z - targetArmAngle) > 0.5 ||
               Mathf.Abs(forearm.transform.eulerAngles.z - targetForearmAngle) > 0.5)
        {
            float armAngle = Mathf.SmoothDampAngle(arm.transform.eulerAngles.z, targetArmAngle, ref _armVelocity, angleDampTime);
            float forearmAngle = Mathf.SmoothDampAngle(forearm.transform.eulerAngles.z, targetForearmAngle, ref _forearmVelocity, angleDampTime);

            arm.transform.rotation = Quaternion.Euler(arm.transform.eulerAngles.x, arm.transform.eulerAngles.y, armAngle);
            forearm.transform.rotation = Quaternion.Euler(forearm.transform.eulerAngles.x, forearm.transform.eulerAngles.y, forearmAngle);
            
            yield return null;
        }
    }
    
    public void LaunchAttack(int numberOfRockets, float rocketAttackTime)
    {
        Vector2 destination = new Vector2(offScreenXCoordinate * DirectionModifier, transform.position.y);
        _offScreenPos = destination;
        _startPos = transform.position;
        StartCoroutine(MoveArmOffScreen(destination));
        StartCoroutine(RocktAttack(numberOfRockets, rocketAttackTime));
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

    private IEnumerator RocktAttack(int numberOfRockets, float rocketAttackTime)
    {
        Vector2 start = Vector2.zero;
        
        // Step 1: waiting for arm to get in position
        while (!_rocketReady)
        {
            yield return null;
        }
        
        _rocketReady = false;

        Quaternion shoulderRotation = shoulder.transform.rotation;
        Quaternion armRotation = arm.transform.rotation;
        Quaternion forearmRotation = forearm.transform.rotation;
        Quaternion handRotation = hand.transform.rotation;
        
        // Step 2: adjust piece rotations
        if (side == Direction.Left)
        {
            shoulder.transform.rotation = Quaternion.Euler(0, 0, 90);
        } else if (side == Direction.Right)
        {
            shoulder.transform.rotation = Quaternion.Euler(0, 0, -90);
        }
        arm.transform.rotation = Quaternion.Euler(0, 0, 0);
        forearm.transform.rotation = Quaternion.Euler(0, 0, 0);
        hand.transform.rotation = Quaternion.Euler(0, 0, 0);

        for (int i = 0; i < numberOfRockets; i++)
        {
            // Step 3: determine launch spot and teleport there (adjust whole rotation)
            RocketDirection[] directions = (RocketDirection[])Enum.GetValues(typeof(RocketDirection));
            Random random = new Random();
            int randomIndex = random.Next(directions.Length);
            RocketDirection direction = directions[randomIndex];

            float x;
            float y;
            Vector2 destination = Vector2.zero;
            switch (direction)
            {
                case RocketDirection.Up:
                    x = (float) (minXCoordinate + (random.NextDouble() * (maxXCoordinate - minXCoordinate)));
                    start = new Vector2(x, startYCoordinateBottom);
                    transform.rotation = Quaternion.Euler(0, 0, 180);
                    Debug.Log("Up");
                    Debug.Log(transform.rotation.eulerAngles);
                    destination = new Vector2(start.x, start.y + (startYCoordinateTop - startYCoordinateBottom));
                    BossController.Instance.ArmProjections(false, x);
                    break;
                case RocketDirection.Down:
                    x = (float) (minXCoordinate + (random.NextDouble() * (maxXCoordinate - minXCoordinate)));
                    start = new Vector2(x, startYCoordinateTop);
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                    Debug.Log("Down");
                    Debug.Log(transform.rotation.eulerAngles);
                    destination = new Vector2(start.x, start.y + (startYCoordinateBottom - startYCoordinateTop));
                    BossController.Instance.ArmProjections(false, x);
                    break;
                case RocketDirection.Left:
                    y = (float) (minYCoordinate + (random.NextDouble() * (maxYCoordinate - minYCoordinate)));
                    start = new Vector2(startXCoordinateRight, y);
                    transform.rotation = Quaternion.Euler(0, 0, -90);
                    Debug.Log("Left");
                    Debug.Log(transform.rotation.eulerAngles);
                    destination = new Vector2(start.x + (startXCoordinateLeft - startXCoordinateRight), start.y);
                    BossController.Instance.ArmProjections(true, y);
                    break;
                case RocketDirection.Right:
                    y = (float) (minYCoordinate + (random.NextDouble() * (maxYCoordinate - minYCoordinate)));
                    start = new Vector2(startXCoordinateLeft, y);
                    transform.rotation = Quaternion.Euler(0, 0, 90);
                    Debug.Log("Right");
                    Debug.Log(transform.rotation.eulerAngles);
                    destination = new Vector2(start.x + (startXCoordinateRight - startXCoordinateLeft), start.y);
                    BossController.Instance.ArmProjections(true, y);
                    break;
                default:
                    break;
            }
            
            transform.localPosition = start;
            yield return null;
            
            // Step 4: fly across the screen until max coordinate is reached
            while (true)
            {
                transform.localPosition = Vector2.SmoothDamp(transform.localPosition, destination, ref _velocity, rocketAttackTime);
                if (Vector2.Distance(new Vector2(transform.localPosition.x, transform.localPosition.y), destination) < 2.0f)
                {
                    BossController.Instance.RocketFinished();
                    break;
                }
                yield return null;
            }            
        }
        
        // step 5: after all cycles, return to the head

        shoulder.transform.rotation = shoulderRotation;
        arm.transform.rotation = armRotation;
        forearm.transform.rotation = forearmRotation;
        hand.transform.rotation = handRotation;

        transform.position = _offScreenPos;
        
        while (true)
        {
            transform.position = Vector2.SmoothDamp(transform.position, _startPos, ref _velocity, smoothDampTime);
            if (Vector2.Distance(new Vector2(transform.position.x, transform.position.y), _startPos) < 0.01f)
            {
                if (side == Direction.Left)
                {
                    BossController.Instance.LeftArmReturned();
                } else if (side == Direction.Right)
                {
                    BossController.Instance.RightArmReturned();
                }
                break;
            }
            yield return null;
        }    
        

    }
}
