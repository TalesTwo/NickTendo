using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * The central hub for controlling all things boss fight
 *
 * This script is responsible for:
 * * initiating the arm attacks (finished)
 * * spawning minions
 * * shooting projectiles (next)
 * * managing health
 * * calling for animations
 * * calling for the arms to perform actions
 * * managing vulnerable and invulnerable stages
 * * managing the state of the battle
 */
public class BossController : MonoBehaviour
{
    [System.Serializable]
    public class Stats
    {
        [Header("Rockets")]
        public int rocketCountPerArm;
        public float rocketAttackTime;
        [Header("Minions")]
        public int enemiesSpawning;
        public int enemiesDifficulty;
        [Header("Projectiles")]
        public int projectileCount;
        public float projectileSpeed;
        public float projectileDamage;
        public float knockbackForce;
        public float stunTimer;
        [Header("Battle State")]
        public HealthState health;
    }
    
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
    
    [Header("Attacks")]
    public List<Stats> attacks;
    public GameObject bossProjectile;
    public float projectileSpawnDistance;
    
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
            LaunchArm(leftArmController);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            LaunchArm(rightArmController);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            LaunchProjectile();
        }
    }

    private void LaunchArm(BossArmController armController)
    {
        foreach (Stats stat in attacks)
        {
            if (stat.health == health)
            {
                armController.LaunchAttack(stat.rocketCountPerArm, stat.rocketAttackTime);
                break;
            }
        }
    }

    private void LaunchProjectile()
    {
        foreach (Stats stat in attacks)
        {
            if (stat.health == health)
            {
                Vector2 direction = (_player.transform.position - transform.position).normalized;
                // instantiate a projectile and give it velocity
                Vector2 attackPosition = new Vector2(transform.position.x + direction.x * projectileSpawnDistance, transform.position.y + direction.y * projectileSpawnDistance);
                GameObject newProjectile = Instantiate(bossProjectile, attackPosition, Quaternion.identity);
                Rigidbody2D ProjectileRb = newProjectile.GetComponent<Rigidbody2D>();
                ProjectileRb.velocity = direction * stat.projectileSpeed;
                newProjectile.GetComponent<EnemyProjectileController>().SetAngle(direction);
                Managers.AudioManager.Instance.PlayEnemyShotSound();
                            
                // set damage of projectile
                EnemyProjectileController controller = newProjectile.GetComponent<EnemyProjectileController>();
                controller.SetDamage(stat.projectileDamage, stat.knockbackForce, stat.stunTimer);                
            }
        }

    }
}
