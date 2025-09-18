using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public int health = 10;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    // reduce health on damage from a PlayerAttack
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerAttack"))
        {
            AttackDamageController controller = collision.gameObject.GetComponent<AttackDamageController>();
            health -= controller.damage;
        }
    }
}
