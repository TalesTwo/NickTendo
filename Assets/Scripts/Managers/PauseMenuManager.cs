using Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour
{
    [Header("Scenes")]
    [SerializeField]
    private SceneField _mainMenuScene;

    [Header("Main Window")]
    [SerializeField]
    private Button _closeButton;
    [SerializeField]
    private Button _sfxToggle;
    [SerializeField]
    private Button _musicToggle;
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

    private void Start()
    {
        _window.SetActive(false);

        _closeButton.onClick.AddListener(ClosePauseMenu);
        _sfxToggle.onClick.AddListener(ToggleSFX);
        _musicToggle.onClick.AddListener(ToggleMusic);  
        _continueButton.onClick.AddListener(ClosePauseMenu);
        _mainMenuButton.onClick.AddListener(EnableConfirmationWindow);
        _yesButton.onClick.AddListener(ConfirmYes);
        _noButton.onClick.AddListener(ConfirmNo);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameObject.activeInHierarchy)
            {
                ClosePauseMenu();
            }
        }
    }

    public void OpenPauseMenu()
    {
        EventBroadcaster.Broadcast_StartStopAction();
        EventBroadcaster.Broadcast_GamePause();
        gameObject.SetActive(true);

        _sfxToggle.GetComponent<ToggleButtonUI>().SetIsToggledOn(!AudioManager.Instance.muteSFX);
        _musicToggle.GetComponent<ToggleButtonUI>().SetIsToggledOn(!AudioManager.Instance.muteMusic);
        Time.timeScale = 0;
    }

    private void ClosePauseMenu()
    {
        EventBroadcaster.Broadcast_StartStopAction();
        EventBroadcaster.Broadcast_GameUnpause();
        gameObject.SetActive(false);
        Time.timeScale = 1;
    }

    private void ToggleSFX()
    {
        if (_sfxToggle.GetComponent<ToggleButtonUI>().GetIsToggledOn())
        {
            AudioManager.Instance.muteSFX = true;
        }
        else
        {
            AudioManager.Instance.muteSFX = false;
        }
    }

    private void ToggleMusic()
    {
        if (_musicToggle.GetComponent<ToggleButtonUI>().GetIsToggledOn())
        {
            AudioManager.Instance.muteMusic = true;
        }
        else
        {
            AudioManager.Instance.muteMusic = false;
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
        SceneSwapManager.Instance.SwapScene(_mainMenuScene, 1, 3);
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