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

    private void Start()
    {
        _statDictionary = new Dictionary<PlayerStatsEnum, float[]>();

        SetStatDict();
        SetBaseStats();
        _statDisplayNumber = 0;

        EventBroadcaster.PersonaChanged += HandlePersonaChanged;
        EventBroadcaster.PlayerStatsChanged += HandleStatChanged;
        EventBroadcaster.PlayerDeath += HandleDeath;

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
            if (_buffType == PlayerStatsEnum.Attack_Cooldown) { _baseStat = PlayerStats.Instance.GetAttackCooldown(); }
            else if (_buffType == PlayerStatsEnum.Attack_Damage) { _baseStat = PlayerStats.Instance.GetAttackDamage(); }
            else if (_buffType == PlayerStatsEnum.Dash_Cooldown) { _baseStat = PlayerStats.Instance.GetDashCooldown(); }
            else if (_buffType == PlayerStatsEnum.Dash_Damage) { _baseStat = PlayerStats.Instance.GetDashDistance(); }
            else if (_buffType == PlayerStatsEnum.Dash_Speed) { _baseStat = PlayerStats.Instance.GetDashSpeed(); }
            else if (_buffType == PlayerStatsEnum.Movement_Speed) { _baseStat = PlayerStats.Instance.GetMovementSpeed(); }
            
            _statDictionary[_buffType][0] = _baseStat;
            _statDictionary[_buffType][1] = 0;
            gameObject.SetActive(false);
        }
    }

    void HandleStatChanged(PlayerStatsEnum _buffType, float _buffValue)
    {
        SetBuffedStats(_buffType, _buffValue);
        HandleDisplayText(_buffType, _buffValue);
    }

    void SetBuffedStats(PlayerStatsEnum _buffType, float _buffValue)
    {
        if (_statDictionary.ContainsKey(_buffType))
        {
            _statDictionary[_buffType][1] += _buffValue;
        }
    }

    void HandleDisplayText(PlayerStatsEnum _buffType, float _buffValue)
    {
        if (_statDictionary.ContainsKey(_buffType))
        {
            _statDisplayNumber++;
            gameObject.SetActive(true);
            _statDisplayText.SetText(_statDisplayNumber.ToString());
            _statDisplayTextBG.SetText(_statDisplayNumber.ToString());
        }        
    }

    string TooltipText()
    {
        string _buffText = "";
        foreach (PlayerStatsEnum _buffType in _statDictionary.Keys)
        {
            _buffText += AddSpace(_buffType) + "\n" +
                         "Base stat: " + _statDictionary[_buffType][0] + ", Buffed by: " + _statDictionary[_buffType][1] + "\n\n";
        }
        return _buffText;
    }

    string AddSpace(PlayerStatsEnum _nameInEnum)
    {
        string[] _splitName = _nameInEnum.ToString().Split('_');
        return String.Join(" ", _splitName);
    }

    public void Enter(bool _hasEntered)
    {
        if(_hasEntered)
        {
            _tooltip.GetComponent<TooltipUI>().ShowTooltip(TooltipText());
        }
        else
        {
            _tooltip.GetComponent<TooltipUI>().HideTooltip();
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
}