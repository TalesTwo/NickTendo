using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class SceneSwapManager : Singleton<SceneSwapManager>
    {
        private float _fadeInTime = 1f;

        private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
        private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

        public void SwapScene(SceneField newScene, float fadeOutTime = 1f, float fadeInTime = 1f)
        {
            _fadeInTime = fadeInTime;
            StartCoroutine(SwapSceneRoutine(newScene, fadeOutTime));
        }

        private IEnumerator SwapSceneRoutine(SceneField newScene, float fadeOutTime)
        {
            
            yield return SceneFadeManager.Instance.FadeOut(fadeOutTime);

            
            // Pause the load until fade out is complete
            yield return new WaitUntil(() => !SceneFadeManager.Instance.IsFading);

            
            // Storing in case we wanna try to track progress or do something with this later
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(newScene.SceneName, LoadSceneMode.Single);
            
            
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            DebugUtils.LogSuccess($"[SceneSwapManager] Scene loaded: {scene.name}");
            
            if (scene.name == "Gameplay")
            {
                EventBroadcaster.Broadcast_GameStarted();
            }
            
            StartCoroutine(SceneFadeManager.Instance.FadeIn(_fadeInTime));
        }
    }
}