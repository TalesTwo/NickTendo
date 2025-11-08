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
    [Header("UI Elements")] 
    public float widthPerUnitHealth = 25;
    public float healthBarHeight = 20;
    public float backgroundHeight = 29.1f;

    [SerializeField] 
    private RectTransform health;
    [SerializeField]
    private RectTransform healthBar;
    [SerializeField]
    private RectTransform healthBackground;
    [SerializeField]
    private Image _healthIcon;
    [SerializeField]
    private GameObject _enemyCounter;
    [SerializeField]
    private Slider _dashSlider;
    [SerializeField]
    private Image _dashSliderImage;
    [SerializeField]
    private Image _dashIcon;

    [Header("Pause Menu")]
    [SerializeField] private GameObject _pauseMenu;

    private float _width;
    private float _healthWidth;
    private float _backgroundWidth;
    private bool _isHUDActive;
    private GameObject _player;
    private PlayerController _pController;
    private bool _hasStartedSlider;
    private bool _isPlayerAlive;
    private bool _isPlayerInMenu;
    private bool _didSkipCR;
    private bool _isInPauseMenu;

    private void Start()
    {
        EventBroadcaster.PersonaChanged += HandlePersonaChanged;
        EventBroadcaster.PlayerDamaged += HandlePlayerDamage;
        EventBroadcaster.PlayerDeath += HandlePlayerDeath;
        EventBroadcaster.PlayerOpenMenu += HandlePlayerOpenMenu;
        EventBroadcaster.PlayerCloseMenu += HandlePlayerCloseMenu;
        EventBroadcaster.GameUnpause += ResetPauseBool;

        _enemyCounter.SetActive(false);
        _player = GameObject.FindGameObjectWithTag("Player");
        _pController = _player.GetComponent<PlayerController>();

        _isHUDActive = true;
        _dashSlider.value = 1;
        _dashSliderImage.color = Color.yellow;
        _hasStartedSlider = false;
        _isPlayerAlive = true;
        _isPlayerInMenu = false;
        _didSkipCR = false;
        _isInPauseMenu = false;

        SetHealth();
    }

    private void Update()
    {
        //SetHealth();

        if (_pController.IsDashing() && !_hasStartedSlider)
        {
            HandleDashSlider();
            _hasStartedSlider = true;         
            Invoke(nameof(ResetSlide), PlayerStats.Instance.GetDashCooldown());
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

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!_isPlayerInMenu && !_isInPauseMenu)
            {
                GameObject _pauseMenuInstace = Instantiate(_pauseMenu);
                _pauseMenuInstace.GetComponent<PauseMenuManager>()?.OpenPauseMenu();
                _isInPauseMenu = true;
            }
        }
    }

    public void SetHealth()
    {
        _width = widthPerUnitHealth * PlayerStats.Instance.GetMaxHealth();
        _backgroundWidth = widthPerUnitHealth * PlayerStats.Instance.GetMaxHealth();
        _healthWidth = widthPerUnitHealth * PlayerStats.Instance.GetCurrentHealth();

        healthBar.sizeDelta = new Vector2(_width, healthBarHeight);
        healthBackground.sizeDelta = new Vector2(_backgroundWidth, backgroundHeight);
        health.sizeDelta = new Vector2(_healthWidth, healthBarHeight);
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

    void HandleDashSlider()
    {
        _dashSlider.value = 0;
        _dashIcon.GetComponent<Image>().color = Color.gray;
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

    void ResetSlide()
    {
        _hasStartedSlider = false;
        _dashIcon.color = Color.white;
        if(!_isPlayerInMenu)
        {
            if (_didSkipCR)
            {
                _didSkipCR = false;
            }
            else
            {
                _dashIcon.GetComponent<ScaleEffectsUI>().IncreaseSize();
                _dashIcon.GetComponent<ScaleEffectsUI>().DecreaseSize();
                AudioManager.Instance.PlayKeyGetSound();
            }
        }
    }

    void HandlePlayerDamage()
    {
        SetHealth();
        if (_isPlayerAlive && !_isPlayerInMenu)
        {
            _healthIcon.GetComponent<ScaleEffectsUI>().IncreaseSize();
            _healthIcon.GetComponent<ScaleEffectsUI>().DecreaseSize();
        }        
    }

    void HandlePlayerDeath()
    {
        _isPlayerAlive = false;
    }

    void HandlePlayerOpenMenu()
    {
        _isPlayerInMenu = true;
        if (_hasStartedSlider && _dashSlider.value < 1)
        {
            _didSkipCR = true;
            _dashSlider.value = 1;
            _dashIcon.color = Color.white;
        }
        _dashIcon.GetComponent<ScaleEffectsUI>().ResetScale();
    }

    void HandlePlayerCloseMenu()
    {
        _isPlayerInMenu = false;
    }

    void ResetPauseBool()
    {
        _isInPauseMenu = false;
    }
}
