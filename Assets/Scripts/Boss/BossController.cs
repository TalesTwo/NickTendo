using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * The central hub for controlling all things boss fight
 *
 * This script is responsible for:
 * * initiating the arm attacks
 * * spawning minions
 * * shooting projectiles
 * * managing health
 * * calling for animations
 * * calling for the arms to perform actions
 * * managing vulnerable and invulnerable stages
 * * managing the state of the battle
 */
public class BossController : MonoBehaviour
{
    [Header("body parts")]
    public GameObject rightArm;
    public BossArmController rightArmController;
    public GameObject leftArm;
    public BossArmController leftArmController;
    public GameObject face;

    public enum HealthState
    {
        Healthy,
        Light,
        Medium,
        Heavy,
        Dead
    }

    public enum BattleState
    {
        Idle,
        RocketArms,
        BanHammer,
        Summoning
    }
    
    [Header("State of the Fight")]
    public HealthState health;
    public BattleState battle;
    private RoomGridManager _roomGridManager;
    
    [Header("Player Reference")]
    private PlayerController _playerController;
    private GameObject _player;
    
    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player");
        _playerController = _player.GetComponent<PlayerController>();
        _roomGridManager = transform.parent.GetComponent<RoomGridManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LaunchLeftArm();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            LaunchRightArm();
        }
    }

    // launching the left arm for rocket attack
    private void LaunchLeftArm()
    {
        leftArmController.LaunchAttack();
    }

    // launching the right arm for rocket attack
    private void LaunchRightArm()
    {
        rightArmController.LaunchAttack();
    }
}
