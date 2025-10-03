using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;

public class PlayerController : MonoBehaviour
{
    // stats
    [Header("Player Stats")]
    public float speed;
    public float dashSpeed = 10.0f;
    public float attackCooldown = 1.0f;
    public float dashCooldown = 5.0f;
    public float dashMovingCooldown = 0.5f;
    
    
    // input variables
    [Header("inputs")]
    public float horizontalInput;
    public float verticalInput;
    private Vector3 mouseDirection;
    
    // attack animations
    [Header("Attack Animations")]
    public GameObject attackAnimation;
    public GameObject dashAnimation;

    // rigidbody
    private Rigidbody2D rb;
    
    // dashing boolean
    private bool _isDashing = false;
    private bool _isDashMoving = false;
    private bool _isAttacking = false;
    
    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
    private void Update()
    {
        // get WASD input
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        
        // press the space bar to perform a slash attack
        if (Input.GetKeyDown(KeyCode.Space) && !_isAttacking)
        {
            StartAttack();
            AudioManager.Instance.PlaySwordSwingSound(1f, 0.1f);
            _isAttacking = true;
            Invoke(nameof(ResetAttack), attackCooldown);
        }
        
        // press left shift to perform a dash attack
        else if (Input.GetKeyDown(KeyCode.LeftShift) && !_isDashing)
        {
            // start dash
            StartDash();
            _isDashing = true;
            _isDashMoving = true;
            AudioManager.Instance.PlayDashSound(1f, 0.1f);
            // get dash direction
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            mouseDirection = (mousePosition - transform.position).normalized;
            // invoke dash cooldown
            Invoke(nameof(ResetDash), dashCooldown);
            Invoke(nameof(DashMovingStop), dashMovingCooldown);
        }
    }

    // Update is called once per frame
    // "fixedDeltaTime" is necessary instead of "Delta Time" for this method
    private void FixedUpdate()
    {
        // move player based on input
        if (_isDashMoving)
        {
            rb.MovePosition(rb.position + new Vector2(mouseDirection.x, mouseDirection.y) * (dashSpeed * Time.fixedDeltaTime));
        }
        else
        {
            Vector2 update = new Vector2(horizontalInput, verticalInput);
            rb.MovePosition(rb.position + update * speed * Time.fixedDeltaTime); 
        }

    }
    
    // starts base attack animation
    private void StartAttack()
    {
        attackAnimation.SetActive(true);
    }
    
    // starts base dash attack
    private void StartDash()
    {
        dashAnimation.SetActive(true);
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
}
