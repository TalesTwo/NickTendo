using Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerUIManager : Singleton<PlayerUIManager>
{
    [Header("UI Elements")] 
    public float widthPerUnitHealth = 25;
    public float healthBarHeight = 20;
    public float backgroundHeight = 29.1f;

    [SerializeField]
    private Canvas _mainCanvas;
    [SerializeField] 
    private RectTransform health;
    [SerializeField]
    private RectTransform healthBar;
    [SerializeField]
    private RectTransform healthBackground;
    [SerializeField]
    private Image _healthIcon;
    [SerializeField]
    private Image _enemyIcon;
    [SerializeField]
    private GameObject _enemyCounter;
    [SerializeField]
    private Slider _dashSlider;
    [SerializeField]
    private Image _dashSliderImage;
    [SerializeField]
    private Image _dashIcon;

    [Header("Arcade Text")]
    [SerializeField]
    private GameObject _arcadeText;
    [SerializeField]
    private float _xPos = 0;
    [SerializeField]
    private float _yPos = 360;
    [SerializeField]
    private float _xScale = 2;
    [SerializeField]
    private float _yScale = 2;
    [SerializeField]
    private float _duration = 1;

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
    private bool _isInDialogue;
    private int _currentEnemyCount;
    private int _updateEnemyCount;
    private RectTransform _arcadeTextRectTrans;
    private TextMeshProUGUI _arcadeTextMeshPro;

    private void Start()
    {
        EventBroadcaster.PersonaChanged += HandlePersonaChanged;
        EventBroadcaster.PlayerDamaged += HandlePlayerDamage;
        EventBroadcaster.PlayerDeath += HandlePlayerDeath;
        EventBroadcaster.PlayerOpenMenu += HandlePlayerOpenMenu;
        EventBroadcaster.PlayerCloseMenu += HandlePlayerCloseMenu;
        EventBroadcaster.GameUnpause += ResetPauseBool;
        EventBroadcaster.PlayerStatsChanged += OnPlayerStatChanged;
        EventBroadcaster.StartDialogue += StartDialogue;
        EventBroadcaster.StopDialogue += StopDialogue;

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
        _currentEnemyCount = 0;
        _updateEnemyCount = 0;

        SetHealth();
    }

    private void OnPlayerStatChanged(PlayerStatsEnum stat, float Value)
    {
        if (stat == PlayerStatsEnum.Max_Health || stat == PlayerStatsEnum.Current_Health)
        {
            SetHealth();
        }
        if (gameObject.activeInHierarchy)
        {
            ArcadeText(stat);
        }
    }

    void ArcadeText(PlayerStatsEnum stat)
    {        
        GameObject _arcadeTextInstance = Instantiate(_arcadeText);
        _arcadeTextInstance.transform.SetParent(_mainCanvas.transform);
        _arcadeTextRectTrans = _arcadeTextInstance.GetComponent<RectTransform>();
        _arcadeTextRectTrans.localPosition = new Vector2(_xPos, _yPos);
        _arcadeTextRectTrans.localScale = new Vector2(_xScale, _yScale);
        string _textToSet = "";
        if (stat == PlayerStatsEnum.Current_Health)
        {
            _textToSet = "Health get!!!";
        }
        else if (stat == PlayerStatsEnum.Chips)
        {
            _textToSet = "Chip get!!!";
        }
        _arcadeTextMeshPro = _arcadeTextInstance.GetComponent<TextMeshProUGUI>();
        _arcadeTextMeshPro.text = _textToSet;
        StartCoroutine(ArcadeTextEffect(_arcadeTextRectTrans, _arcadeTextMeshPro, _duration));
    }

    IEnumerator ArcadeTextEffect(RectTransform transform, TextMeshProUGUI textmesh, float duration)
    {
        float _effectTime = 0;
        float _newYPos = _yPos;
        Color _newColor = textmesh.color;
        while (_effectTime < duration)
        {
            _effectTime += Time.deltaTime;
            transform.localPosition = new Vector2 (_xPos, _newYPos);
            _newYPos += 0.25f;
            if (_newColor.a - 0.004f < 0)
            {
                _newColor.a = 0;
            }
            else
            {
                _newColor.a -= 0.004f;
            }
            textmesh.color = _newColor;
            yield return null;
        }
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

        _currentEnemyCount = DungeonController.Instance.GetNumberOfEnemiesInCurrentRoom();
        if (_currentEnemyCount > 0)
        {
            if (!_enemyCounter.activeInHierarchy)
            {
                _enemyCounter.SetActive(true);
                _updateEnemyCount = _currentEnemyCount;
            }
            _enemyCounter.GetComponentInChildren<TextMeshProUGUI>().SetText(_currentEnemyCount.ToString());
            
            if (_updateEnemyCount > _currentEnemyCount)
            {
                _updateEnemyCount = _currentEnemyCount;
                if (!_isPlayerInMenu)
                {
                    _enemyIcon.GetComponent<ScaleEffectsUI>().IncreaseSize();
                    _enemyIcon.GetComponent<ScaleEffectsUI>().DecreaseSize();
                }
            }
        }
        else if (_currentEnemyCount == 0 && _enemyCounter.activeInHierarchy) 
        { 
            _enemyCounter.SetActive(false);
        }

        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Tab))
            {
                if (!_isPlayerInMenu && !_isInPauseMenu)
                {
                    GameObject _pauseMenuInstace = Instantiate(_pauseMenu);
                    _pauseMenuInstace.GetComponent<PauseMenuManager>()?.OpenPauseMenu();
                    _isInPauseMenu = true;
                }
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

    void StartDialogue(string m)
    {
        _isPlayerInMenu = true;
    }

    void StopDialogue()
    {
        _isPlayerInMenu = false;
    }
 }