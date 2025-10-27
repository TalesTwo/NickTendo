using System;
using System.Collections;
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
        
        // variables for the error message
        [SerializeField] private Text _errorMessageText;
        [SerializeField] private Text _errorMessageTextShadow;
        
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
            // hide the error message at start
            if (_errorMessageText != null && _errorMessageTextShadow != null)
            {
                _errorMessageText.gameObject.SetActive(false);
                _errorMessageTextShadow.gameObject.SetActive(false);
            }
            
        }

        private IEnumerator ShowErrorMessage(string message, float displayTime)
        {
            if (_errorMessageText != null && _errorMessageTextShadow != null)
            {
                _errorMessageText.text = message;
                _errorMessageTextShadow.text = message;
                _errorMessageText.gameObject.SetActive(true);
                _errorMessageTextShadow.gameObject.SetActive(true);
                yield return new WaitForSeconds(displayTime);
                _errorMessageText.gameObject.SetActive(false);
                _errorMessageTextShadow.gameObject.SetActive(false);
            }
        }
        private bool VerifyLoginName()
        {
            // read in the current login name
            string loginName = _usernameInputField.text;
            int maxLength = 15; // example maximum length
            // check for a maximum length
            if (loginName.Length > maxLength)
            {
                StartCoroutine(ShowErrorMessage("Error: Name too long!", 3f));
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
                // hide the error message if it was showing
                _errorMessageText.gameObject.SetActive(false);
                _errorMessageTextShadow.gameObject.SetActive(false);
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