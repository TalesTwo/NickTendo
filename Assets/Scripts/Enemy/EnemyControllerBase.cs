using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControllerBase : SpawnableObject
{
    [Header("Stats & difficulty")]
    public TextAsset statLineCSV;
    public int difficulty;
    public int difficultyScalingFactor;
    
    [Header("stats")]
    protected float health = 10;
    protected float speed = 2;
    protected float knockBackSpeed = 10;
    protected float knockBackTime = 0.15f;
    protected float damage = 1f;
    protected float stunTimer;
    protected float knockbackForce;
    
    [Header("Game Components & objects")]
    private Rigidbody2D _rb;
    protected GameObject _player;
    protected PlayerController _playerController;
    private SpriteRenderer _renderer;
    private Color _color;
    protected Transform _playerTransform;
    protected Transform _transform;
    
    [Header("Hit Effects")]
    public GameObject hitEffect;
    public float hitEffectDistance;
    public float hitFlashDuration = 0.1f;
    
    [Header("state Bools")]
    private bool _isKnockback = false;

    [Header("direction of movement")]
    protected Vector3 _direction;
    
    [Header("PathFinding")]
    protected List<Node> currentPath;
    protected int targetIndex;
    protected RoomGridManager _gridManager;
    protected float findPathCooldown = 0;
    protected float pathingTimer = 0;
    [Header("Type")]
    public Types.EnemyType enemyType;

    private float _walktimer = 0f;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _renderer = GetComponent<SpriteRenderer>();
        _color = _renderer.color;
        _player = GameObject.Find("Player");
        _playerController = _player.GetComponent<PlayerController>();
        _transform = GetComponent<Transform>();
        _playerTransform = _player.GetComponent<Transform>();
        _gridManager = transform.parent.GetComponent<RoomGridManager>();
        EventBroadcaster.PlayerDeath += Deactivate;
        EventBroadcaster.ObjectFellInPit += OnFellInPit;
        EventBroadcaster.SetSeed += SetSeed;
        ParseStatsText();
    }
    private void SetSeed(int seed)
    {
        UnityEngine.Random.InitState(seed);
    }

    protected virtual void OnFellInPit(GameObject obj, Vector3 pitCenter)
    {
        // Overridden in child classes
    }

    protected virtual void Deactivate() 
    {
        
        Destroy(gameObject);
        // unsubscribe from event (added this line)
        EventBroadcaster.PlayerDeath -= Deactivate;
        EventBroadcaster.ObjectFellInPit -= OnFellInPit;
    }
    
    public void Initialize(int roomDifficulty)
    {
        _gridManager = transform.parent.GetComponent<RoomGridManager>();
        difficulty = roomDifficulty;
        ParseStatsText();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        // step 1: check death condition
        CheckForDeath();

        _direction = getPlayerDirection();
        
        pathingTimer += Time.deltaTime;
        if (pathingTimer > findPathCooldown)
        {
            pathingTimer = 0;
            FindPath();
            if (currentPath != null && currentPath.Count > 0)
            {
                StopAllCoroutines();
                StartCoroutine(Follow());
            }
        }
        
        Attack();
        
        _walktimer += Time.deltaTime;

        if (enemyType == Types.EnemyType.FollowerEnemy && _walktimer >= 0.25f)
        {
            Managers.AudioManager.Instance.PlayFollowMovementSound(1, 0.1f);
            _walktimer = 0;
        }
        if (enemyType == Types.EnemyType.RangedEnemy && _walktimer >= 0.3f)
        {
            Managers.AudioManager.Instance.PlayRangedEnemyMovementSound(1, 0.1f);
            _walktimer = 0;
        }
    }


    // reduce health on damage from a PlayerAttack
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("DashAttack"))
        {
            Managers.AudioManager.Instance.PlayEnemyDamagedSound();
            health -= (int)PlayerStats.Instance.GetDashDamage();
            SetKnockBack();
        } else if (collision.gameObject.CompareTag("PlayerAttack"))
        {
            Managers.AudioManager.Instance.PlayEnemyDamagedSound();
            Managers.AudioManager.Instance.PlaySwordHitSound(1f, 0.3f);

            health -= (int)PlayerStats.Instance.GetAttackDamage();
            SetKnockBack();
        }
    }
    
    // check for death condition
    private void CheckForDeath()
    {
        if (health <= 0)
        {
            //Destroy(gameObject);
            Deactivate(); // switching to this to ensure event is unsubscribed
        }
    }
    
    // Sets knockback and adds particle effect
    private void SetKnockBack()
    {
        _isKnockback = true;
        StopAllCoroutines();
        Vector2 knockBack = getKnockBackDirection();
        _rb.AddForce(knockBack * knockBackSpeed, ForceMode2D.Impulse);
        HitFlash();
        Quaternion angle = getEffectAngle();
        Instantiate(hitEffect, transform.position + new Vector3(-_direction.x, 0, -_direction.y) * hitEffectDistance, angle);
        Invoke(nameof(ResetKnockBack), knockBackTime);
    }

    // find the direction of knockback
    private Vector2 getKnockBackDirection()
    {
        Vector3 direction = (_player.transform.position - transform.position).normalized;
        return new Vector2(-direction.x, -direction.y);
    }

    private Vector2 getPlayerDirection()
    {
        return (_playerTransform.position - _transform.position).normalized;
    }
    
    // flash the sprite white to indicate a hit
    private void HitFlash()
    {
        _renderer.color = Color.white;
        Invoke(nameof(EndHitFlash),  hitFlashDuration);
    }

    private void EndHitFlash()
    {
        _renderer.color = _color;
    }
    
    // reset knockback state
    private void ResetKnockBack()
    {
        _isKnockback = false;
    }
    
    // gets angle for the particles
    private Quaternion getEffectAngle()
    {
        // step 1: get direction
        Vector3 direction = _player.transform.position - transform.position;
        direction.z = 0f;
        direction.Normalize();
        
        // step 2: set the rotation angle
        float angle = MathF.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 180;
        
        // Step 3: return the direction as Quaternion
        return Quaternion.Euler(0, 0, angle);
    }

    // splits the stats on a line
    private void ParseStatsText()
    {
        string[] lines = statLineCSV.text.Split('\n');
        double lineNumber = (double)difficulty/difficultyScalingFactor;
        lineNumber = Math.Ceiling(lineNumber);
        if (lineNumber > 5)
        {
            lineNumber = 5;
        }
        GetStats(lines[(int)lineNumber]);
    }

    // method MUST be overriden in child class
    protected virtual void GetStats(string statLine)
    {
        return;
    }

    protected virtual void FindPath()
    {
        return;
    }

    protected virtual IEnumerator Follow()
    {
        return null;
    }

    protected virtual void Attack()
    {
        return;
    }
}
