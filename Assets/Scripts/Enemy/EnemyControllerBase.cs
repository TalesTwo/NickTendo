using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControllerBase : MonoBehaviour
{
    public int health = 10;
    public float speed = 2;
    public float knockBackSpeed = 10;
    public float knockBackTime = 0.15f;
    public float hitFlashDuration = 0.1f;
    
    private Rigidbody2D _rb;
    protected GameObject _player;
    private SpriteRenderer _renderer;
    private Color _color;
    
    public GameObject hitEffect;
    public float hitEffectDistance;
    
    private bool _isKnockback = false;

    protected Vector3 _direction;
    
    // Start is called before the first frame update
    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _renderer = GetComponent<SpriteRenderer>();
        _color = _renderer.color;
        _player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        // step 1: check death condition
        CheckForDeath();
        
        // step 2: get movement direction
        _direction = GetDirection();
        
        // step 3: check for knockback then move: direction dependent on knockback state
        if (!_isKnockback)
        {
            Move();
        }
    }
    
    // reduce health on damage from a PlayerAttack
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("DashAttack"))
        {
            health -= (int)PlayerStats.Instance.GetDashDamage();
            SetKnockBack();
        } else if (collision.gameObject.CompareTag("PlayerAttack"))
        {
            health -= (int)PlayerStats.Instance.GetAttackDamage();
            SetKnockBack();
        }
    }
    
    // check for death condition
    private void CheckForDeath()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
    
    // Sets knockback and adds particle effect
    private void SetKnockBack()
    {
        _isKnockback = true;
        _rb.AddForce(new Vector2(-_direction.x, -_direction.y) * knockBackSpeed, ForceMode2D.Impulse);
        StartCoroutine(HitFlash());
        Quaternion angle = getEffectAngle();
        Instantiate(hitEffect, transform.position + new Vector3(-_direction.x, 0, -_direction.y) * hitEffectDistance, angle);
        Invoke(nameof(ResetKnockBack), knockBackTime);
    }
    
    // flash the sprite white to indicate a hit
    IEnumerator HitFlash()
    {
        _renderer.color = Color.white;
        yield return new WaitForSeconds(hitFlashDuration);
        _renderer.color = _color;
    }
    
    // reset knockback state
    private void ResetKnockBack()
    {
        _isKnockback = false;
    }
    
    // gets angle for the particles
    private Quaternion getEffectAngle()
    {
        // step 1: get direction
        Vector3 direction = _player.transform.position - transform.position;
        direction.z = 0f;
        direction.Normalize();
        
        // step 2: set the rotation angle
        float angle = MathF.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 180;
        
        // Step 3: return the direction as Quaternion
        return Quaternion.Euler(0, 0, angle);
    }

    // class MUST be overidden by child to move
    protected virtual Vector3 GetDirection()
    {
        return Vector3.zero;
    }

    // ovverideing this metod is optional if the enemy has a different movement algorithm
    protected virtual void Move()
    {
        transform.Translate(_direction * (speed * Time.deltaTime), Space.World);
    }
}
