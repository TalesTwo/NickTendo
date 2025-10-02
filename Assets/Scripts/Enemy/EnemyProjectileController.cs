using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectileController : MonoBehaviour
{
    public GameObject particle;
    public float projectileDamage;

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
        PlayerStats.Instance.UpdateCurrentHealth(projectileDamage);
    }

    // attacker sets damage when spawning projectile
    public void SetDamage(float damage)
    {
        projectileDamage = damage;
    }
}
