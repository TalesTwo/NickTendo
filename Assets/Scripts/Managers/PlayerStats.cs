using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : Singleton<PlayerStats>
{
    // Start is called before the first frame update

    private float CurrentHealth = 1f;
    private float MaxHealth = 1f;
    private float MovementSpeed = 1f;
    private float AttackDamage = 1f;
    private float AttackSpeed = 1f;
    private float DashDistance = 1f;
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
    public float GetAttackDamage() { return AttackDamage; }
    public float GetAttackSpeed() { return AttackSpeed; }
    public float GetDashDistance() { return DashDistance; }
    public int GetKeys() { return Keys; } 
    public int GetCoins() { return Coins; }

    public void SetCurrentHealth(float NewHealth) { CurrentHealth = NewHealth; }
    public void SetMaxHealth(float NewMaxHealth) { MaxHealth = NewMaxHealth; }
    public void SetMovementSpeed(float NewMovementSpeed) { MovementSpeed = NewMovementSpeed; }
    public void SetAttackDamage(float NewAttackDamage) { AttackDamage = NewAttackDamage; }
    public void SetAttackSpeed(float NewAttackSpeed) { AttackSpeed = NewAttackSpeed; }  
    public void SetDashDistance(float NewDashDistance) { DashDistance = NewDashDistance; }
    public void SetKeys(int NewKeys) { Keys = NewKeys; }
    public void SetCoins(int NewCoins) { Coins = NewCoins; }
    
    public void UpdateCurrentHealth(float UpdateValue) { CurrentHealth += UpdateValue; }
    public void UpdateMaxHealth(float UpdateValue) { MaxHealth += UpdateValue; }
    public void UpdateMovementSpeed(float UpdateValue) { MovementSpeed += UpdateValue; }
    public void UpdateAttackDamage(float UpdateValue) { AttackDamage += UpdateValue; }
    public void UpdateAttackSpeed(float UpdateValue) { AttackSpeed += UpdateValue; }
    public void UpdateDashDistance(float UpdateValue) { DashDistance += UpdateValue; }
    public void UpdateKeys(int UpdateValue) { Keys += UpdateValue; }
    public void UpdateCoins(int UpdateValue) { Coins += UpdateValue; }
}
