using System;
using Managers;
using UnityEngine;

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
    private int _chips = 0;
    private int _coins = 0;
    private int _carryOverCoins = 0;
    private int _carryOverChips = 0;

    private PlayerController _player;
    
    /*
     * Carry over stats (these are permanent upgrades that persist between runs)
     */
    private float _carryOverMaxHealth = 0; public float GetCarryOverMaxHealth() { return _carryOverMaxHealth; } public void SetCarryOverMaxHealth(float value) { _carryOverMaxHealth = value; }
    public int numberOfCarryOverHealthUpgrades = 0;
    private float _carryOverMovementSpeed = 0; public float GetCarryOverMovementSpeed() { return _carryOverMovementSpeed; } public void SetCarryOverMovementSpeed(float value) { _carryOverMovementSpeed = value; }
    public int numberOfCarryOverMovementSpeedUpgrades = 0;
    private float _carryOverAttackDamage = 0; public float GetCarryOverAttackDamage() { return _carryOverAttackDamage; } public void SetCarryOverAttackDamage(float value) { _carryOverAttackDamage = value; }
    public int numberOfCarryOverAttackDamageUpgrades = 0;
    private float _carryOverAttackCooldown = 0; public float GetCarryOverAttackCooldown() { return _carryOverAttackCooldown; } public void SetCarryOverAttackCooldown(float value) { _carryOverAttackCooldown = value; }
    public int numberOfCarryOverAttackCooldownUpgrades = 0;
    private float _carryOverDashDamage = 0; public float GetCarryOverDashDamage() { return _carryOverDashDamage; } public void SetCarryOverDashDamage(float value) { _carryOverDashDamage = value; }
    public int numberOfCarryOverDashDamageUpgrades = 0;
    private float _carryOverDashCooldown = 0; public float GetCarryOverDashCooldown() { return _carryOverDashCooldown; } public void SetCarryOverDashCooldown(float value) { _carryOverDashCooldown = value; }
    public int numberOfCarryOverDashCooldownUpgrades = 0;
    private float _carryOverDashSpeed = 0; public float GetCarryOverDashSpeed() { return _carryOverDashSpeed; } public void SetCarryOverDashSpeed(float value) { _carryOverDashSpeed = value; }
    public int numberOfCarryOverDashSpeedUpgrades = 0;
    // This function will be used anytime the persona is selected, as it will apply the carry over stats to the current stats
    public void ApplyCarryOverStats()
    {
        UpdateCoins(_carryOverCoins);
        UpdateChips(_carryOverChips);
        UpdateMaxHealth(_carryOverMaxHealth);
        // we also need to update the currnt health to match the new max health
        UpdateCurrentHealth(_carryOverMaxHealth, true);
        UpdateMovementSpeed(_carryOverMovementSpeed);
        UpdateAttackDamage(_carryOverAttackDamage);
        //EventBroadcaster.Broadcast_PlayerStatsChanged(PlayerStatsEnum.Attack_Damage, _carryOverAttackDamage);
        UpdateAttackCooldown(-_carryOverAttackCooldown); // cooldown reduction
        UpdateDashDamage(_carryOverDashDamage);
        UpdateDashCooldown(-_carryOverDashCooldown); // cooldown reduction
        UpdateDashSpeed(_carryOverDashSpeed);
    }
    
    public void Start()
    {
        // if the player is already dead, we dont want to allow further health updates
        _player = PlayerManager.Instance.GetPlayer().GetComponent<PlayerController>();
    }
    
    
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
    public int GetChips() { return _chips; } 
    public int GetCoins() { return _coins; }
    public int GetCarryOverCoins() { return _carryOverCoins; }
    public int GetCarryOverChips() { return _carryOverChips; }

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
    public void SetChips(int NewChips) { _chips = NewChips; }
    public void SetCoins(int NewCoins) { _coins = NewCoins; }
    public void SetCarryOverCoins(int NewCarryOverCoins) { _carryOverCoins = NewCarryOverCoins; }
    public void SetCarryOverChips(int NewCarryOverChips) { _carryOverChips = NewCarryOverChips; }

    public void UpdateCurrentHealth(float UpdateValue, bool IgnoreEvents = false)
    {
        if (_player == null)
        {
            GameObject player = PlayerManager.Instance.GetPlayer();
            if (player != null)
            {
                _player = player.GetComponent<PlayerController>();
            }
            else
            {
                return;
            }
        }
        
        
        if (_player.GetIsDeadFlag()) {return;}
        
        _currentHealth += UpdateValue;
        //DebugUtils.Log("H: " + GetCurrentHealth() + " M: " + GetMaxHealth());
        if (!IgnoreEvents)
        {
            EventBroadcaster.Broadcast_PlayerDamaged();
        }
        
        if (_currentHealth > _maxHealth)
        {
            _currentHealth = _maxHealth;
        }
        else if (_currentHealth < 0)
        {
            _currentHealth = 0;
        }

        if (UpdateValue < 0 && _currentHealth != 0 && !IgnoreEvents) AudioManager.Instance.PlayPlayerDamagedSound();

        if (_currentHealth <= 0  && !IgnoreEvents)
        {
            AudioManager.Instance.PlayPlayerDeathSound(1, 0);
            EventBroadcaster.Broadcast_PlayerDeath();
        }
    }

    public void UpdateMaxHealth(float UpdateValue)
    {
        _maxHealth += UpdateValue;
    }
    public void UpdateMovementSpeed(float UpdateValue) { _movementSpeed += UpdateValue; }
    public void UpdateDashSpeed(float UpdateValue) { _dashSpeed += UpdateValue; }
    public void UpdateAttackDamage(float UpdateValue) { _attackDamage += UpdateValue; }
    public void UpdateDashDamage(float UpdateValue) { _dashDamage += UpdateValue; }
    public void UpdateDashCooldown(float UpdateValue) 
    { 
        _dashCooldown += UpdateValue; 
        if(_dashCooldown <= 0) { _dashCooldown = 0.1f; }
    }
    public void UpdateAttackCooldown(float UpdateValue) 
    {
        _attackCooldown += UpdateValue; 
        if(_attackCooldown <= 0) { _attackCooldown = 0.1f; }
    }
    public void UpdateDashDistance(float UpdateValue) { _dashDistance += UpdateValue; }
    public void UpdateChips(int UpdateValue) { _chips += UpdateValue; }
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
            "\nChips: " + _chips +
            "\nCoins: " + _coins
        );     
    }

    public void ApplyItemBuffs(PlayerStatsEnum BuffType, float BuffValue)
    {
        if (BuffType == PlayerStatsEnum.Max_Health)
        {
            Managers.AudioManager.Instance.PlayItemGetSound(1, 0);
            UpdateMaxHealth(BuffValue);
            UpdateCurrentHealth(BuffValue);
        }
        else if (BuffType == PlayerStatsEnum.Movement_Speed)
        {
            Managers.AudioManager.Instance.PlayItemGetSound(1, 0);
            UpdateMovementSpeed(BuffValue);
        }
        else if (BuffType == PlayerStatsEnum.Dash_Speed)
        {
            Managers.AudioManager.Instance.PlayItemGetSound(1, 0);
            UpdateDashSpeed(BuffValue);
        }
        else if (BuffType == PlayerStatsEnum.Attack_Damage)
        {
            Managers.AudioManager.Instance.PlayItemGetSound(1, 0);
            UpdateAttackDamage(BuffValue);
        }
        else if (BuffType == PlayerStatsEnum.Dash_Damage)
        {
            Managers.AudioManager.Instance.PlayItemGetSound(1, 0);
            UpdateDashDamage(BuffValue);
        }
        else if (BuffType == PlayerStatsEnum.Dash_Cooldown)
        {
            Managers.AudioManager.Instance.PlayItemGetSound(1, 0);
            UpdateDashCooldown(BuffValue);
        }
        else if (BuffType == PlayerStatsEnum.Attack_Cooldown)
        {
            Managers.AudioManager.Instance.PlayItemGetSound(1, 0);
            UpdateAttackCooldown(BuffValue);
        }
        else if (BuffType == PlayerStatsEnum.Dash_Distance)
        {
            Managers.AudioManager.Instance.PlayItemGetSound(1, 0);
            UpdateDashDistance(BuffValue);
        }
        else if (BuffType == PlayerStatsEnum.Current_Health)
        {
            float Heal = BuffValue;
            if (BuffValue + GetCurrentHealth() >= GetMaxHealth())
            {
                SetCurrentHealth(GetMaxHealth());
                Heal = 0;
            }
            UpdateCurrentHealth(Heal);
            Managers.AudioManager.Instance.PlayHealSound(1, 0);
        }
        else if (BuffType == PlayerStatsEnum.Chips)
        {
            AudioManager.Instance.PlayCoinGetSound(1f, 0f);
            UpdateChips((int)BuffValue);
            if (BuffValue < 0)
            {
                SetCarryOverChips(Mathf.Max(0, GetCarryOverChips() + (int)BuffValue));
            }

        }
        else if (BuffType == PlayerStatsEnum.Coins)
        {
            AudioManager.Instance.PlayCoinGetSound(1f, 0f);
            UpdateCoins((int)BuffValue);
            // edge case: if we ever "remove" coins, we want to also remove from carry_over coins
            if (BuffValue < 0)
            {
                SetCarryOverCoins(Mathf.Max(0, GetCarryOverCoins() + (int)BuffValue));
            }
        }
        // Now we get to the carry over stats
        // these have a bit more involved
        else if (BuffType == PlayerStatsEnum.CarryOver_Max_Health)
        {
            DebugUtils.Log("Applying Carry Over Max Health Buff: " + BuffValue);
            SetCarryOverMaxHealth(GetCarryOverMaxHealth() + BuffValue);
            // now we want to immediately apply this to the current stats as well
            UpdateMaxHealth(BuffValue);
            UpdateCurrentHealth(BuffValue, true);
            // we need to specifically tell the UI that we updated the max health
            // (as by default, we only update the CarryOver)
            numberOfCarryOverHealthUpgrades += 1;
            EventBroadcaster.Broadcast_PlayerStatsChanged(PlayerStatsEnum.Max_Health, BuffValue);
        }
        else if (BuffType == PlayerStatsEnum.CarryOver_Movement_Speed)
        {
            SetCarryOverMovementSpeed(GetCarryOverMovementSpeed() + BuffValue);
            UpdateMovementSpeed(BuffValue);
            numberOfCarryOverMovementSpeedUpgrades += 1;
            EventBroadcaster.Broadcast_PlayerStatsChanged(PlayerStatsEnum.Movement_Speed, BuffValue);
        }
        else if (BuffType == PlayerStatsEnum.CarryOver_Attack_Damage)
        {
            SetCarryOverAttackDamage(GetCarryOverAttackDamage() + BuffValue);
            UpdateAttackDamage(BuffValue);
            numberOfCarryOverAttackDamageUpgrades += 1;
            EventBroadcaster.Broadcast_PlayerStatsChanged(PlayerStatsEnum.Attack_Damage, BuffValue);
        }
        else if (BuffType == PlayerStatsEnum.CarryOver_Attack_Cooldown)
        {
            SetCarryOverAttackCooldown(GetCarryOverAttackCooldown() + BuffValue);
            UpdateAttackCooldown(BuffValue);
            numberOfCarryOverAttackCooldownUpgrades += 1;
            EventBroadcaster.Broadcast_PlayerStatsChanged(PlayerStatsEnum.Attack_Cooldown, BuffValue);
        }
        else if (BuffType == PlayerStatsEnum.CarryOver_Dash_Damage)
        {
            SetCarryOverDashDamage(GetCarryOverDashDamage() + BuffValue);
            UpdateDashDamage(BuffValue);
            numberOfCarryOverDashDamageUpgrades += 1;
            EventBroadcaster.Broadcast_PlayerStatsChanged(PlayerStatsEnum.Dash_Damage, BuffValue);
        }
        else if (BuffType == PlayerStatsEnum.CarryOver_Dash_Cooldown)
        {
            SetCarryOverDashCooldown(GetCarryOverDashCooldown() + BuffValue);
            UpdateDashCooldown(BuffValue);
            numberOfCarryOverDashCooldownUpgrades += 1;
            EventBroadcaster.Broadcast_PlayerStatsChanged(PlayerStatsEnum.Dash_Cooldown, BuffValue);
        }
        else if (BuffType == PlayerStatsEnum.CarryOver_Dash_Speed)
        {
            SetCarryOverDashSpeed(GetCarryOverDashSpeed() + BuffValue);
            UpdateDashSpeed(BuffValue);
            numberOfCarryOverDashSpeedUpgrades += 1;
            EventBroadcaster.Broadcast_PlayerStatsChanged(PlayerStatsEnum.Dash_Speed, BuffValue);
        }
        EventBroadcaster.Broadcast_PlayerStatsChanged(BuffType, BuffValue);
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
        SetChips(stats.Chips);
        SetCoins(stats.Coins);
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
    Chips,
    Coins,
    // Carry over stats
    CarryOver_Max_Health,
    CarryOver_Movement_Speed,
    CarryOver_Attack_Damage,
    CarryOver_Attack_Cooldown,
    CarryOver_Dash_Damage,
    CarryOver_Dash_Cooldown,
    CarryOver_Dash_Speed
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
    public int Chips;
    public int Coins;
    public Color PlayerColor;
    public string Description;
    public string Email;
    public string Username;
    public Types.Persona PersonaType;
}