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

    public static float directionAngle;

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
        /*
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        
        Vector2 direction = mousePos - transform.position;
        
        directionAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        */
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
