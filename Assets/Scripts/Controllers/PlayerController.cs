using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // rigidbody & animator
    private Rigidbody2D _rb;
    private AnimatedPlayer _playerAnimator;
    private bool _isFacingRight = true;
    
    // dashing boolean
    private bool _isDashing = false;
    private bool _isDashMoving = false;
    private bool _isAttacking = false;
    private bool _isActive = true;        // blocks all player inputs when false (call broadcaster to toggle)
    
    // Start is called before the first frame update
    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _playerAnimator = GetComponent<AnimatedPlayer>();
        EventBroadcaster.StartStopAction += ToggleStartStop;
    }
    
    private void Update()
    {
        // get WASD input
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        if (_isActive)
        {
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
                _isAttacking = true;
                _playerAnimator.SetAttacking();
                Invoke(nameof(ResetAttack), PlayerStats.Instance.GetAttackCooldown());
            }
            
            // press RMB to perform a dash attack
            else if (Input.GetMouseButtonDown(1) && !_isDashing)
            {
                // start dash
                StartDash();
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

    // Update is called once per frame
    // "fixedDeltaTime" is necessary instead of "Delta Time" for this method
    private void FixedUpdate()
    {
        if (_isActive)
        {
            // move player based on input
            if (_isDashMoving)
            {
                float dashSpeed = PlayerStats.Instance.GetDashSpeed();
                _rb.MovePosition(_rb.position + new Vector2(_mouseDirection.x, _mouseDirection.y) * (dashSpeed * Time.fixedDeltaTime));
            }
            else
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
        Instantiate(attackAnimation);
    }
    
    // starts base dash attack
    private void StartDash()
    {
        Instantiate(dashAnimation);
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
    }

    private void Flip()
    {
        _isFacingRight = !_isFacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

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
}
