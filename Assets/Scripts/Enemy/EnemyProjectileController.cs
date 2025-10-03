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

    private GameObject _player;
    private PlayerController _playerController;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _playerController = _player.GetComponent<PlayerController>();
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
            DoDamage();
            DestroySelf();
        }
    }

    // destroy with a particle effect
    private void DestroySelf()
    {
        Instantiate(particle, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    // do damage to the player
    private void DoDamage()
    {
        Vector2 direction = new Vector2(_player.transform.position.x - transform.position.x, _player.transform.position.y - transform.position.y).normalized;
        _playerController.KnockBack(ProjectileKnockback, direction, stunTimer);
        PlayerStats.Instance.UpdateCurrentHealth(projectileDamage);
    }

    // attacker sets damage when spawning projectile
    public void SetDamage(float damage, float knockback, float stun)
    {
        projectileDamage = damage;
        stunTimer = stun;
        ProjectileKnockback = knockback;
    }
}
