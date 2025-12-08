using Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour
{
    [Header("Menu Stuff")]
    [SerializeField]
    private bool _isPauseMenu = true;
    [SerializeField]
    private SceneField _mainMenuScene;

    [Header("Main Window")]
    [SerializeField]
    private Button _closeButton;
    [SerializeField]
    private Slider _sfxSlider;
    [SerializeField]
    private Slider _musicSlider;
    [SerializeField]
    private Button _continueButton;
    [SerializeField]
    private Button _mainMenuButton;

    [Header("Confirmation Window")]
    [SerializeField]
    private GameObject _window;
    [SerializeField]
    private Button _yesButton;
    [SerializeField]
    private Button _noButton;

    private bool _hasClickedButton;

    private void Start()
    {
        _closeButton.onClick.AddListener(ClosePauseMenu);
        _continueButton.onClick.AddListener(ClosePauseMenu);
        if (_isPauseMenu)
        {
            _window.SetActive(false);
            _mainMenuButton.onClick.AddListener(EnableConfirmationWindow);
            _yesButton.onClick.AddListener(ConfirmYes);
            _noButton.onClick.AddListener(ConfirmNo);
        }
        _hasClickedButton = false;
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Tab))
            {
                if (gameObject.activeInHierarchy)
                {
                    ClosePauseMenu();
                }
            }
        }
    }

    public void SfxSlider()
    {
        AudioManager.Instance.sfxValue = _sfxSlider.value;
        DebugUtils.Log("Current sfx value: " + _sfxSlider.value);
    }

    public void MusicSlider()
    {
        AudioManager.Instance.musicValue = _musicSlider.value;
        DebugUtils.Log("Current music value: " + _musicSlider.value);
    }

    public void OpenPauseMenu()
    {
        gameObject.SetActive(true);

        _sfxSlider.value = AudioManager.Instance.sfxValue;
        _musicSlider.value = AudioManager.Instance.musicValue;
        _hasClickedButton = false;

        DebugUtils.Log($"Current sfx: {AudioManager.Instance.sfxValue} and current music: {AudioManager.Instance.musicValue}");

        if (_isPauseMenu)
        {
            EventBroadcaster.Broadcast_StartStopAction();
            EventBroadcaster.Broadcast_GamePause();
            Time.timeScale = 0;
            Managers.AudioManager.Instance.PlayPauseMenuSound(1, 0);
        }
    }

    private void ClosePauseMenu()
    {
        gameObject.SetActive(false);
        if (_isPauseMenu)
        {
            EventBroadcaster.Broadcast_StartStopAction();
            EventBroadcaster.Broadcast_GameUnpause();
            Time.timeScale = 1;
            Managers.AudioManager.Instance.PlayPauseMenuSound(1, 0);
        }
    }

    private void EnableConfirmationWindow()
    {
        _window.SetActive(true);
    }

    private void ConfirmYes()
    {
        Time.timeScale = 1;
        EventBroadcaster.Broadcast_StartStopAction(); 
        EventBroadcaster.Broadcast_GameUnpause();
        if (!_hasClickedButton)
        {
            _hasClickedButton = true;
            SceneSwapManager.Instance.SwapScene(_mainMenuScene, 1, 3);
            // Handle resetting all stats
            EventBroadcaster.Broadcast_ReturnToMainMenu();
        }
        //Invoke(nameof(UnmuteStuff), 0.99f);
    }

    private void UnmuteStuff()
    {
        AudioManager.Instance.muteMusic = false;
        AudioManager.Instance.muteSFX = false;
    }
    private void ConfirmNo()
    {
        _window.SetActive(false);
    }

    public void UIHoverSound()
    {
        AudioManager.Instance.PlayUIHoverSound();
    }

    public void UISelectSound()
    {
        AudioManager.Instance.PlayUISelectSound();
    }
}