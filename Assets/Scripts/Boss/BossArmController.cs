using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
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

    public GameObject particles;

    private BossColliderController _shoulder;
    private BossColliderController _arm;
    private BossColliderController _elbow;
    private BossColliderController _forearm;
    private BossColliderController _hand;
    private Rigidbody2D _rb;
    private GameObject _player;
    private CameraShake _cameraShake;
    public GameObject boss;
    
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
    public float cameraShakeTime = 0.5f;
    public float cameraShakeMagnitude = 0.4f;

    [Header("Exhausted Phase")] 
    public float angleDampTime = 0.2f;
    public float returnAngleDampTime;
    public float targetArmAngle;
    public float targetForearmAngle;
    private float _originalArmAngle;
    private float _originalForearmAngle;
    
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
    [Header("Bounds from Player")] 
    public float maxXCoordinateFromPlayer = 1.5f;
    public float minXCoordinateFromPlayer = -1.5f;
    public float maxYCoordinateFromPlayer = 1.5f;
    public float minYCoordinateFromPlayer = -1.5f;

    private int rocketcount = 0;

    private void Start() {
        if (side == Direction.Right)
        {
            DirectionModifier = 1;
        }
        else
        {
            DirectionModifier = -1;
        }
        
        _rb = GetComponent<Rigidbody2D>();
        _shoulder = shoulder.GetComponent<BossColliderController>();
        _arm = arm.GetComponent<BossColliderController>();
        _elbow = elbow.GetComponent<BossColliderController>();
        _forearm = forearm.GetComponent<BossColliderController>();
        _hand = hand.GetComponent<BossColliderController>();
        _player = GameObject.FindGameObjectWithTag("Player");
        _cameraShake = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraShake>();
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
        float time = 0f;

        Quaternion startArmRot = arm.transform.rotation;
        Quaternion startForearmRot = forearm.transform.rotation;

        Quaternion endArmRot = Quaternion.Euler(0, 0, _originalArmAngle);
        Quaternion endForearmRot = Quaternion.Euler(0, 0, _originalForearmAngle);
        
        while (time < returnAngleDampTime)
        {
            float t = time / returnAngleDampTime;
            
            t = Mathf.SmoothStep(0f, 1f, t);
            
            arm.transform.rotation = Quaternion.Lerp(startArmRot, endArmRot, t);
            forearm.transform.rotation = Quaternion.Lerp(startForearmRot, endForearmRot, t);
            
            time += Time.deltaTime;
            
            yield return null;
        }
        
        SetExhaustedBools(false);
    }

    private IEnumerator MoveToTiredPosition()
    {
        _originalArmAngle = arm.transform.eulerAngles.z;
        _originalForearmAngle = forearm.transform.eulerAngles.z;
        
        SetExhaustedBools(true);

        float time = 0f;
        
        Quaternion startArmRot = arm.transform.rotation;
        Quaternion startForearmRot = forearm.transform.rotation;
        
        Quaternion endArmRot = Quaternion.Euler(0, 0, targetArmAngle);
        Quaternion endForearmRot = Quaternion.Euler(0, 0, targetForearmAngle);
        
        while (time < angleDampTime)
        {
            float t = time / angleDampTime;
            
            t = Mathf.SmoothStep(0f, 1f, t);
            
            arm.transform.rotation = Quaternion.Lerp(startArmRot, endArmRot, t);
            forearm.transform.rotation = Quaternion.Lerp(startForearmRot, endForearmRot, t);
            
            time += Time.deltaTime;

            yield return null;
        }
    }
    
    public void LaunchAttack(int numberOfRockets, float rocketAttackTime)
    {
        Vector2 destination = new Vector2(transform.position.x + (offScreenXCoordinate * DirectionModifier), transform.position.y);
        _offScreenPos = destination;
        _startPos = transform.position;
        StartCoroutine(MoveArmOffScreen(destination));
        StartCoroutine(RocktAttack(numberOfRockets, rocketAttackTime));
    }

    private IEnumerator MoveArmOffScreen(Vector2 destination)
    {
        particles.gameObject.SetActive(true);
        
        float time = 0f;

        Vector2 startPos = transform.position;
        Vector2 endPos = destination;
        
        while (time < smoothDampTime)
        {
            _cameraShake.ShakeOnce(cameraShakeTime, cameraShakeMagnitude);
            
            float t = time / smoothDampTime;
            
            t = Mathf.SmoothStep(0f, 1f, t);
            
            transform.position = Vector2.Lerp(startPos, endPos, t);
            
            time += Time.deltaTime;

            yield return null;
        }
        _rocketReady = true;
        BossController.Instance.BackToIdleState();
    }

    private float GetPlayerEdit(bool _isVertical)
    {
        if (_isVertical)
        {
            return _player.transform.position.x - boss.transform.position.x;
        }
        return _player.transform.position.y - boss.transform.position.y;
    }

    private float CheckForBounds(float x, float max, float min)
    {
        if (x > max)
        {
            return max;
        } 
        if (x < min)
        {
            return min;
        }
        return x;
    }

    private IEnumerator RocktAttack(int numberOfRockets, float rocketAttackTime)
    {
        Vector2 start = Vector2.zero;
        Debug.Log(transform.localPosition);
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
        Quaternion particleRotation = particles.transform.rotation;
        
        // Step 2: adjust piece rotations

        if (side == Direction.Right)
        {
            shoulder.transform.rotation = Quaternion.Euler(0, 0, -90);
            particles.transform.rotation = Quaternion.Euler(0, 0, -90);
        } else if (side == Direction.Left)
        {
            shoulder.transform.rotation = Quaternion.Euler(0, 0, 90);
            particles.transform.rotation = Quaternion.Euler(0, 0, 90);
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

            // find x or y relation to the player
            float playerEdit;
            float x;
            float y;
            Vector2 destination = Vector2.zero;
            switch (direction)
            {
                case RocketDirection.Up:
                    playerEdit = GetPlayerEdit(true);
                    x = (float) (minXCoordinateFromPlayer + (random.NextDouble() * (maxXCoordinateFromPlayer - minXCoordinateFromPlayer))) + playerEdit;
                    x = CheckForBounds(x, maxXCoordinate, minXCoordinate);
                    start = new Vector2(x, startYCoordinateBottom);
                    transform.rotation = Quaternion.Euler(0, 0, 180);
                    Debug.Log("Up");
                    Debug.Log(transform.rotation.eulerAngles);
                    destination = new Vector2(start.x, start.y + (startYCoordinateTop - startYCoordinateBottom));
                    BossController.Instance.ArmProjections(false, x, side);
                    break;
                case RocketDirection.Down:
                    playerEdit = GetPlayerEdit(true);
                    x = (float) (minXCoordinateFromPlayer + (random.NextDouble() * (maxXCoordinateFromPlayer - minXCoordinateFromPlayer))) + playerEdit;
                    x = CheckForBounds(x, maxXCoordinate, minXCoordinate);
                    start = new Vector2(x, startYCoordinateTop);
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                    Debug.Log("Down");
                    Debug.Log(transform.rotation.eulerAngles);
                    destination = new Vector2(start.x, start.y + (startYCoordinateBottom - startYCoordinateTop));
                    BossController.Instance.ArmProjections(false, x, side);
                    break;
                case RocketDirection.Left:
                    playerEdit = GetPlayerEdit(false);
                    y = (float) (minYCoordinateFromPlayer + (random.NextDouble() * (maxYCoordinateFromPlayer - minYCoordinateFromPlayer))) + playerEdit;
                    y = CheckForBounds(y, maxYCoordinate, minYCoordinate);
                    start = new Vector2(startXCoordinateRight, y);
                    transform.rotation = Quaternion.Euler(0, 0, -90);
                    Debug.Log("Left");
                    Debug.Log(transform.rotation.eulerAngles);
                    destination = new Vector2(start.x + (startXCoordinateLeft - startXCoordinateRight), start.y);
                    BossController.Instance.ArmProjections(true, y, side);
                    break;
                case RocketDirection.Right:
                    playerEdit = GetPlayerEdit(false);
                    y = (float) (minYCoordinateFromPlayer + (random.NextDouble() * (maxYCoordinateFromPlayer - minYCoordinateFromPlayer))) + playerEdit;
                    y = CheckForBounds(y, maxYCoordinate, minYCoordinate);
                    start = new Vector2(startXCoordinateLeft, y);
                    transform.rotation = Quaternion.Euler(0, 0, 90);
                    Debug.Log("Right");
                    Debug.Log(transform.rotation.eulerAngles);
                    destination = new Vector2(start.x + (startXCoordinateRight - startXCoordinateLeft), start.y);
                    BossController.Instance.ArmProjections(true, y, side);
                    break;
                default:
                    break;
            }
            
            transform.localPosition = start;
            yield return null;

            float time = 0f;
            
            Vector2 startPos = transform.localPosition;
            
            // Step 4: fly across the screen until max coordinate is reached
            while (time < rocketAttackTime)
            {
                _cameraShake.ShakeOnce(cameraShakeTime, cameraShakeMagnitude);
                
                float t = time / rocketAttackTime;
                
                t = Mathf.SmoothStep(0f, 1f, t);
                
                transform.localPosition = Vector3.Lerp(startPos, destination, t);
                
                if (rocketcount == 120)
                {
                    Managers.AudioManager.Instance.PlayBUDDEEPunchSound(1, 0);
                    rocketcount = 0;
                }
                else ++rocketcount;

                time += Time.deltaTime;
                yield return null;
            }
            BossController.Instance.RocketFinished();
        }
        
        // step 5: after all cycles, return to the head
        transform.rotation = Quaternion.Euler(0, 0, 0);

        shoulder.transform.rotation = shoulderRotation;
        arm.transform.rotation = armRotation;
        forearm.transform.rotation = forearmRotation;
        hand.transform.rotation = handRotation;
        particles.transform.rotation = particleRotation;
        
        transform.position = _offScreenPos;

        float time2 = 0f;
        
        Vector2 startPos2 = transform.position;
        Vector2 endPos2 = _startPos;
        
        while (time2 < smoothDampTime)
        {
            float t = time2 / smoothDampTime;
            
            t = Mathf.SmoothStep(0f, 1f, t);
            
            transform.position = Vector3.Lerp(startPos2, endPos2, t);
            
            time2 += Time.deltaTime;
            yield return null;
        }
        
        if (side == Direction.Left)
        {
            BossController.Instance.LeftArmReturned();
        } else if (side == Direction.Right)
        {
            BossController.Instance.RightArmReturned();
        }
        
        particles.gameObject.SetActive(false);

    }

    public void BossIsDeadArms()
    {
        _rb.gravityScale = 1f;
        _rb.constraints = RigidbodyConstraints2D.None;
        if (side == Direction.Left)
        {
            _rb.AddForce(new Vector2(-5, 1), ForceMode2D.Impulse);
        } else if (side == Direction.Right)
        {
            _rb.AddForce(new Vector2(5, 1), ForceMode2D.Impulse);
        }
        _shoulder.TurnOffCollider();
        _arm.TurnOffCollider();
        _forearm.TurnOffCollider();
        _hand.TurnOffCollider();
        _elbow.TurnOffCollider();
        Invoke(nameof(SelfDestruct), 10f);
    }

    private void SelfDestruct()
    {
        Destroy(gameObject);
    }
}
