using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;

public class PlayerController : MonoBehaviour
{
    // input variables
    [Header("inputs")]
    public float horizontalInput;
    public float verticalInput;
    private Vector3 _mouseDirection;
    
    // attack animations
    [Header("Attack Animations")]
    public GameObject attackAnimation;
    public GameObject dashAnimation;
    
    // effect for getting hit
    [Header("Hit Effects")]
    public GameObject hitEffect;
    public float hitEffectDistance;

    // rigidbody & animator
    private Rigidbody2D _rb;
    private SpriteRenderer _sr;
    private AnimatedPlayer _playerAnimator;
    private bool _isFacingRight = true;
    
    // dashing boolean
    private bool _isDashing = false;
    private bool _isDashMoving = false;
    private bool _isAttacking = false;
    private bool _isActive = true;        // blocks all player inputs when false (call broadcaster to toggle)
    private bool _isKnockback = false;
    private bool _isDead = false;
    private bool _isWalking = false;

    // Start is called before the first frame update
    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _playerAnimator = GetComponent<AnimatedPlayer>();
        _sr = GetComponent<SpriteRenderer>();
        EventBroadcaster.StartStopAction += ToggleStartStop;
    }
    
    private void Update()
    {
        // get WASD input
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        
        if (_isActive && !_isDead)
        {
            
            // set default animation to running or ide depending on movement
            if (!_isDashMoving)
            {
                if (horizontalInput == 0 && verticalInput == 0)
                {
                    StopWalkSound();
                    _playerAnimator.SetStill();
                }
                else
                {
                    StartWalkSound();
                    _playerAnimator.SetRunning();
                }                  
            }
          
            
            // flip sprite along y axis if direction changes
            if (horizontalInput < 0 && _isFacingRight && !(_isAttacking || _isDashMoving))
            {
                Flip();
            }
            else if (horizontalInput > 0 && !_isFacingRight && !(_isAttacking || _isDashMoving))
            {
                Flip();
            }
            
            // press LMB to perform a slash attack
            if (Input.GetMouseButtonDown(0) && !_isAttacking)
            {
                StartAttack();
                AudioManager.Instance.PlaySwordSwingSound(0.5f, 0.1f);
                _isAttacking = true;
                _playerAnimator.SetAttacking();
                Invoke(nameof(ResetAttack), PlayerStats.Instance.GetAttackCooldown());
            }
            
            // press RMB to perform a dash attack
            else if (Input.GetMouseButtonDown(1) && !_isDashing)
            {
                // start dash
                StartDash();
                _playerAnimator.SetDashing();
                AudioManager.Instance.PlayDashSound(1, 0.1f);
                _isDashing = true;
                _isDashMoving = true;
                // get dash direction
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePosition.z = 0;
                _mouseDirection = (mousePosition - transform.position).normalized;
                // invoke dash cooldown
                Invoke(nameof(ResetDash), PlayerStats.Instance.GetDashCooldown());
                Invoke(nameof(DashMovingStop), PlayerStats.Instance.GetDashDistance());
            }            
        }

    }

    public void StartWalkSound()
    {
        if (!_isWalking)
        {
            _isWalking = true;
            StartCoroutine(WalkingSoundLoop());
        }
    }

    public void StopWalkSound()
    {
        if (_isWalking)
        {
            _isWalking = false;
            StopAllCoroutines();
        }
    }




    private IEnumerator WalkingSoundLoop()
    {
        for (int x = 0; x <= 222; ++x)
        {
            if(x == 222)
            {
                Managers.AudioManager.Instance.PlayWalkingSound(1, 0);
                x = 0;
            }
            yield return null;
        }
    }

    // Update is called once per frame
    // "fixedDeltaTime" is necessary instead of "Delta Time" for this method
    private void FixedUpdate()
    {
        if (_isActive && !_isDead)
        {
            // move player based on input
            if (_isDashMoving && !_isKnockback)
            {
                float dashSpeed = PlayerStats.Instance.GetDashSpeed();
                _rb.MovePosition(_rb.position + new Vector2(_mouseDirection.x, _mouseDirection.y) * (dashSpeed * Time.fixedDeltaTime));
            }
            else if (!_isKnockback)
            {
                Vector2 update = new Vector2(horizontalInput, verticalInput);
                float speed = PlayerStats.Instance.GetMovementSpeed();
                _rb.MovePosition(_rb.position + update * (speed * Time.fixedDeltaTime)); 
            }            
        }
    }
    
    // starts base attack animation
    private void StartAttack()
    {
        StopWalkSound();
        GameObject attack = Instantiate(attackAnimation);
        Renderer rnd = attack.gameObject.GetComponent<Renderer>();
        Color playerColor = _sr.color;
        rnd.material.color = playerColor;
    }
    
    // starts base dash attack
    private void StartDash()
    {
        StopWalkSound();
        // instantiate dash
        GameObject attack = Instantiate(dashAnimation);
        
        // get dash direction
        AttackPositionController trn = attack.gameObject.GetComponent<AttackPositionController>();
        trn.FindRotation();
        
        // get player + dash color
        Renderer rnd = attack.gameObject.GetComponent<Renderer>();
        Color playerColor = _sr.color;
        
        // set values
        _playerAnimator.SetDashAngle(attack.transform.rotation);
        rnd.material.color = playerColor;
    }

    // reset the dash attack after the cooldown
    private void ResetDash()
    {
        _isDashing = false;
    }

    // reset the standard attack after the cooldown
    private void ResetAttack()
    {
        _isAttacking = false;
    }

    // stop the dash
    private void DashMovingStop()
    {
        _isDashMoving = false;
        _playerAnimator.SetStill();
        _playerAnimator.ResetDashAngle();
    }

    // flips the sprite depending on the direction of movement
    private void Flip()
    {
        _isFacingRight = !_isFacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    // starts and stops all player input
    private void ToggleStartStop()
    {
        if (_isActive)
        {
            _isActive = false;
        }
        else
        {
            _isActive = true;
        }
    }

    // player is hit by attack that has knockback. knockback is physics based
    public void KnockBack(float power, Vector2 direction, float stunTimer)
    {
        StopWalkSound();
        _isKnockback = true;
        Invoke(nameof(UnsetKnockback), stunTimer);
        _rb.AddForce(direction * power, ForceMode2D.Impulse);
        _playerAnimator.SetHurting();
    }

    public void HitEffect(Vector3 enemyPosition)
    {
        Quaternion angle = getHitEffectAngle(enemyPosition);
        Vector3 direction = (enemyPosition - transform.position).normalized;
        Instantiate(hitEffect, transform.position + direction * hitEffectDistance, angle);
    }
    
    // gets angle for the particles
    private Quaternion getHitEffectAngle(Vector3 enemyPosition)
    {
        // step 1: get direction
        Vector3 direction = enemyPosition - transform.position;
        direction.z = 0f;
        direction.Normalize();
        
        // step 2: set the rotation angle
        float angle = MathF.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 180;
        
        // Step 3: return the direction as Quaternion
        return Quaternion.Euler(0, 0, angle);
    }

    // unstun the player after knockback
    private void UnsetKnockback()
    {
        _isKnockback = false;
    }

    // function to set player as dead
    public void SetIsDead()
    {
        StopWalkSound();
        _isDead = true;
    }

    // function to set player as alive again
    public void ResetIsDead()
    {
        _isDead = false;
    }

    public bool IsDashing()
    {
        return _isDashMoving;
    }

}
