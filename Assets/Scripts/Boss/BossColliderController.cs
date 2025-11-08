using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This script controls the contact damage when the player touches the boss's arms or hands
 */
public class BossColliderController : MonoBehaviour
{
    private PlayerController _playerController;
    private GameObject _player;
    
    public int damage = 2;
    public float stunTimer = 0.5f;
    public float knockBackForce = 500;
    
    private bool _isTired = false;

    private void Start()
    {
        _player = GameObject.Find("Player");
        _playerController = _player.GetComponent<PlayerController>();
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player") && !_isTired)
        {
            DoDamage();
        }
    }
    
    private void DoDamage()
    {
        Vector2 direction = new Vector2(_player.transform.position.x - transform.position.x, _player.transform.position.y - transform.position.y).normalized;
        _playerController.KnockBack(knockBackForce, direction, stunTimer);
        _playerController.HitEffect(transform.position);
        Managers.AudioManager.Instance.PlayFollowerHitSound(1, 0);
        PlayerStats.Instance.UpdateCurrentHealth(-damage);
    }

    public void SetIsTired(bool isTired)
    {
        _isTired = isTired;
    }
}
