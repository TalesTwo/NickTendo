using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPositionController : MonoBehaviour
{
    private Transform _attacker;
    public float radius = 0.2f;
    public float verticalEdit = 0.2f;  // adjust vertical position of animation to account for animation wonkiness
    public float angleEdit; // adjust rotation of object to account for animation wonkiness

    // states
    private bool _isAttacking = false;

    private Vector3 _direction;
    private Vector3 _edit;
    
    void Start()
    {
        _attacker = GameObject.FindGameObjectWithTag("Player").transform;
        
        FollowMouse();
    }
    
    // Update is called once per frame
    void Update()
    {
        FollowMouse();
        SetPosition();
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
            _direction = mouseWorldPos - _attacker.position;
            _direction.z = 0f;
            _direction.Normalize();
            
            // calculate adjustment direction
            _edit = Vector2.Perpendicular(new Vector2(_direction.x, _direction.y));

            // Set position at radius from parent in the direction of the mouse
            //transform.position = _attacker.position + direction * radius + edit * verticalEdit;
            SetPosition();

            // Rotate to face the mouse
            float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg + angleEdit;
            transform.rotation = Quaternion.Euler(0, 0, angle);
            
            // change position on the first frame only
            _isAttacking = true;
        }

    }

    private void SetPosition()
    {
        transform.position = _attacker.position + _direction * radius + _edit * verticalEdit;
    }
}
