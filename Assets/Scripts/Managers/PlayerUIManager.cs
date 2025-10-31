using Managers;
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
    [SerializeField]
    private Slider _dashSlider;
    [SerializeField]
    private Image _dashSliderImage;
    [SerializeField]
    private Image _dashIcon;

    private GameObject _player;
    private PlayerController _pController;
    private bool _hasStartedSlider;

    private void Start()
    {
        EventBroadcaster.PlayerStatsChanged += OnChangedStats;
        EventBroadcaster.PersonaChanged += HandlePersonaChanged;
        _enemyCounter.SetActive(false);
        _player = GameObject.FindGameObjectWithTag("Player");
        _pController = _player.GetComponent<PlayerController>();
        _isHUDActive = true;
        _dashSlider.value = 1;
        _dashSliderImage.color = Color.yellow;
        _hasStartedSlider = false;
        //SetHealth();
    }

    private void Update()
    {
        SetHealth();

        if (_pController.IsDashing() && !_hasStartedSlider)
        {
            HandleDashSlider();
            _hasStartedSlider = true;         
            Invoke(nameof(ResetSlideBool), PlayerStats.Instance.GetDashCooldown());
        }

        if (DungeonController.Instance.GetNumberOfEnemiesInCurrentRoom() > 0)
        {
            _enemyCounter.SetActive(true);
            _enemyCounter.GetComponentInChildren<TextMeshProUGUI>().SetText(DungeonController.Instance.GetNumberOfEnemiesInCurrentRoom().ToString());
        }
        else 
        { 
            _enemyCounter.SetActive(false); 
        }
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

    void HandleDashSlider()
    {
        _dashSlider.value = 0;
        _dashIcon.GetComponent<ScaleEffectsUI>().IncreaseSize();
        _dashIcon.GetComponent<ScaleEffectsUI>().DecreaseSize();
        StartCoroutine(FillSlider(PlayerStats.Instance.GetDashCooldown()));
    }

    IEnumerator FillSlider(float _cooldown)
    {
        
        float _fillTime = 0;
        while (_fillTime < _cooldown)
        {
            _fillTime += Time.deltaTime;
            float _lerpValue = _fillTime / _cooldown;
            _dashSlider.value = Mathf.Lerp(0, 1, _lerpValue);
            yield return null;
        }
    }

    void ResetSlideBool()
    {
        _hasStartedSlider = false;
        _dashIcon.GetComponent<ScaleEffectsUI>().IncreaseSize();
        _dashIcon.GetComponent<ScaleEffectsUI>().DecreaseSize();
        AudioManager.Instance.PlayKeyGetSound();
    }
}
