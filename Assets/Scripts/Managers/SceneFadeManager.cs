using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class SceneFadeManager : Singleton<SceneFadeManager>
    {
        [SerializeField] private Image _fadeImage;

        public bool IsFading { get; private set; }

        protected override void Awake()
        {
            if (_fadeImage != null)
            {
                _fadeImage.raycastTarget = false;
                _fadeImage.color = new Color(0, 0, 0, 0);
                _fadeImage.gameObject.SetActive(true);
            }
            Canvas canvas = _fadeImage.GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                canvas.sortingOrder = 9999;
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            }
        }

        public IEnumerator FadeOut(float duration)
        {
            if (_fadeImage == null)
            {
                DebugUtils.LogError("SceneFadeManager: _fadeImage not assigned!");
                yield break;
            }

            AudioManager.Instance.PlayFirstTransitionSound(10, 0.1f);

            _fadeImage.color = new Color(0, 0, 0, 0);
            yield return Fade(0f, 1f, duration);
            _fadeImage.color = new Color(0, 0, 0, 1f);
        }

        public IEnumerator FadeIn(float duration)
        {
            if (_fadeImage == null)
            {
                DebugUtils.LogError("SceneFadeManager: _fadeImage not assigned!");
                yield break;
            }

            AudioManager.Instance.PlaySecondTransitionSound(10, 0.1f);

            _fadeImage.color = new Color(0, 0, 0, 1f);
            yield return Fade(1f, 0f, duration);
            _fadeImage.color = new Color(0, 0, 0, 0f);
        }

        private IEnumerator Fade(float from, float to, float duration)
        {
            IsFading = true;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float alpha = Mathf.Lerp(from, to, elapsed / duration);
                _fadeImage.color = new Color(0, 0, 0, alpha);
                yield return null;
            }

            _fadeImage.color = new Color(0, 0, 0, to);
            IsFading = false;
        }
    }
}
