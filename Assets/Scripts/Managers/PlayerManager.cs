using System;
using System.Collections;
using UnityEngine;

namespace Managers
{
    public class PlayerManager : Singleton<PlayerManager>
    {
        private GameObject player;

        [Header("Teleport Settings")]
        [SerializeField] private float fadeDuration = 0.15f;
        [SerializeField] private float fadeDelay = 0.15f;

        void Start()
        {
            // Get reference to the player
            player = GameObject.FindWithTag("Player");
        }

        /// <summary>
        /// Teleports player immediately (optionally with screen fade)
        /// </summary>
        public void TeleportPlayer(Vector3 newPosition, bool bShouldUseScreenFade = true)
        {
            if (player == null)
            {
                DebugUtils.LogError("PlayerManager: Player not found!");
                return;
            }

            if (bShouldUseScreenFade)
            {
                StartCoroutine(FadeTeleportRoutine(newPosition, fadeDuration, fadeDelay));
            }
            else
            {
                player.transform.position = newPosition;
            }
        }

        /// <summary>
        /// Handles fade out → teleport → fade in using SceneFadeManager
        /// </summary>
        private IEnumerator FadeTeleportRoutine(Vector3 newPosition, float fadeDuration, float delay)
        {
            SceneFadeManager fadeManager = SceneFadeManager.Instance;
            
            yield return fadeManager.FadeOut(fadeDuration);
            
            player.transform.position = newPosition;
            
            if (delay > 0f)
                yield return new WaitForSeconds(delay);
            
            yield return fadeManager.FadeIn(fadeDuration);
        }
    }
}