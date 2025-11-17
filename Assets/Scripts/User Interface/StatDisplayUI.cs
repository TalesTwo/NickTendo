using Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;

public class StatDisplayUI : MonoBehaviour
{
    [Header("Stat Display Data")]
    [SerializeField]
    private PlayerStatsEnum[] _statTypeList;
    [SerializeField]
    private TextMeshProUGUI _statDisplayText;
    [SerializeField]
    private TextMeshProUGUI _statDisplayTextBG;
    [SerializeField]
    private GameObject _tooltip;

    private float _statDisplayNumber;
    private Dictionary<PlayerStatsEnum, float[]> _statDictionary;
    private bool _isViewing;
    private bool _isInMenu;

    private void Start()
    {
        _statDictionary = new Dictionary<PlayerStatsEnum, float[]>();

        SetStatDict();
        SetBaseStats();
        _statDisplayNumber = 0;
        _isViewing = false;
        _isInMenu = false;

        EventBroadcaster.PersonaChanged += HandlePersonaChanged;
        EventBroadcaster.PlayerStatsChanged += HandleStatChanged;
        EventBroadcaster.PlayerDeath += HandleDeath;
        EventBroadcaster.PlayerOpenMenu += OpenMenu;
        EventBroadcaster.PlayerCloseMenu += CloseMenu;

        gameObject.SetActive(false);
    }

    void SetStatDict()
    {
        foreach (PlayerStatsEnum _buffType in _statTypeList)
        {
            float[] _initStats = new float[2];
            _initStats[0] = 0;
            _initStats[1] = 0;
            _statDictionary[_buffType] = _initStats;
        }
    }

    void SetBaseStats()
    {
        foreach (PlayerStatsEnum _buffType in _statDictionary.Keys)
        {
            float _baseStat = 0;
            if (_buffType == PlayerStatsEnum.Attack_Cooldown) { _baseStat = PersonaStatsLoader.GetStats(Types.Persona.Normal).AttackCooldown;}
            else if (_buffType == PlayerStatsEnum.Attack_Damage) { _baseStat = PersonaStatsLoader.GetStats(Types.Persona.Normal).AttackDamage; }
            else if (_buffType == PlayerStatsEnum.Dash_Cooldown) { _baseStat = PersonaStatsLoader.GetStats(Types.Persona.Normal).DashCooldown; }
            else if (_buffType == PlayerStatsEnum.Dash_Damage) { _baseStat = PersonaStatsLoader.GetStats(Types.Persona.Normal).DashDamage; }
            else if (_buffType == PlayerStatsEnum.Dash_Speed) { _baseStat = PersonaStatsLoader.GetStats(Types.Persona.Normal).DashSpeed; }
            else if (_buffType == PlayerStatsEnum.Movement_Speed) { _baseStat = PersonaStatsLoader.GetStats(Types.Persona.Normal).MovementSpeed; }

            _statDictionary[_buffType][0] = _baseStat;
            _statDictionary[_buffType][1] = 0;
            gameObject.SetActive(false);
        }
    }

    void HandleStatChanged(PlayerStatsEnum _buffType, float _buffValue)
    {
        SetBuffedStats(_buffType, _buffValue);
        HandleDisplayText(_buffType, _buffValue);
        if (_isViewing)
        {
            _tooltip.GetComponent<TooltipUI>().ShowTooltip(TooltipText());
        }
    }

    void SetBuffedStats(PlayerStatsEnum _buffType, float _buffValue)
    {
        if (_statDictionary.ContainsKey(PlayerStatsEnum.Attack_Cooldown)) { _statDictionary[PlayerStatsEnum.Attack_Cooldown][1] = PlayerStats.Instance.GetAttackCooldown(); }
        if (_statDictionary.ContainsKey(PlayerStatsEnum.Attack_Damage)) _statDictionary[PlayerStatsEnum.Attack_Damage][1] = PlayerStats.Instance.GetAttackDamage();
        if (_statDictionary.ContainsKey(PlayerStatsEnum.Dash_Cooldown)) _statDictionary[PlayerStatsEnum.Dash_Cooldown][1] = PlayerStats.Instance.GetDashCooldown();
        if (_statDictionary.ContainsKey(PlayerStatsEnum.Dash_Damage)) _statDictionary[PlayerStatsEnum.Dash_Damage][1] = PlayerStats.Instance.GetDashDamage();
        if (_statDictionary.ContainsKey(PlayerStatsEnum.Dash_Speed)) _statDictionary[PlayerStatsEnum.Dash_Speed][1] = PlayerStats.Instance.GetDashSpeed();
        if (_statDictionary.ContainsKey(PlayerStatsEnum.Movement_Speed)) _statDictionary[PlayerStatsEnum.Movement_Speed][1] = PlayerStats.Instance.GetMovementSpeed();
    }

    void HandleDisplayText(PlayerStatsEnum _buffType, float _buffValue)
    {
        if (_statDictionary.ContainsKey(_buffType))
        {
            _statDisplayNumber++;
            gameObject.SetActive(true);
            _statDisplayText.SetText(_statDisplayNumber.ToString());
            _statDisplayTextBG.SetText(_statDisplayNumber.ToString());
            if (_statDisplayNumber > 1 && !_isInMenu)
            {
                gameObject.GetComponent<ScaleEffectsUI>().IncreaseSize();
                gameObject.GetComponent<ScaleEffectsUI>().DecreaseSize();
            }
        }        
    }

    string TooltipText()
    {
        string _buffText = "";
        float _buffPerc;
        foreach (PlayerStatsEnum _buffType in _statDictionary.Keys)
        {

            _buffText += AddSpace(_buffType) + ": ";

            if (IsCoolDown(_buffType))
            {
                //DebugUtils.Log(_buffType + ": " + _statDictionary[_buffType][1]);
                _buffText += _statDictionary[_buffType][1] + " sec\n";
            }
            else
            {
                //DebugUtils.Log(_buffType + "- BaseStat: " + _statDictionary[_buffType][0] + " BuffedStat: " + _statDictionary[_buffType][1]);
                _buffPerc = _statDictionary[_buffType][1] / _statDictionary[_buffType][0];
                _buffPerc *= 100;
                _buffPerc = Mathf.RoundToInt(_buffPerc);
                _buffText += _buffPerc + "%\n";
            }
        }
        _buffText += "\n";
        return _buffText;
    }

    private string AddSpace(PlayerStatsEnum _nameInEnum)
    {
        string[] _splitName = _nameInEnum.ToString().Split('_');
        return String.Join(" ", _splitName);
    }

    private bool IsCoolDown(PlayerStatsEnum _nameInEnum)
    {
        bool _returnBool = false;
        string[] _splitName = _nameInEnum.ToString().Split('_');
        if (_splitName[1] == "Cooldown")
        {
            _returnBool = true;
        }

        return _returnBool;
    }

    public void Enter(bool _hasEntered)
    {
        if(_hasEntered)
        {
            _tooltip.GetComponent<TooltipUI>().ShowTooltip(TooltipText());
            _isViewing = true;
        }
        else
        {
            _tooltip.GetComponent<TooltipUI>().HideTooltip();
            _isViewing = false;
        }
    }

    void HandlePersonaChanged(Types.Persona p)
    {
        SetStatDict();
        SetBaseStats();
        _statDisplayNumber = 0;
    }

    void HandleDeath()
    {
        SetStatDict();
        SetBaseStats();
        _statDisplayNumber = 0;
    }

    void OpenMenu()
    {
        _isInMenu = true;
    }

    void CloseMenu()
    {
        _isInMenu = false;
    }
}