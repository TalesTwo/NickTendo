using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Managers
{
    public class MainMenuManager : MonoBehaviour
    {
        [Header("Scene References")]
        [SerializeField] private SceneField _initialGameScene;
    
        [Header("UI References")]
        [SerializeField] private GameObject[] _objectsToHideWhenLoading; // e.g. buttons, text, etc.
        [SerializeField] private TMP_InputField _usernameInputField;
    
        private List<AsyncOperation> _scenesToLoad = new List<AsyncOperation>();

        public void StartGame()
        {
            
            // start loading the game the scenes we need
            _scenesToLoad.Add(SceneManager.LoadSceneAsync(_initialGameScene));
            // the flow we will follow is:
            // Flowly fade out the main menu
            // Load the _initialGameScene
            // call the GameStarted event
            // Fade in from black
            StartCoroutine(LoadGameCoroutine());
            
        }
        
        public void LoginButtonClicked()
        {
            // ensure we have a valid username before proceeding
            if (_usernameInputField && _usernameInputField.text != "")
            {
                PlayerStats.Instance.SetPlayerName(_usernameInputField.text);
                StartGame();
            }
            
        }
        
        private IEnumerator LoadGameCoroutine()
        {
            SceneFadeManager.Instance.StartFadeOut();
            
            foreach (var obj in _objectsToHideWhenLoading)
            {
                if (obj != null)
                    obj.SetActive(false);
            }

            if (_initialGameScene != null)
                SceneManager.LoadSceneAsync(_initialGameScene, LoadSceneMode.Single);


            EventBroadcaster.Broadcast_GameStarted();
            yield return new WaitUntil(() => !SceneFadeManager.Instance.IsFadingOut);

            SceneFadeManager.Instance.StartFadeIn();
            yield return null;
        }
        



        
    }
}
