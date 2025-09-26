using System;
using System.Collections;
using UnityEngine;

namespace Managers
{
    public class PlayerManager : Singleton<PlayerManager>
    {
        private GameObject player;

        [Header("Teleport Settings")]
        [SerializeField] private float fadeDelay = 0.15f; // delay between fade out and fade in

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
                StartCoroutine(FadeTeleportRoutine(newPosition, fadeDelay));
            }
            else
            {
                player.transform.position = newPosition;
            }
        }

        /// <summary>
        /// Handles fade out → teleport → fade in
        /// </summary>
        private IEnumerator FadeTeleportRoutine(Vector3 newPosition, float delay)
        {
            SceneFadeManager fadeManager = SceneFadeManager.Instance;

            // 1️⃣ Fade out
            fadeManager.StartFadeOut();

            // Wait for fade out to complete
            while (fadeManager.IsFadingOut)
                yield return null;

            // 2️⃣ Teleport player
            player.transform.position = newPosition;

            // 3️⃣ Optional pause before fading back in
            yield return new WaitForSeconds(delay);

            // 4️⃣ Fade back in
            fadeManager.StartFadeIn();
        }
    }
}