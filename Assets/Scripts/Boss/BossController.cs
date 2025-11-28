using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using Random = System.Random;

/*
 * The central hub for controlling all things boss fight
 *
 * This script is responsible for:
 * * initiating the arm attacks (finished)
 * * spawning minions (finished)
 * * shooting projectiles (finished)
 * * managing health (finished)
 * * calling for animations
 * * calling for the arms to perform actions (finished)
 * * managing vulnerable and invulnerable stages (finished)
 * * managing the state of the battle (finished)
 */
public class BossController : Singleton<BossController>
{
    [Header("debugging")]
    public bool debugging = false;
    
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
        public Sprite cracksOverlay;
        public int exhaustionCounter;
        public float exhaustionTime;
    }

    [System.Serializable]
    public class SmokeParticles
    {
        [Header("Smoke or sparks")]
        public GameObject smoke;
        public HealthState health;
    }
    
    [Header("body parts")]
    public GameObject rightArm;
    public BossArmController rightArmController;
    public GameObject leftArm;
    public BossArmController leftArmController;
    public GameObject face;
    public SpriteRenderer faceRenderer;
    public GameObject screen;
    public SpriteRenderer screenRenderer;
    public GameObject cracks;
    public SpriteRenderer cracksRenderer;
    public GameObject expressions;
    public SpriteRenderer expressionsRenderer;
    public AnimatedBossFace expressionsAnimator;

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
    
    [Header("particles")] 
    public List<SmokeParticles> smokeList;
    
    [Header("State of the Fight")]
    public HealthState health;
    public BattleState battle;
    public ProjectileState projectile;
    private Stats _currentStats;
    private CameraShake _cameraShake;
    private RoomGridManager _roomGridManager;
    
    [Header("Vulnerable State")]
    public Color hurtPulseColor = Color.red;
    public float pulseSpeed = 2f;
    public float minIntensity = 0.5f;
    public float maxIntensity = 1f;
    public float hitFlashDuration = 0.1f;
    private Color _originalColorFace;
    private Color _originalColorScreen;
    
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

    [Header("Death Sequence")] 
    public List<GameObject> finalExplosionParticles;
    public GameObject smallExplosionParticles;
    public float explosionMinY;
    public float explosionMaxY;
    public float explosionMinX;
    public float explosionMaxX;
    public float explosionLengthTime;
    public float maxTimeBetweenExplosions;
    public float minTimeBetweenExplosions;
    public float timeBetweenExplosionsChange;
    public float waitTimeAfterDeath;

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
    private bool _istired = false;
    private bool _playerAlive = true;
    private bool _isDying = false;
    private bool _damageTaken = false;
    
    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player");
        _playerController = _player.GetComponent<PlayerController>();
        _roomGridManager = transform.parent.GetComponent<RoomGridManager>();
        _cameraShake = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraShake>();
        _rocketProjectionsQueue = new Queue<GameObject>();
        _currentStats = attacks[0];
        
        _originalColorFace = faceRenderer.color;
        _originalColorScreen = screenRenderer.color;

        EventBroadcaster.PlayerDeath += Stop;
        
        SetRandomRocketTimers(true, true);
    }

    private void Stop()
    {
        StopAllCoroutines();
        _playerAlive = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (debugging) return;

        if (_isDying) return;
        
        if (!_playerAlive) return;
        
        if (_phases >= _currentStats.exhaustionCounter && battle == BattleState.Idle && _leftArmAttached &&
            _rightArmAttached)
        {
            rightArmController.BecomeTired();
            leftArmController.BecomeTired();
            
            BossScreenController.Instance.SetIsExhausted(true);
            expressionsAnimator.SetExhuastedAnimation();

            battle = BattleState.Tired;
            _phases = 0;
            StartCoroutine(ExhaustionTimer());
            StartCoroutine(VulnerablePulse(_originalColorFace, faceRenderer));
            StartCoroutine(VulnerablePulse(_originalColorScreen, screenRenderer));
        }
        
        _enemiesTimer += Time.deltaTime;

        if (_enemiesTimer >= _currentStats.timeBetweenEnemyWaves && !_isSpawningEnemies && battle == BattleState.Idle)
        {
            
            _isSpawningEnemies = true;
            _enemiesTimer = 0f;
            _phases += 1;
            battle = BattleState.Summoning;
            StartCoroutine(SpawnMinions());
            Managers.AudioManager.Instance.PlaySpawnEnemiesSound(1, 0);
            expressionsAnimator.SetLoadingMinionsAnimation();
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
    }

    private IEnumerator Explode()
    {
        float time = 0f;
        float waitTime = maxTimeBetweenExplosions;

        while (time <= explosionLengthTime)
        {
            time += Time.deltaTime;
            time += waitTime;
            float explosionX = RandomNumber(explosionMinX, explosionMaxX);
            float explosionY = RandomNumber(explosionMinY, explosionMaxY);
            Vector3 pos = new Vector3(transform.position.x + explosionX, transform.position.y + explosionY,
                transform.position.z);
            Instantiate(smallExplosionParticles, pos, Quaternion.identity);

            if (waitTime <= minTimeBetweenExplosions)
            {
                waitTime = minTimeBetweenExplosions;
            }
            else
            {
                waitTime -= timeBetweenExplosionsChange;
            }
            yield return new WaitForSeconds(waitTime);
        }

        foreach (GameObject explosion in finalExplosionParticles)
        {
            explosion.gameObject.SetActive(true);
        }
        
        foreach (SmokeParticles smoke in smokeList)
        {
            smoke.smoke.gameObject.SetActive(false);
        }
        face.gameObject.SetActive(false);
        screen.gameObject.SetActive(false);
        expressions.gameObject.SetActive(false);
        cracks.gameObject.SetActive(false);
        
        rightArmController.BossIsDeadArms();
        leftArmController.BossIsDeadArms();

        yield return new WaitForSeconds(waitTimeAfterDeath);
        EventBroadcaster.Broadcast_EndBossFight();
        GameStateManager.Instance.SetBuddeeDialogState("PostBossDefeat");
        EventBroadcaster.Broadcast_StartDialogue("BUDDEE");
    }

    private float RandomNumber(float min, float max)
    {
        return UnityEngine.Random.Range(min, max);
    }
    
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
        _istired = true;
        float dizzytimer = 0f;
        Managers.AudioManager.Instance.PlayBUDDEEDizzy(1, 0);
        while (timer < _currentStats.exhaustionTime && !_damageTaken)
        {
            dizzytimer += Time.deltaTime;
            if(dizzytimer >= 0.5f && _istired)
            {
                Managers.AudioManager.Instance.PlayBUDDEEDizzy(1, 0);
                dizzytimer = 0;
            }
            timer += Time.deltaTime;
            yield return null;
        }

        if (!_damageTaken)
        {
            rightArmController.BecomeUntired();
            leftArmController.BecomeUntired();
            BossScreenController.Instance.SetIsExhausted(false);
            expressionsAnimator.SetIdleAnimation();
            battle = BattleState.Idle;
            
            _rightArmsLaunchedThisPhase = 0;
            _leftArmsLaunchedThisPhase = 0;
        }

    }

    private void UnsetDamageTaken()
    {
        _damageTaken = false;
    }
    
    public void TakeDamage()
    {
        _damageTaken = true;
        Invoke(nameof(UnsetDamageTaken), 1f);
        
        rightArmController.BecomeUntired();
        leftArmController.BecomeUntired();
        _istired = false;

        BossScreenController.Instance.SetIsExhausted(false);
        expressionsAnimator.SetHurtAnimation();
        Invoke(nameof(SetIdleAnimation), 0.3f);
        
        battle = BattleState.Idle;
        Managers.AudioManager.Instance.PlayBUDDEEDamagedSound(1, 0);
        _cameraShake.ShakeOnce(0.2f, 0.6f);

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
                // Boss is killed, so broadcast to everyone that the boss fight has ended
                HandleBossDeath();
                break;
        }

        foreach (Stats stat in attacks)
        {
            if (stat.health == health)
            {
                _currentStats = stat;
                UpdateScreen();
                break;
            }
        }

        foreach (SmokeParticles particles in smokeList)
        {
            if (particles.health == _currentStats.health)
            {
                particles.smoke.gameObject.SetActive(true);
            }
        }

        _leftArmsLaunchedThisPhase = 0;
        _rightArmsLaunchedThisPhase = 0;
        _projectilesTimer = 0f;
    }

    private void HandleBossDeath()
    {
        Managers.AudioManager.Instance.PlayBUDDEEDyingSound(1, 0);
        _isDying = true;
        StartCoroutine(nameof(Explode));
        //EventBroadcaster.Broadcast_EndBossFight();
        // provide a one off dialogue line for defeating the boss
        //GameStateManager.Instance.SetBuddeeDialogState("PostBossDefeat");
        //EventBroadcaster.Broadcast_StartDialogue("BUDDEE");
        //Destroy(gameObject);
    }

    private void SetIdleAnimation()
    {
        expressionsAnimator.SetIdleAnimation();
    }

    private void UpdateScreen()
    {
        cracksRenderer.sprite = _currentStats.cracksOverlay;
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
            Managers.AudioManager.Instance.PlayBUDDEEShootSound();
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
            Managers.AudioManager.Instance.PlayBUDDEEShootSound();
            Managers.AudioManager.Instance.PlayBUDDEEShootSound();

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

    // the boss will pulse red when he is vulnerable
    private IEnumerator VulnerablePulse(Color original, SpriteRenderer spriteRenderer)
    {
        float t = 0f;
        while (battle == BattleState.Tired)
        {
            t += Time.deltaTime;

            float pulse = Mathf.PingPong(t, 1);
                
            float intensity = Mathf.Lerp(minIntensity, maxIntensity, pulse);
            Color current = Color.Lerp(original, hurtPulseColor, pulse);;
                
            current.r *= intensity;
            current.g *= intensity;
            current.b *= intensity;
                
            spriteRenderer.color = current; 
            yield return null;
        }

        spriteRenderer.color = hurtPulseColor;
        StartCoroutine(EndHitFlash(original, spriteRenderer));
    }
    
    private IEnumerator EndHitFlash(Color original, SpriteRenderer spriteRenderer)
    {
        float time = 0f;
        while (time < hitFlashDuration)
        {
            time += Time.deltaTime;
            yield return null;
        }
        spriteRenderer.color = original;
    }
}
