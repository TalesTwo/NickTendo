using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;
using static UnityEditor.Progress;

public class PlayerStats : Singleton<PlayerStats>
{
    // Start is called before the first frame update
    private string _playerName = "Player";

    private float _currentHealth = 3f;
    private float _maxHealth = 3f;
    private float _movementSpeed = 5f;
    private float _dashSpeed = 10f;
    private float _attackDamage = 2f; 
    private float _attackCooldown = 0.5f; 
    private float _dashDamage = 5f; 
    private float _dashCooldown = 5f; 
    private float _dashDistance = 0.5f; 
    private int _keys = 0;
    private int _coins = 0;
    
    public string GetPlayerName() { return _playerName; }
    
    public float GetCurrentHealth() { return _currentHealth; }
    public float GetMaxHealth() { return _maxHealth; }
    public float GetMovementSpeed() { return _movementSpeed; }
    public float GetDashSpeed() { return _dashSpeed; }
    public float GetAttackDamage() { return _attackDamage; }
    public float GetDashDamage() { return _dashDamage; }
    public float GetDashCooldown() { return _dashCooldown; }
    public float GetAttackCooldown() { return _attackCooldown; }
    public float GetDashDistance() { return _dashDistance; }
    public int GetKeys() { return _keys; } 
    public int GetCoins() { return _coins; }

    public void SetPlayerName(string NewName) { _playerName = NewName; }
    public void SetCurrentHealth(float NewHealth) { _currentHealth = NewHealth; }
    public void SetMaxHealth(float NewMaxHealth) { _maxHealth = NewMaxHealth; }
    public void SetMovementSpeed(float NewMovementSpeed) { _movementSpeed = NewMovementSpeed; }
    public void SetDashSpeed(float NewDashSpeed) { _dashSpeed = NewDashSpeed; }
    public void SetAttackDamage(float NewAttackDamage) { _attackDamage = NewAttackDamage; }
    public void SetDashDamage(float NewDashDamage) { _dashDamage = NewDashDamage; }
    public void SetDashCooldown(float NewDashCooldown) { _dashCooldown = NewDashCooldown; }
    public void SetAttackCooldown(float NewAttackCooldown) { _attackCooldown = NewAttackCooldown; }
    public void SetDashDistance(float NewDashDistance) { _dashDistance = NewDashDistance; }
    public void SetKeys(int NewKeys) { _keys = NewKeys; }
    public void SetCoins(int NewCoins) { _coins = NewCoins; }

    public void UpdateCurrentHealth(float UpdateValue)
    {
        _currentHealth += UpdateValue;
        if (_currentHealth > _maxHealth)
        {
            _currentHealth = _maxHealth;
        }
        else if (_currentHealth < 0)
        {
            _currentHealth = 0;
        }

        if (_currentHealth <= 0)
        {
            EventBroadcaster.Broadcast_PlayerDeath();
        }
    }
    public void UpdateMaxHealth(float UpdateValue) { _maxHealth += UpdateValue; }
    public void UpdateMovementSpeed(float UpdateValue) { _movementSpeed += UpdateValue; }
    public void UpdateDashSpeed(float UpdateValue) { _dashSpeed += UpdateValue; }
    public void UpdateAttackDamage(float UpdateValue) { _attackDamage += UpdateValue; }
    public void UpdateDashDamage(float UpdateValue) { _dashDamage += UpdateValue; }
    public void UpdateDashCooldown(float UpdateValue) { _dashCooldown += UpdateValue; }
    public void UpdateAttackCooldown(float UpdateValue) {_attackCooldown += UpdateValue; }
    public void UpdateDashDistance(float UpdateValue) { _dashDistance += UpdateValue; }
    public void UpdateKeys(int UpdateValue) { _keys += UpdateValue; }
    public void UpdateCoins(int UpdateValue) { _coins += UpdateValue; }

    public void DisplayAllStats()
    {
        DebugUtils.Log(
            "Player Name: " + _playerName +
            "\nCurrent Persona: " + PersonaManager.Instance.GetPersona() +
            "Current Health: " + _currentHealth + "/" + _maxHealth +
            "\nMovement Speed: " + _movementSpeed +
            "\nDash Speed: " + _dashSpeed +
            "\nAttack Damage: " + _attackDamage +
            "\nAttack Cooldown: " + _attackCooldown +
            "\nDash Damage: " + _dashDamage +
            "\nDash Cooldown: " + _dashCooldown +
            "\nDash Distance: " + _dashDistance +
            "\nKeys: " + _keys +
            "\nCoins: " + _coins
        );     
    }

    public void ApplyItemBuffs(PlayerStatsEnum BuffType, float BuffValue)
    {
        if (BuffType == PlayerStatsEnum.Max_Health)
        {
            UpdateMaxHealth(BuffValue);
        }
        else if (BuffType == PlayerStatsEnum.Movement_Speed)
        {
            UpdateMovementSpeed(BuffValue);
        }
        else if (BuffType == PlayerStatsEnum.Dash_Speed)
        {
            UpdateDashSpeed(BuffValue);
        }
        else if (BuffType == PlayerStatsEnum.Attack_Damage)
        {
            UpdateAttackDamage(BuffValue);
        }
        else if (BuffType == PlayerStatsEnum.Dash_Damage)
        {
            UpdateDashDamage(BuffValue);
        }
        else if (BuffType == PlayerStatsEnum.Dash_Cooldown)
        {
            UpdateDashCooldown(BuffValue);
        }
        else if (BuffType == PlayerStatsEnum.Attack_Cooldown)
        {
            UpdateAttackCooldown(BuffValue);
        }
        else if (BuffType == PlayerStatsEnum.Dash_Distance)
        {
            UpdateDashDistance(BuffValue);
        }
        else if (BuffType == PlayerStatsEnum.Current_Health)
        {
            if (BuffValue + GetCurrentHealth() >= GetMaxHealth())
            {
                BuffValue = GetMaxHealth();
            }
            UpdateCurrentHealth(BuffValue);
        }
        else if (BuffType == PlayerStatsEnum.Keys)
        {
            UpdateKeys(((int)BuffValue));
        }
        else if (BuffType == PlayerStatsEnum.Coins)
        {
            UpdateCoins((int)BuffValue);
        }
    }
    
    // function to initialize All of the player stats, based on a struct
    public void InitializePlayerStats(PlayerStatsStruct stats)
    {
        SetCurrentHealth(stats.CurrentHealth);
        SetMaxHealth(stats.MaxHealth);
        SetMovementSpeed(stats.MovementSpeed);
        SetDashSpeed(stats.DashSpeed);
        SetAttackDamage(stats.AttackDamage);
        SetAttackCooldown(stats.AttackCooldown);
        SetDashDamage(stats.DashDamage);
        SetDashCooldown(stats.DashCooldown);
        SetDashDistance(stats.DashDistance);
        SetKeys(stats.Keys);
        SetCoins(stats.Coins);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            DisplayAllStats();
        }
    }
}

public enum PlayerStatsEnum
{
    Current_Health,
    Max_Health,
    Movement_Speed,
    Dash_Speed,
    Attack_Damage,
    Dash_Damage,
    Dash_Cooldown,
    Attack_Cooldown,
    Dash_Distance,
    Keys,
    Coins
}

// create a struct to hold all of the player stats, which can be used to initialize the player stats
[Serializable]
public struct PlayerStatsStruct
{
    public float CurrentHealth;
    public float MaxHealth;
    public float MovementSpeed;
    public float DashSpeed;
    public float AttackDamage;
    public float AttackCooldown;
    public float DashDamage;
    public float DashCooldown;
    public float DashDistance;
    public int Keys;
    public int Coins;
    public Color PlayerColor;
}