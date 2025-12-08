using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossScreenController : Singleton<BossScreenController>
{
    private PlayerController _playerController;
    private GameObject _player;
    public GameObject immuneHitEffect;
    public float immuneParticleAngleEdit;
    public GameObject damageHitEffect;
    public float damageParticleAngleEdit;
    
    public AnimatedBossFace animatedBossFace;

    public float stunTimer = 0.1f;
    public float knockBackForce = 500;
    
    private bool _isExhausted = false;
    private bool _hitParticleCooldown = false;
    
    private void Start()
    {
        _player = GameObject.Find("Player");
        _playerController = _player.GetComponent<PlayerController>();
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name.Contains("BossProjectile"))
        {
            EnemyProjectileController projectile = collision.GetComponent<EnemyProjectileController>();
            if (projectile.GetIsPlayerAttack())
            {
                projectile.DestroySelf();
            }
        }
        
        if (collision.gameObject.CompareTag("PlayerAttack") && !_hitParticleCooldown)
        {
            if (!_isExhausted)
            {
                if (!collision.gameObject.name.Contains("BossProjectile"))
                {
                    PushPlayer();
                    Vector3 position = collision.ClosestPoint(transform.position);
                    Quaternion angle = GetAngle(position, immuneParticleAngleEdit);
                    HitEffect(immuneHitEffect, position, angle);                    
                }
                animatedBossFace.SetBlockedAnimation();
                Managers.AudioManager.Instance.PlayBUDDEENope();
                Managers.AudioManager.Instance.PlayBUDDEELaughSound();
            }
            else if (_isExhausted)
            {
                BossController.Instance.TakeDamage();
                Vector3 position = collision.ClosestPoint(transform.position);
                Quaternion angle = GetAngle(position, damageParticleAngleEdit);
                HitEffect(damageHitEffect, position, angle);
            }
        }
    }

    public void SetIsExhausted(bool isExhausted)
    {
        _isExhausted = isExhausted;
    }

    private void PushPlayer()
    {
        Vector2 direction = new Vector2(_player.transform.position.x - transform.position.x, _player.transform.position.y - transform.position.y).normalized;
        _playerController.KnockBack(knockBackForce, direction, stunTimer, true);
    }

    private void HitEffect(GameObject hit, Vector3 position, Quaternion angle)
    {
        GameObject particle = Instantiate(hit, position, angle);
        ParticleSystem particleS = particle.GetComponent<ParticleSystem>();
        Destroy(particle, particleS.main.duration);
        _hitParticleCooldown = true;
        Invoke(nameof(ResetHitParticle), 0.05f);
    }

    private void ResetHitParticle()
    {
        _hitParticleCooldown = false;
    }

    private Quaternion GetAngle(Vector3 position, float edit)
    {
        Vector3 direction = position - transform.position;
        direction.z = 0f;
        direction.Normalize();
        
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + edit;

        return Quaternion.Euler(0f, 0f, angle);
    }
    
}
