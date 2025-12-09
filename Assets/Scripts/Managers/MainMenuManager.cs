using System;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEditor.Rendering;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class MainMenuManager : MonoBehaviour
    {
        [SerializeField] private SceneField _initialGameScene;

        [Header("Buttons and other stuff")]
        [SerializeField] private Button _startButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _gameButton;
        [SerializeField] private GameObject _settingsMenu;
        [SerializeField] private GameObject _launcher;

        [Header("Pop-up stuff")]
        [SerializeField] private GameObject _popUP;
        [SerializeField] private Vector2 _topLeftBound;
        [SerializeField] private Vector2 _bottomRightBound;
        
        private bool _hasClickedButton;

        public void Start()
        {
            // Temporaryily start the main menu music here
            AudioManager.Instance.PlayTitleTrack(1f, false, 0.1f, false, 0.1f);
            var playerController = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.enabled = false;
            }
            _startButton.interactable = true;

            _settingsMenu.SetActive(false);
            _popUP.SetActive(false);
            _startButton.onClick.AddListener(StartGameButton);
            _settingsButton.onClick.AddListener(OpenSettings);
            _gameButton.onClick.AddListener(OpenLauncher);
            _hasClickedButton = false;
            if (GameStateManager.Instance.hasOpenedLauncher)
            {
                _launcher.SetActive(true);
            }
            else
            {
                _launcher.SetActive(false);
            }
        }

        private void StartGameButton()
        {
            AudioManager.Instance.PlayOverworldTrack(1f, true, 1f, true, 0.1f);
            PlayerStats.Instance.SetPlayerName("Player");
            if (_startButton.interactable)
            {
                _startButton.interactable = false;
                Invoke(nameof(ResetBool), 1.1f);
                StartGame();
            }
        }

        private void OpenSettings()
        {
            _settingsMenu.GetComponent<PauseMenuManager>().OpenPauseMenu();
        }
        private void StartGame()
        {
            var playerController = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.enabled = true;
            }
            SceneSwapManager.Instance.SwapScene(_initialGameScene, 1f, 3f);
        }

        private void OpenLauncher()
        {
            AudioManager.Instance.PlayUISelectSound(1, 0);
            if (_popUP.activeInHierarchy)
            {
                _popUP.SetActive(false);
            }
            if(!GameStateManager.Instance.hasOpenedLauncher)
            {
                GameStateManager.Instance.hasOpenedLauncher = true;
                _launcher.SetActive(true);
            }
        }

        private void ResetBool()
        {
            _hasClickedButton = false;
        }
        public void IconClick()
        {
            AudioManager.Instance.PlayUIInvalidClick(1, 0);
            if (!_popUP.activeInHierarchy && !GameStateManager.Instance.hasOpenedLauncher)
            {
                _popUP.SetActive(true);
            }
            _popUP.GetComponent<RectTransform>().localPosition = RandomLocation();
        }

        private Vector2 RandomLocation()
        {
            float _randX = UnityEngine.Random.Range(_topLeftBound.x, _bottomRightBound.x);
            float _randY = UnityEngine.Random.Range(_bottomRightBound.y, _topLeftBound.y);

            Vector2 _randLoc = new Vector2(_randX, _randY);
            return _randLoc;
        }
    }
}