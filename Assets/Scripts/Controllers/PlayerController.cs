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

    // rigidbody
    private Rigidbody2D rb;
    
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
    }

    // Update is called once per frame
    // "fixedDeltaTime" is necessary instead of "Delta Time" for this method
    private void FixedUpdate()
    {
        // move player based on input
        Vector2 update = new Vector2(horizontalInput, verticalInput);
        rb.MovePosition(rb.position + update * speed * Time.fixedDeltaTime);
    }
}
