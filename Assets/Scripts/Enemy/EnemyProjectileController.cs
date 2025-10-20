using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectileController : MonoBehaviour
{
    public GameObject particle;
    public float projectileDamage;
    public float ProjectileKnockback;
    public float stunTimer;

    public float lifeDuration = 5f;

    private GameObject _player;
    private PlayerController _playerController;
    private Rigidbody2D _rb;

    private bool _isPlayerAttack = false;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _playerController = _player.GetComponent<PlayerController>();
        _rb = gameObject.GetComponent<Rigidbody2D>();
        EventBroadcaster.PlayerDeath += DestroySelf;
        Invoke(nameof(DestroySelf), lifeDuration);
    }
    
    // on collision, destroy
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            DestroySelf();
        }

        if (other.gameObject.CompareTag("Player"))
        {
            Managers.AudioManager.Instance.PlayEnemyShotHitSound(1f, 0.2f);
            DoDamage();
            DestroySelf();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("PlayerAttack"))
        {
            Debug.Log(other.gameObject.name);
            _isPlayerAttack = true;
            Managers.AudioManager.Instance.PlayDeflectSound(1, 0.25f);
            Deflect();
        }
        
        if (other.gameObject.CompareTag("Enemy") && _isPlayerAttack)
        {
            Managers.AudioManager.Instance.PlayEnemyShotHitSound(1, 0);
            DestroySelf();
        }
    }

    // destroy with a particle effect
    private void DestroySelf()
    {
        Instantiate(particle, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    // deflect the enemy projectile and turn it into a player attack
    private void Deflect()
    {
        // turn into player attack
        gameObject.tag = "PlayerAttack";
        
        // deflect towards mouse position
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        Vector3 mouseDirection = (mousePosition - transform.position).normalized;
        float projectileSpeed = _rb.velocity.magnitude;
        _rb.velocity= mouseDirection * projectileSpeed;
    }

    // do damage to the player
    private void DoDamage()
    {
        Vector2 direction = new Vector2(_player.transform.position.x - transform.position.x, _player.transform.position.y - transform.position.y).normalized;
        _playerController.KnockBack(ProjectileKnockback, direction, stunTimer);
        _playerController.HitEffect(transform.position);
        PlayerStats.Instance.UpdateCurrentHealth(-projectileDamage);
    }

    // attacker sets damage when spawning projectile
    public void SetDamage(float damage, float knockback, float stun)
    {
        projectileDamage = damage;
        stunTimer = stun;
        ProjectileKnockback = knockback;
    }
}
