using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // stats
    [Header("Player Stats")]
    public float speed;
    public float attackCooldown = 1.0f;
    public float dashCooldown = 5.0f;
    
    
    // input variables
    [Header("inputs")]
    public float horizontalInput;
    public float verticalInput;
    
    // attack animations
    [Header("Attack Animations")]
    public GameObject attackAnimation;
    public GameObject dashAnimation;

    // rigidbody
    private Rigidbody2D rb;
    
    // dashing boolean
    private bool _isDashing = false;
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
            _isAttacking = true;
            Invoke(nameof(ResetAttack), attackCooldown);
        }
        
        // press left shift to perform a dash attack
        else if (Input.GetKeyDown(KeyCode.LeftShift) && !_isDashing)
        {
            StartDash();
            _isDashing = true;
            Invoke(nameof(ResetDash), dashCooldown);
        }
    }

    // Update is called once per frame
    // "fixedDeltaTime" is necessary instead of "Delta Time" for this method
    private void FixedUpdate()
    {
        // move player based on input
        Vector2 update = new Vector2(horizontalInput, verticalInput);
        rb.MovePosition(rb.position + update * speed * Time.fixedDeltaTime);
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
}
