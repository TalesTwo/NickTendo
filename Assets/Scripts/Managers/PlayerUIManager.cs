using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class PlayerUIManager : Singleton<PlayerUIManager>
{
    private float _width;
    private float _healthWidth;
    private Dictionary<string, float> BuffedStats;
    private bool _isHUDActive;

    [Header("UI Elements")] 
    public float widthPerUnitHealth = 25;
    public float healthBarHeight = 20;

    [SerializeField] 
    private RectTransform health;
    [SerializeField]
    private RectTransform healthBar;
    [SerializeField]
    private TextMeshProUGUI buffedStatText;

    private void Start()
    {

        BuffedStats = new Dictionary<string, float>();
        EventBroadcaster.PlayerStatsChanged += OnChangedStats;
        EventBroadcaster.PersonaChanged += HandlePersonaChanged;
        _isHUDActive = true;
        SetHealth();
        SetBuffedStats();
    }

    private void Update()
    {
        //SetHealth();
    }

    public void SetHealth()
    {
        _width = widthPerUnitHealth * PlayerStats.Instance.GetMaxHealth();
        _healthWidth = widthPerUnitHealth * PlayerStats.Instance.GetCurrentHealth();

        healthBar.sizeDelta = new Vector2(_width, healthBarHeight);
        health.sizeDelta = new Vector2(_healthWidth, healthBarHeight);
    }

    void OnChangedStats(PlayerStatsEnum BuffType, float BuffValue)
    {
        if (BuffType == PlayerStatsEnum.Max_Health || BuffType == PlayerStatsEnum.Current_Health)
        {
            SetHealth();
        }
        if (BuffedStats.ContainsKey(BuffType.ToString()))
        {
            BuffedStats[BuffType.ToString()] += BuffValue;
        }
        SetBuffedStatsText();
        DisplayBuffedStats();
    }

    void SetBuffedStats()
    {
        string[] BannedStats = { "Current_Health", "Max_Health", "Keys", "Coins"};
        
        foreach (int i in Enum.GetValues(typeof(PlayerStatsEnum)))
        {
            string name = Enum.GetName(typeof(PlayerStatsEnum), i);
            if(!BannedStats.Contains(name))
            {
                BuffedStats[name] = 0;
            }

            //DebugUtils.Log(i + ": " +Enum.GetName(typeof(PlayerStatsEnum), i));

        }

        

        DisplayBuffedStats();

    }

    void DisplayBuffedStats()
    {
        foreach (string name in BuffedStats.Keys)
        {
            DebugUtils.Log(name + ": " + BuffedStats[name]);
        }
    }

    void SetBuffedStatsText()
    {
        string textToSet ="";
        foreach (string name in BuffedStats.Keys)
        {
            if (BuffedStats[name] != 0)
            {
                textToSet += name + ": " + BuffedStats[name] + "\n";
            }
        }
        buffedStatText.text = textToSet;
    }

    public void ToggleHUD()
    {
        if(_isHUDActive)
        {
           gameObject.SetActive(false);
            _isHUDActive = false;
        }
        else if (!_isHUDActive)
        {
            gameObject.SetActive(true);
            _isHUDActive = true;
        }
    }

    void HandlePersonaChanged(Types.Persona P)
    {
        SetHealth();
    }
}
