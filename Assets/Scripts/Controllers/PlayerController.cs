using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;
    
    public float horizontalInput;
    public float verticalInput;

    private Rigidbody2D rb;
    
    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        Vector2 update = new Vector2(horizontalInput, verticalInput);
        
        rb.MovePosition(rb.position + update * speed * Time.fixedDeltaTime);

        //transform.Translate(new Vector3(horizontalInput * speed * Time.deltaTime, verticalInput * speed * Time.deltaTime, 0));
    }
}
