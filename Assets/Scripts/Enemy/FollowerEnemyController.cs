using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowerEnemyController : MonoBehaviour
{
    public int health = 10;
    public float speed = 2;
    public float knockBackSpeed = 10;
    public float knockBackTime = 0.15f;
    public float hitFlashDuration = 0.1f;

    private Rigidbody2D _rb;
    private GameObject _player;
    private SpriteRenderer _renderer;
    private Color _color;
    
    public GameObject hitEffect;
    public float hitEffectDistance;
    
    private bool _isKnockback = false;

    private Vector3 _direction;
    
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
            transform.Translate(_direction * (speed * Time.deltaTime), Space.World);
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

    // find the intended direction of movement
    private Vector3 GetDirection()
    {
        return (_player.transform.position - transform.position).normalized;
    }

    // check for death condition
    private void CheckForDeath()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    // todo 
    // add particles
    private void SetKnockBack()
    {
        _isKnockback = true;
        _rb.AddForce(new Vector2(-_direction.x, -_direction.y) * knockBackSpeed, ForceMode2D.Impulse);
        StartCoroutine(HitFlash());
        Instantiate(hitEffect, transform.position + new Vector3(-_direction.x, 0, -_direction.y) * hitEffectDistance, Quaternion.identity);
        Invoke(nameof(ResetKnockBack), knockBackTime);
    }

    // reset knockback state
    private void ResetKnockBack()
    {
        _isKnockback = false;
    }

    // flash the sprite white to indicate a hit
    IEnumerator HitFlash()
    {
        _renderer.color = Color.white;
        yield return new WaitForSeconds(hitFlashDuration);
        _renderer.color = _color;
    }
}