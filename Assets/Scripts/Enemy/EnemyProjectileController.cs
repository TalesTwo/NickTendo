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
    public float angleEdit = 45f;
    private Quaternion _rotation;

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
        EventBroadcaster.StartBossFightDeathSequence += DestroySelf;
        Invoke(nameof(DestroySelf), lifeDuration);
        
        // Fix to stop the projectiles from colliding with pickups
        int projectileLayer = LayerMask.NameToLayer("Projectile");
        int lootLayer = LayerMask.NameToLayer("Loot");
        Physics2D.IgnoreLayerCollision(projectileLayer, lootLayer, true);
        EventBroadcaster.ReturnToMainMenu += DestroySelf;
    }
    
    // on collision, destroy
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Wall") || other.gameObject.CompareTag("Spawnable"))
        {
            Managers.AudioManager.Instance.PlayEnemyShotMissSound(1, 0);
            DestroySelf();
        }

        if (other.gameObject.CompareTag("Player"))
        {
            if (!gameObject.CompareTag("PlayerAttack") && !_playerController.IsDashing())
            {
                Managers.AudioManager.Instance.PlayEnemyShotHitSound(1f, 0.2f);
                DoDamage();
                DestroySelf();                
            }
            else
            {
                DestroySelf();
            }

        }
        
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // make sure that what we hit is a player attack, and that we are NOT a player attack
        if (other.gameObject.CompareTag("PlayerAttack") && !gameObject.CompareTag("PlayerAttack"))
        {
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
        EventBroadcaster.PlayerDeath -= DestroySelf;
        EventBroadcaster.StartBossFightDeathSequence -= DestroySelf;
        EventBroadcaster.ReturnToMainMenu -= DestroySelf;
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
        SetAngle(mouseDirection);
        float projectileSpeed = _rb.velocity.magnitude;
        _rb.velocity= mouseDirection * projectileSpeed;
        // if we have been deflected, increase damage
        projectileDamage *= 1.5f;
    }

    // do damage to the player
    private void DoDamage()
    {
        PlayerStats.Instance.UpdateCurrentHealth(-projectileDamage);
        Vector2 direction = new Vector2(_player.transform.position.x - transform.position.x, _player.transform.position.y - transform.position.y).normalized;
        _playerController.KnockBack(ProjectileKnockback, direction, stunTimer);
        _playerController.HitEffect(transform.position);
        
    }

    // attacker sets damage when spawning projectile
    public void SetDamage(float damage, float knockback, float stun)
    {
        projectileDamage = damage;
        stunTimer = stun;
        ProjectileKnockback = knockback;
    }

    // change angle depending on the direction of the projectile
    public void SetAngle(Vector3 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + angleEdit;
        _rotation = Quaternion.Euler(0, 0, angle);
        transform.rotation = _rotation;
    }
}
