using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : Singleton<PlayerStats>
{
    // Start is called before the first frame update

    private float CurrentHealth = 3f;
    private float MaxHealth = 3f;
    private float MovementSpeed = 5f;
    private float DashSpeed = 10f;
    private float AttackDamage = 2f; // fixed
    private float AttackCooldown = 0.5f; //fixed
    private float DashDamage = 5f; // fixed
    private float DashCooldown = 5f; // fixed
    private float DashDistance = 0.5f; // fixed
    private int Keys = 1;
    private int Coins = 1;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float GetCurrentHealth() { return CurrentHealth; }
    public float GetMaxHealth() { return MaxHealth; }
    public float GetMovementSpeed() { return MovementSpeed; }
    public float GetDashSpeed() { return DashSpeed; }
    public float GetAttackDamage() { return AttackDamage; }
    public float GetDashDamage() { return DashDamage; }
    public float GetDashCooldown() { return DashCooldown; }
    public float GetAttackCooldown() { return AttackCooldown; }
    public float GetDashDistance() { return DashDistance; }
    public int GetKeys() { return Keys; } 
    public int GetCoins() { return Coins; }

    public void SetCurrentHealth(float NewHealth) { CurrentHealth = NewHealth; }
    public void SetMaxHealth(float NewMaxHealth) { MaxHealth = NewMaxHealth; }
    public void SetMovementSpeed(float NewMovementSpeed) { MovementSpeed = NewMovementSpeed; }
    public void SetDashSpeed(float NewDashSpeed) { DashSpeed = NewDashSpeed; }
    public void SetAttackDamage(float NewAttackDamage) { AttackDamage = NewAttackDamage; }
    public void SetDashDamage(float NewDashDamage) { DashDamage = NewDashDamage; }
    public void SetDashCooldown(float NewDashCooldown) { DashCooldown = NewDashCooldown; }
    public void SetAttackCooldown(float NewAttackCooldown) { AttackCooldown = NewAttackCooldown; }
    public void SetDashDistance(float NewDashDistance) { DashDistance = NewDashDistance; }
    public void SetKeys(int NewKeys) { Keys = NewKeys; }
    public void SetCoins(int NewCoins) { Coins = NewCoins; }
    
    public void UpdateCurrentHealth(float UpdateValue) { CurrentHealth += UpdateValue; }
    public void UpdateMaxHealth(float UpdateValue) { MaxHealth += UpdateValue; }
    public void UpdateMovementSpeed(float UpdateValue) { MovementSpeed += UpdateValue; }
    public void UpdateDashSpeed(float UpdateValue) { DashSpeed += UpdateValue; }
    public void UpdateAttackDamage(float UpdateValue) { AttackDamage += UpdateValue; }
    public void UpdateDashDamage(float UpdateValue) { DashDamage += UpdateValue; }
    public void UpdateDashCooldown(float UpdateValue) { DashCooldown += UpdateValue; }
    public void UpdateAttackCooldown(float UpdateValue) {AttackCooldown += UpdateValue; }
    public void UpdateDashDistance(float UpdateValue) { DashDistance += UpdateValue; }
    public void UpdateKeys(int UpdateValue) { Keys += UpdateValue; }
    public void UpdateCoins(int UpdateValue) { Coins += UpdateValue; }
}
