using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

/*
 * The central hub for controlling all things boss fight
 *
 * This script is responsible for:
 * * initiating the arm attacks (finished)
 * * spawning minions (finished)
 * * shooting projectiles (finished)
 * * managing health
 * * calling for animations
 * * calling for the arms to perform actions (finished)
 * * managing vulnerable and invulnerable stages
 * * managing the state of the battle
 */
public class BossController : Singleton<BossController>
{
    [System.Serializable]
    public class Stats
    {
        [Header("Rockets")]
        public int rocketCountPerArm;
        public float rocketAttackTime;
        public int maxRocketLaunchesPerPhase;
        public float timeBetweenRocketLaunches;
        [Header("Minions")]
        public int numberOfFollowers;
        public int numberOfRanged;
        public int numberOfChaoticFollowers;
        public int enemiesDifficulty;
        public float timeBetweenEnemies;
        public float timeBetweenEnemyWaves;
        [Header("Projectiles")]
        public int projectileCount;
        public float projectileSpeed;
        public float projectileDamage;
        public float knockbackForce;
        public float stunTimer;
        public float followWaitTime;
        public int spreadWavesCount;
        public float spreadWaitTime;
        public float spreadAngle;
        public float timeBetweenProjectileWaves;
        [Header("Battle State")]
        public HealthState health;
        public int exhaustionCounter;
        public float exhaustionTime;
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
        Shooting,
        Summoning,
        Tired
    }

    public enum ProjectileState
    {
        Spread,
        Follow
    }
    
    [Header("State of the Fight")]
    public HealthState health;
    public BattleState battle;
    public ProjectileState projectile;
    private Stats _currentStats;
    private RoomGridManager _roomGridManager;
    
    [Header("Player Reference")]
    private PlayerController _playerController;
    private GameObject _player;
    
    [Header("Attacks")]
    public List<Stats> attacks;
    public GameObject bossProjectile;
    public float projectileSpawnDistance;

    public GameObject rocketProjectionHorizontal;
    public Vector2 horizontalProjectionOffset;
    public GameObject rocketProjectionVertical;
    public Vector2 verticalProjectionOffsetRight;
    public Vector2 verticalProjectionOffsetLeft;
    private Queue<GameObject> _rocketProjectionsQueue;

    [Header("Battle State Bools")] 
    private int _phases = 0;
    private bool _rightArmAttached = true;
    private bool _leftArmAttached = true;
    private bool _isSpawningEnemies = false;
    private bool _isShooting = false;
    private float _enemiesTimer = 25f;
    private float _projectilesTimer = 0f;
    private float _rightArmRandomTimer = 0f;
    private float _leftArmRandomTimer = 0f;
    private float _leftArmTimer = 0f;
    private float _rightArmTimer = 0f;
    private int _rightArmsLaunchedThisPhase = 0;
    private int _leftArmsLaunchedThisPhase = 0;
    private int _armsCurrentlyLaunched = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player");
        _playerController = _player.GetComponent<PlayerController>();
        _roomGridManager = transform.parent.GetComponent<RoomGridManager>();
        _rocketProjectionsQueue = new Queue<GameObject>();
        _currentStats = attacks[0];
        
        SetRandomRocketTimers(true, true);
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.L) && _leftArmAttached)
        {
            LaunchArm(leftArmController);
            _leftArmAttached = false;
        }

        if (Input.GetKeyDown(KeyCode.R) && _rightArmAttached)
        {
            LaunchArm(rightArmController);
            _rightArmAttached = false;
        }
        
        if (Input.GetKeyDown(KeyCode.B))
        {
            LaunchProjectile();
        }
        
        
        if (Input.GetKeyDown(KeyCode.M))
        {
            StartCoroutine(SpawnMinions());
        }
        
        
        // note: can only be tired when both arms are connected
        if (Input.GetKeyDown(KeyCode.X) && _leftArmAttached && _rightArmAttached)
        {
            rightArmController.BecomeTired();
            leftArmController.BecomeTired();
            
            BossScreenController.Instance.SetIsExhausted(true);
            
            
            // todo call to the animator to switch over to the exhausted face
        }
        */
        
        if (_phases >= _currentStats.exhaustionCounter && battle == BattleState.Idle && _leftArmAttached &&
            _rightArmAttached)
        {
            rightArmController.BecomeTired();
            leftArmController.BecomeTired();
            
            BossScreenController.Instance.SetIsExhausted(true);

            battle = BattleState.Tired;
            _phases = 0;
            StartCoroutine(ExhaustionTimer());
        }
        
        _enemiesTimer += Time.deltaTime;

        if (_enemiesTimer >= _currentStats.timeBetweenEnemyWaves && !_isSpawningEnemies && battle == BattleState.Idle)
        {
            
            _isSpawningEnemies = true;
            _enemiesTimer = 0f;
            _phases += 1;
            battle = BattleState.Summoning;
            StartCoroutine(SpawnMinions());
        }
        
        _projectilesTimer += Time.deltaTime;

        if (_projectilesTimer >= _currentStats.timeBetweenProjectileWaves && battle == BattleState.Idle && !_isShooting)
        {
            _isShooting = true;
            _projectilesTimer = 0f;
            _phases += 1;
            battle = BattleState.Shooting;
            LaunchProjectile();
        }

        _rightArmTimer += Time.deltaTime;
        _leftArmTimer += Time.deltaTime;

        if (_rightArmTimer >= _rightArmRandomTimer && _leftArmTimer >= _leftArmRandomTimer &&
            battle == BattleState.Idle && _leftArmAttached && _rightArmAttached && 
            _rightArmsLaunchedThisPhase < _currentStats.maxRocketLaunchesPerPhase && 
            _leftArmsLaunchedThisPhase < _currentStats.maxRocketLaunchesPerPhase)
        {
            // both arms launching at once
            _rightArmTimer = 0f;
            _leftArmTimer = 0f;
            _phases += 2;
            _armsCurrentlyLaunched += 2;
            _rightArmsLaunchedThisPhase += 1;
            _leftArmsLaunchedThisPhase += 1;
            battle = BattleState.RocketArms;
            
            LaunchArm(leftArmController);
            _leftArmAttached = false;
            LaunchArm(rightArmController);
            _rightArmAttached = false;
            SetRandomRocketTimers(true, true);
        }

        if (_rightArmTimer >= _rightArmRandomTimer && battle == BattleState.Idle && _rightArmAttached && 
            _rightArmsLaunchedThisPhase < _currentStats.maxRocketLaunchesPerPhase)
        {
            // launching the right arm only
            _rightArmTimer = 0f;
            _phases += 1;
            _armsCurrentlyLaunched += 1;
            _rightArmsLaunchedThisPhase += 1;
            battle = BattleState.RocketArms;
            
            LaunchArm(rightArmController);
            _rightArmAttached = false;
            SetRandomRocketTimers(false, true);
        }

        if (_leftArmTimer >= _leftArmRandomTimer && battle == BattleState.Idle && _leftArmAttached && 
            _leftArmsLaunchedThisPhase < _currentStats.maxRocketLaunchesPerPhase)
        {
            // launching the left arm only
            _leftArmTimer = 0f;
            _phases += 1;
            _armsCurrentlyLaunched += 1;
            _leftArmsLaunchedThisPhase += 1;
            battle = BattleState.RocketArms;
            
            LaunchArm(leftArmController);
            _leftArmAttached = false;
            SetRandomRocketTimers(true, false);
        }
        
        // reset state back to idle after smoothdamptime in the bossArmController (also use an integer to track how many calls must be received for the state to return to idle)

    }
    
    // todo add function to turn battle state back to idle after launching an arm
    public void BackToIdleState()
    {
        _armsCurrentlyLaunched -= 1;
        if (_armsCurrentlyLaunched == 0)
        {
            battle = BattleState.Idle;
        }
    }

    private void SetRandomRocketTimers(bool isLeft, bool isRight)
    {
        Random random = new Random();
        float min = 10f;
        
        if (isRight)
        {
            float randomValue = (float)random.NextDouble();
            _rightArmRandomTimer = randomValue * (_currentStats.timeBetweenRocketLaunches - min) + min;
        }

        if (isLeft)
        {
            float randomValue = (float)random.NextDouble();
            _leftArmRandomTimer = randomValue * (_currentStats.timeBetweenRocketLaunches - min) + min;
        }
    }

    private IEnumerator ExhaustionTimer()
    {
        float timer = 0f;
        while (timer < _currentStats.exhaustionTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        rightArmController.BecomeUntired();
        leftArmController.BecomeUntired();
        BossScreenController.Instance.SetIsExhausted(false);
        battle = BattleState.Idle;

        _rightArmsLaunchedThisPhase = 0;
        _leftArmsLaunchedThisPhase = 0;
    }

    public void TakeDamage()
    {
        rightArmController.BecomeUntired();
        leftArmController.BecomeUntired();
        
        BossScreenController.Instance.SetIsExhausted(false);
        
        battle = BattleState.Idle;

        switch (health)
        {
            case HealthState.Healthy:
                health = HealthState.Light;
                break;
            case HealthState.Light:
                health = HealthState.Medium;
                break;
            case HealthState.Medium:
                health = HealthState.Heavy;
                break;
            case HealthState.Heavy:
                health = HealthState.Dead;
                break;
            case HealthState.Dead:
                Destroy(gameObject);
                break;
        }

        foreach (Stats stat in attacks)
        {
            if (stat.health == health)
            {
                _currentStats = stat;
                break;
            }
        }

        _leftArmsLaunchedThisPhase = 0;
        _rightArmsLaunchedThisPhase = 0;
        _projectilesTimer = 0f;
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

    public void RightArmReturned()
    {
        _rightArmAttached = true;
    }

    public void LeftArmReturned()
    {
        _leftArmAttached = true;
    }

    public void ArmProjections(bool isHorizontal, float a, BossArmController.Direction direction)
    {
        if (isHorizontal)
        {
            Vector2 spawnPos = horizontalProjectionOffset;
            spawnPos.y = a;
            GameObject projection = Instantiate(rocketProjectionHorizontal, this.transform);
            projection.transform.localPosition = spawnPos;
            _rocketProjectionsQueue.Enqueue(projection);
        }
        else
        {
            Vector2 spawnPos;
            if (direction == BossArmController.Direction.Left)
            {
                spawnPos = new Vector2(a + verticalProjectionOffsetLeft.x, verticalProjectionOffsetLeft.y);
            }
            else
            {
                spawnPos = new Vector2(a + verticalProjectionOffsetRight.x, verticalProjectionOffsetRight.y);
            }
            GameObject projection = Instantiate(rocketProjectionVertical, this.transform);
            projection.transform.localPosition = spawnPos;
            _rocketProjectionsQueue.Enqueue(projection);
        }
    }

    public void RocketFinished()
    {
        GameObject projection = _rocketProjectionsQueue.Dequeue();
        Destroy(projection);
    }

    private void LaunchProjectile()
    {
        foreach (Stats stat in attacks)
        {
            if (stat.health == health)
            {
                if (projectile == ProjectileState.Follow)
                {
                    StartCoroutine(FollowProjectile(stat));
                    projectile = ProjectileState.Spread;
                }
                else if (projectile == ProjectileState.Spread)
                {
                    StartCoroutine(SpreadProjectile(stat));
                    projectile = ProjectileState.Follow;
                }
            }
        }

    }

    private IEnumerator FollowProjectile(Stats stat)
    {
        for (int i = 0; i < stat.projectileCount; i++)
        {
            // set direction of the projectile
            Vector2 direction = (_player.transform.position - transform.position).normalized;
            
            SpawnProjectile(stat, direction);
            
            // wait time for next projectile
            yield return new WaitForSeconds(stat.followWaitTime);
        }
        battle = BattleState.Idle;
        _isShooting = false;
    }

    private IEnumerator SpreadProjectile(Stats stat)
    {
        // initializing the size of the cone and number of porjectiles
        int vectorCount = stat.projectileCount * 2 + 1;
        float halfAngle = stat.spreadAngle / 2;
        Vector2 centerDirection = Vector2.down;
        float angleStep = stat.spreadAngle / (vectorCount - 1);

        bool launch = true;
        
        // launching waves of projectiles in a cone shape
        for (int i = 0; i < stat.spreadWavesCount; i++)
        {
            for (int j = 0; j < vectorCount; j++)
            {
                float currentAngle = -halfAngle + (j * angleStep);
                
                Quaternion rotation = Quaternion.Euler(0, 0, currentAngle);
                Vector2 direction = rotation * centerDirection;
                
                // spawning every second projectile for the sake of difference between waves
                if (launch == true)
                {
                    SpawnProjectile(stat, direction);
                    launch = false;
                }
                else
                {
                    launch = true;
                }
            }
            
            yield return new WaitForSeconds(stat.spreadWaitTime);
        }
        battle = BattleState.Idle;
        _isShooting = false;
        
        yield return null;
    }

    private void SpawnProjectile(Stats stat, Vector2 direction)
    {
        // instantiate a projectile and give it velocity
        Vector2 attackPosition = new Vector2(transform.position.x + direction.x * projectileSpawnDistance, transform.position.y + direction.y * projectileSpawnDistance);
        GameObject newProjectile = Instantiate(bossProjectile, attackPosition, Quaternion.identity);
        Rigidbody2D projectileRb = newProjectile.GetComponent<Rigidbody2D>();
        projectileRb.velocity = direction * stat.projectileSpeed;
        newProjectile.GetComponent<EnemyProjectileController>().SetAngle(direction);
        Managers.AudioManager.Instance.PlayEnemyShotSound();
                            
        // set damage of projectile
        EnemyProjectileController controller = newProjectile.GetComponent<EnemyProjectileController>();
        controller.SetDamage(stat.projectileDamage, stat.knockbackForce, stat.stunTimer); 
    }

    private IEnumerator SpawnMinions()
    {
        int follower = 0;
        int ranged = 0;
        int chaotic = 0;
        
        foreach (Stats stat in attacks)
        {
            if (stat.health == health)
            {
                while (stat.numberOfFollowers > follower || stat.numberOfRanged > ranged ||
                       stat.numberOfChaoticFollowers > chaotic)
                {
                    if (stat.numberOfFollowers != follower)
                    {
                        DungeonController.Instance.SpawnEnemyInCurrentRoomByType(Types.EnemyType.BOSS_FollowerEnemy, true, stat.enemiesDifficulty);
                        follower += 1;
                        yield return new WaitForSeconds(stat.timeBetweenEnemies);
                    }

                    if (stat.numberOfRanged != ranged)
                    {
                        DungeonController.Instance.SpawnEnemyInCurrentRoomByType(Types.EnemyType.BOSS_RangedEnemy, true, stat.enemiesDifficulty);
                        ranged += 1;
                        yield return new WaitForSeconds(stat.timeBetweenEnemies);
                    }

                    if (stat.numberOfChaoticFollowers != chaotic)
                    {
                        DungeonController.Instance.SpawnEnemyInCurrentRoomByType(Types.EnemyType.BOSS_ChaoticFollowerEnemy, true, stat.enemiesDifficulty);
                        chaotic += 1;
                        yield return new WaitForSeconds(stat.timeBetweenEnemies);
                    }                    
                }

            }
        }
        
        _isSpawningEnemies = false;
        battle = BattleState.Idle;
    }
}
