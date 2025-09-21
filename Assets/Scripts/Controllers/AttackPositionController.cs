using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPositionController : MonoBehaviour
{
    public Transform attacker;
    public float radius = 0.2f;
    public float verticalEdit = 0.2f;  // adjust vertical position of animation to account for animation wonkiness
    public float angleEdit; // adjust rotation of object to account for animation wonkiness

    // states
    private bool _isAttacking = false;
    
    // Update is called once per frame
    void Update()
    {
        FollowMouse();
    }

    private void OnDisable()
    { 
        _isAttacking = false;
    }

    private void FollowMouse()
    {
        // only change position when animation is not playing
        if (!_isAttacking)
        {
            // Get mouse position in world space
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0f;

            // Get direction from parent to mouse
            Vector3 direction = mouseWorldPos - attacker.position;
            direction.z = 0f;
            direction.Normalize();
            
            // calculate adjustment direction
            Vector3 edit = Vector2.Perpendicular(new Vector2(direction.x, direction.y));

            // Set position at radius from parent in the direction of the mouse
            transform.position = attacker.position + direction * radius + edit * verticalEdit;

            // Rotate to face the mouse
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + angleEdit;
            transform.rotation = Quaternion.Euler(0, 0, angle);
            
            // change position on the first frame only
            _isAttacking = true;
        }

    }
}
