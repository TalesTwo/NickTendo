using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossScreenController : Singleton<BossScreenController>
{
    private PlayerController _playerController;
    private GameObject _player;

    public float stunTimer = 0.1f;
    public float knockBackForce = 500;
    
    private bool _isExhausted = false;
    
    private void Start()
    {
        _player = GameObject.Find("Player");
        _playerController = _player.GetComponent<PlayerController>();
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerAttack"))
        {
            if (!_isExhausted)
            {
                PushPlayer();
            }
            else if (_isExhausted)
            {
                BossController.Instance.TakeDamage();
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
        // todo add new particle hit effect when the boss is false hit
    }
    
}
