using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // player speed
    public float speed;
    
    // input variables
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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartAttack();
        }
        else if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            StartDash();
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
}
