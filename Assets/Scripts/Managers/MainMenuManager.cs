using System.Collections;
using System.Collections.Generic;
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
        [SerializeField] private Image _loadingBarImage;
        [SerializeField] private GameObject[] _objectsToHideWhenLoading; // e.g. buttons, text, etc.
    
        private List<AsyncOperation> _scenesToLoad = new List<AsyncOperation>();

        public void StartGame()
        {
            // Hide button and text
            HideMenu();
        
            _loadingBarImage.gameObject.SetActive(true);
        
            // start loading the game the scenes we need
            _scenesToLoad.Add(SceneManager.LoadSceneAsync(_initialGameScene));
        
        
            StartCoroutine(ProgressLoadingBar());
        
            // we can enable our player here
            //TODO: Improve this to wait until the scene is loaded
        
        }

        private void HideMenu()
        {
            for (int i = 0; i < _objectsToHideWhenLoading.Length; i++)
            {
                _objectsToHideWhenLoading[i].SetActive(false);
            }
        }

        private IEnumerator ProgressLoadingBar()
        {
            float loadProgress = 0f;

            for (int i = 0; i < _scenesToLoad.Count; i++)
            {
                AsyncOperation op = _scenesToLoad[i];
                op.allowSceneActivation = false; // hold back final activation

                // Load until 90%
                while (op.progress < 0.9f)
                {
                    loadProgress = Mathf.Clamp01(op.progress / 0.9f);
                    _loadingBarImage.fillAmount = loadProgress;
                    yield return null;
                }

                // Scene is ready but paused at 90% → bar is near full
                float startFill = _loadingBarImage.fillAmount;
                float elapsed = 0f;
                float minTime = 0.5f; // enforce at least 0.5s visible loading

                while (elapsed < minTime)
                {
                    elapsed += Time.deltaTime;
                    float t = Mathf.Clamp01(elapsed / minTime);

                    // Smoothly interpolate from current to 100%
                    _loadingBarImage.fillAmount = Mathf.Lerp(startFill, 1f, t);
                    yield return null;
                }

                // Pause briefly so the player sees "full bar"
                yield return new WaitForSeconds(0.5f);

                // Allow Unity to activate the scene
                op.allowSceneActivation = true;

                // Wait for activation to finish
                while (!op.isDone)
                    yield return null;
            }
        }


    }
}
