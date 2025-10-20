using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIManager : Singleton<PlayerUIManager>
{
    private float _width;
    private float _healthWidth;
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
    [SerializeField]
    private GameObject _enemyCounter;

    private void Start()
    {
        EventBroadcaster.PlayerStatsChanged += OnChangedStats;
        EventBroadcaster.PersonaChanged += HandlePersonaChanged;
        
        _isHUDActive = true;
        //SetHealth();
    }

    private void Update()
    {
        SetHealth();
        
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
        //SetHealth();
    }


}
