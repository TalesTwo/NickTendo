using System;
using System.Collections;
using UnityEngine;


namespace Managers
{
    public class LightingManager : Singleton<LightingManager>
    {

        // Create a reference to the global light source
        private UnityEngine.Rendering.Universal.Light2D globalLight;
        // create a reference to the light which will be attached to the player (spot light)
        private UnityEngine.Rendering.Universal.Light2D playerLight;

        private void Start()
        {
            EventBroadcaster.GameStarted += InitializeWorldLight;
            EventBroadcaster.GameRestart += InitializeWorldLight;
            EventBroadcaster.ObjectFellInPit += OnObjectFellInPit;
            playerLight = GameObject.Find("Player").GetComponent<UnityEngine.Rendering.Universal.Light2D>();
            DebugUtils.LogSuccess("[LightingManager] Initialized successfully.");
        }


        private void OnObjectFellInPit(GameObject obj, Vector3 pitCenter)
        {
            /*
             * When the player falls in a pit, we wanna "dim" the player light down to 0 over a variable duration
             */
            float dimDuration = 0.35f;
            float waitBeforeFadeIn = 0.05f; // wait a bit before restoring light (sync with respawn)
            float restoreDuration = 0.35f;
            DebugUtils.LogSuccess("1");
            if (playerLight == null) {return;}
            DebugUtils.LogSuccess("2");
            // Get the root object (since the hitbox is a child of the player)
            //GameObject parentObj = obj.transform.parent.gameObject;


            //if (parentObj == null){ return; }
            DebugUtils.LogSuccess("3");
                

            // Ensure it's the player, by checking the tag
            if (obj.CompareTag("Player"))
            {
                DebugUtils.LogSuccess("Dimming player light due to pit fall.");
                StartCoroutine(DimAndRestorePlayerLight(dimDuration, waitBeforeFadeIn, restoreDuration));
            }
        }

        
        private IEnumerator DimAndRestorePlayerLight(float dimDuration, float waitDelay, float restoreDuration)
        {
            float originalIntensity = playerLight.intensity;
            float elapsed = 0f;

            // --- Fade Out ---
            while (elapsed < dimDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / dimDuration);
                playerLight.intensity = Mathf.Lerp(originalIntensity, 0f, t);
                yield return null;
            }

            playerLight.intensity = 0f;

            // --- Optional pause before fade back ---
            yield return new WaitForSeconds(waitDelay);

            // --- Fade In ---
            elapsed = 0f;
            while (elapsed < restoreDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / restoreDuration);
                playerLight.intensity = Mathf.Lerp(0f, originalIntensity, t);
                yield return null;
            }

            playerLight.intensity = originalIntensity;
        }


        
        private void InitializeWorldLight()
        {
            // create an instance of the global light
            if (globalLight == null)
            {
                // create a new global light object from the prefab
                GameObject globalLightObj = new GameObject("Global Light");
                globalLight = globalLightObj.AddComponent<UnityEngine.Rendering.Universal.Light2D>();
                globalLight.lightType = UnityEngine.Rendering.Universal.Light2D.LightType.Global;
                globalLight.intensity = 0f;
            }

            
            if (playerLight == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    playerLight = player.GetComponentInChildren<UnityEngine.Rendering.Universal.Light2D>();
                    playerLight.intensity = 1.0f;
                }
                else
                {
                    Debug.LogWarning("No Player found in scene!");
                }
            }
            
            
        }
        
    }
}
