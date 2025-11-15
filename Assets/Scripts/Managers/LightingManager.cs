using System;
using System.Collections;
using UnityEngine;


namespace Managers
{
    
    public class LightingManager : Singleton<LightingManager>
    {
        
        [SerializeField] private float globalLightTransitionDuration = 0.5f;
        [SerializeField] private float playerLightTransitionDuration = 0.5f;
        private bool _bTransitioning = false;
        private bool _bFallingInPit = false;
        

        // Create a reference to the global light source
        private UnityEngine.Rendering.Universal.Light2D globalLight;
        // create a reference to the light which will be attached to the player (spot light)
        private UnityEngine.Rendering.Universal.Light2D playerLight;

        private void Start()
        {
            EventBroadcaster.GameStarted += InitializeWorldLight;
            EventBroadcaster.GameRestart += InitializeWorldLight;
            EventBroadcaster.PlayerChangedRoom += OnPlayerChangedRoom;
            EventBroadcaster.EnemyDeath += OnEnemyDeath;
            playerLight = GameObject.Find("Player").GetComponent<UnityEngine.Rendering.Universal.Light2D>();
            //InvokeRepeating(nameof(CheckLight), 0f, 0.15f); // run every few seconds. //TODO: set this up to only run when enemies die or player changes room. but i dont feel like it rn
        }

        
        private void CheckLight()
        {
            if (_bTransitioning || _bFallingInPit) return;

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null) return;

            Room currentRoom = DungeonController.Instance.GetCurrentRoom();
            if (currentRoom == null) return;

            int enemyCount = DungeonController.Instance.GetNumberOfEnemiesInRoom(currentRoom);
            _bTransitioning = true;

            if (enemyCount == 0 || currentRoom.GetRoomClassification() == Types.RoomClassification.Shop || currentRoom.GetRoomClassification() == Types.RoomClassification.Boss) {
                SetGlobalLightIntensity(1.0f, globalLightTransitionDuration); 
            } else {
                SetGlobalLightIntensity(0f, playerLightTransitionDuration);
            }
        }
        private void OnEnemyDeath(EnemyControllerBase enemy, Room room = null)
        {
            CheckLight();
            Invoke(nameof(CheckLight), 0.75f); // slight delay to ensure enemy count is updated
        }
        
        private void OnPlayerChangedRoom((int row, int col) targetRoomCoords)
        {
            CheckLight();
            Invoke(nameof(CheckLight), 0.75f); // slight delay to ensure enemy count is updated
        }
        
        public void SetGlobalLightIntensity(float intensity, float duration = 0f)
        {
            if (globalLight == null) return;

            if (duration > 0f)
            {
                StopCoroutine(nameof(LerpGlobalLightIntensity));
                StartCoroutine(LerpGlobalLightIntensity(globalLight.intensity, intensity, duration));
            }
            else
            {
                globalLight.intensity = intensity;
            }
        }
        
        private IEnumerator LerpGlobalLightIntensity(float from, float to, float duration)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                globalLight.intensity = Mathf.Lerp(from, to, elapsed / duration);
                yield return null;
            }
            globalLight.intensity = to;
            _bTransitioning = false;
        }
        
        public void SetPlayerLightIntensity(float intensity, float duration = 0f)
        {
            if (playerLight == null) return;

            if (duration > 0f)
            {
                StopCoroutine(nameof(LerpPlayerLightIntensity));
                StartCoroutine(LerpPlayerLightIntensity(playerLight.intensity, intensity, duration));
            }
            else
            {
                playerLight.intensity = intensity;
            }
        }
        
        private IEnumerator LerpPlayerLightIntensity(float from, float to, float duration)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                playerLight.intensity = Mathf.Lerp(from, to, elapsed / duration);
                yield return null;
            }
            playerLight.intensity = to;
            _bTransitioning = false;
        }

        public void PlayerFellInPit(GameObject obj)
        {
            /*
             * When the player falls in a pit, we wanna "dim" the player light down to 0 over a variable duration
             */
            float dimDuration = 0.35f;
            float waitBeforeFadeIn = 0.5f; // wait a bit before restoring light (sync with respawn)
            float restoreDuration = 0.35f;
            _bFallingInPit = true;
            if (playerLight == null) {return;}
            
            // Get the root object (since the hitbox is a child of the player)
            //GameObject parentObj = obj.transform.parent.gameObject;


            //if (parentObj == null){ return; }
                

            // Ensure it's the player, by checking the tag
            if (obj.CompareTag("Player"))
            {
                StartCoroutine(DimAndRestorePlayerLight(dimDuration, waitBeforeFadeIn, restoreDuration));
            }
        }


        
        private IEnumerator DimAndRestorePlayerLight(float dimDuration, float waitDelay, float restoreDuration)
        {
            float originalIntensity = playerLight.intensity;
            float originalGlobalIntensity = globalLight.intensity;
            float elapsed = 0f;

            // --- Fade Out ---
            while (elapsed < dimDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / dimDuration);
                playerLight.intensity = Mathf.Lerp(originalIntensity, 0f, t);
                globalLight.intensity = Mathf.Lerp(originalGlobalIntensity, 0f, t);
                yield return null;
            }

            playerLight.intensity = 0f;
            globalLight.intensity = 0f;

            // --- Optional pause before fade back ---
            yield return new WaitForSeconds(waitDelay);

            // --- Fade In ---
            elapsed = 0f;
            while (elapsed < restoreDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / restoreDuration);
                playerLight.intensity = Mathf.Lerp(0f, originalIntensity, t);
                globalLight.intensity = Mathf.Lerp(0f, originalGlobalIntensity, t);
                yield return null;
            }

            playerLight.intensity = originalIntensity;
            globalLight.intensity = originalGlobalIntensity;
            _bFallingInPit = false;
        }


        
        private void InitializeWorldLight()
        {
            // create an instance of the global light
            if (globalLight == null)
            {
                GameObject foundLight = GameObject.Find("Global Light");
                if (foundLight != null)
                {
                    globalLight = foundLight.GetComponent<UnityEngine.Rendering.Universal.Light2D>();
                    if (globalLight == null)
                    {
                        Debug.LogWarning("[LightingManager] 'Global Light' found but no Light2D component attached!");
                    }
                }
                else
                {
                    Debug.LogWarning("[LightingManager] No object named 'Global Light' found in scene!");
                }
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
