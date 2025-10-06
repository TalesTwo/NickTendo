using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIManager : Singleton<PlayerUIManager>
{
    private float _width;
    private float _healthWidth;

    [Header("UI Elements")] 
    public float widthPerUnitHealth = 25;
    public float healthBarHeight = 20;

    [SerializeField] 
    private RectTransform health;
    [SerializeField]
    private RectTransform healthBar;

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
}
