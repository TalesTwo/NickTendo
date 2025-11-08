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
            DebugUtils.LogSuccess("[LightingManager] Initialized successfully.");
        }

        public void Update()
        {
            // TEMP FIX: 
            // every 1 second, check to update the global light based on enemy count in room
            // This is a temp fix for the issue where the global light does not update when enemies are killed via environmental hazards
            if (Time.frameCount % 60 == 0 && !_bTransitioning) // assuming 60 FPS, this is roughly every second
            {
                // get the current room the player is in
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player == null) { return; }

                Room currentRoom = DungeonController.Instance.GetCurrentRoom();
                if (currentRoom == null) { return; }
                int enemyCount = DungeonController.Instance.GetNumberOfEnemiesInRoom(currentRoom);
                if (enemyCount == 0)
                {
                    _bTransitioning = true;
                    SetGlobalLightIntensity(1.0f, globalLightTransitionDuration);
                    
                }
                else
                {
                    _bTransitioning = true;
                    SetGlobalLightIntensity(0f, playerLightTransitionDuration);
                }
            }
        }


        private void OnEnemyDeath(EnemyControllerBase enemy, Room room = null)
        {
            
            // get the number of enemies in the room
            int enemyCount = DungeonController.Instance.GetNumberOfEnemiesInRoom(room);
            // if the number if 0, set global light to max intensity
            if (enemyCount == 0)
            {
                SetGlobalLightIntensity(1.0f);
            }
            else
            {
                SetGlobalLightIntensity(0f);
            }
        }
        
        private void OnPlayerChangedRoom((int row, int col) targetRoomCoords)
        {
            // get access to the target room
            Room targetRoom = DungeonGeneratorManager.Instance.GetDungeonRooms()[targetRoomCoords.row][targetRoomCoords.col];
            // null check it for saftey
            if (targetRoom == null) { return; }
            // get the number of enemies in the room
            int enemyCount = DungeonController.Instance.GetNumberOfEnemiesInRoom(targetRoom);
            // if the number if 0, set global light to max intensity
            if (enemyCount == 0)
            {
                SetGlobalLightIntensity(1.0f);
            }
            else
            {
                SetGlobalLightIntensity(0f);
            }
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
