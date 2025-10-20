using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class StatDisplayUI : MonoBehaviour
{
    [Header("Stat Display Data")]
    [SerializeField]
    private PlayerStatsEnum[] _statTypeList;
    [SerializeField]
    private PlayerStatsEnum[] _bannedStats;


    [SerializeField]
    private GameObject _tooltip;
    private Dictionary<PlayerStatsEnum, float[]> _statDictionary;
   
    private void Start()
    {
        _statDictionary = new Dictionary<PlayerStatsEnum, float[]>();
        //_tooltip = GameObject.Find("Tooltip");
        //_tooltip.GetComponent<TooltipUI>().HideTooltip();

        SetStatDict();

        EventBroadcaster.PersonaChanged += SetBaseStats;
        EventBroadcaster.PlayerStatsChanged += SetBuffedStats;
    }

    private void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.L))
        {
            Test();
        }*/
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

    void SetBaseStats(Types.Persona P)
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
        }
    }

    void SetBuffedStats(PlayerStatsEnum _buffType, float _buffValue)
    {
        float _buffedStat = 0;
        if (_buffType == PlayerStatsEnum.Attack_Cooldown) { _buffedStat = PlayerStats.Instance.GetAttackCooldown(); }
        else if (_buffType == PlayerStatsEnum.Attack_Damage) { _buffedStat = PlayerStats.Instance.GetAttackDamage(); }
        else if (_buffType == PlayerStatsEnum.Dash_Cooldown) { _buffedStat = PlayerStats.Instance.GetDashCooldown(); }
        else if (_buffType == PlayerStatsEnum.Dash_Damage) { _buffedStat = PlayerStats.Instance.GetDashDistance(); }
        else if (_buffType == PlayerStatsEnum.Dash_Speed) { _buffedStat = PlayerStats.Instance.GetDashSpeed(); }
        else if (_buffType == PlayerStatsEnum.Movement_Speed) { _buffedStat = PlayerStats.Instance.GetMovementSpeed(); }

        _statDictionary[_buffType][1] = _buffedStat;
    }

    string Test()
    {
        string buffs = "";
        foreach (PlayerStatsEnum _buffType in _statDictionary.Keys)
        {
            buffs += _buffType + " has a base stat of " + _statDictionary[_buffType][0] + " and a buff of " + _statDictionary[_buffType][1] + "\n";
        }
        return buffs;
    }

    public void Enter(bool _hasEntered)
    {
        if(_hasEntered)
        {
            _tooltip.GetComponent<TooltipUI>().ShowTooltip(Test());
        }
        else
        {
            _tooltip.GetComponent<TooltipUI>().HideTooltip();
        }
    }
}
