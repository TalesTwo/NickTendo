using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Managers
{
    public class MainMenuManager : MonoBehaviour
    {
        [SerializeField] private SceneField _initialGameScene;
        [SerializeField] private GameObject[] _objectsToHideWhenLoading;
        [SerializeField] private TMP_InputField _usernameInputField;
        
        private Button _loginButton;

        public void Start()
        {
            // Temporaryily start the main menu music here
            AudioManager.Instance.PlayTitleTrack(1f, false, 0.1f, false, 0.1f);
            var playerController = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.enabled = false;
            }
            _loginButton = GetComponentInChildren<Button>();
            if (_loginButton != null)
            {
                _loginButton.onClick.AddListener(LoginButtonClicked);
            }

        }

        private bool VerifyLoginName()
        {
            // read in the current login name
            string loginName = _usernameInputField.text;
            int maxLength = 20; // example maximum length
            // check for a maximum length
            if (loginName.Length > maxLength)
            {
                DebugUtils.LogWarning($"Login name exceeds maximum length of {maxLength} characters. We need to warn the player");
                return false;
            }


            // if we reach this point, the name is valid
            return true;
        }
        public void LoginButtonClicked()
        {
            if (_usernameInputField && !string.IsNullOrWhiteSpace(_usernameInputField.text) && VerifyLoginName())
            {
                // disable the button to prevent multiple clicks
                _loginButton.interactable = false;
                AudioManager.Instance.PlayUISelectSound();
                AudioManager.Instance.PlayOverworldTrack(1f, true, 1f, true, 0.1f);
                PlayerStats.Instance.SetPlayerName(_usernameInputField.text);
                StartGame();
            }
        }

        private void StartGame()
        {
            foreach (var obj in _objectsToHideWhenLoading)
            {
                if (obj != null)
                    obj.SetActive(false);
            }
            var playerController = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.enabled = true;
            }
            SceneSwapManager.Instance.SwapScene(_initialGameScene, 1f, 3f);
        }
    }
}