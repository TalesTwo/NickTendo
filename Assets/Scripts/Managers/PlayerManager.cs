using System;
using System.Collections;
using UnityEngine;

namespace Managers
{
    public class PlayerManager : Singleton<PlayerManager>
    {
        private GameObject player;
        private PlayerController playerController;
        private AnimatedPlayer _playerAnimator;
        private GameObject camera;

        private bool _isFallingIntoPit = false;
        [Header("Teleport Settings")]
        [SerializeField] private float fadeDuration = 0.15f;
        [SerializeField] private float fadeDelay = 0.15f;

        void Start()
        {
            // Get reference to the player
            player = GameObject.FindWithTag("Player");
            playerController = player.GetComponent<PlayerController>();
            _playerAnimator = player.GetComponent<AnimatedPlayer>();
            camera = GameObject.FindWithTag("MainCamera");
            EventBroadcaster.PlayerDeath += PlayerDeath;
            EventBroadcaster.PersonaChanged += OnPersonaChanged;
            EventBroadcaster.GameRestart += ActivatePlayer;
            EventBroadcaster.ObjectFellInPit += OnObjectFellInPit;
            
        }


        public float GetNormalizedDistanceFromPlayer(Vector3 position, float maxDistance = 10f)
        {
            /*
             * Takes in a vector and a max distance, and computed a normalzized distance from the player
             */
            float distance = Vector3.Distance(player.transform.position, position);
            float normalizedDistance = Mathf.Clamp01(distance / maxDistance);
            return 1 - normalizedDistance;
        }
        
        private void OnObjectFellInPit(GameObject obj, Vector3 pitCenter)
        {
            // check to ensure the object is the player
            if (obj != player) { return; }
            
            /*
             * We also need to see if we are actively in the middle of a Dash,
             * since if we are, we do not want to fall into the pit
             */

            if (playerController.IsDashing()) { return; }
            
            // make sure we are not already falling into a pit
            if (_isFallingIntoPit) { return; }
            _isFallingIntoPit = true;

            
            
            /*
             * Goals for this are:
             * Shrink the player down to 0 scale over some time
             * Damage the player fpr some amount
             * if the player is dead, trigger death sequence
             * if the player is alive, respawn them at the last checkpoint (the door they entered the room from)
             */
            // change the player sprite to be the "falling" sprite (for now, we will use the dead sprite)
            _playerAnimator.SetHurting();
            Managers.AudioManager.Instance.PlayPitFallSound(1,0);
            LightingManager.Instance.PlayerFellInPit(obj);
            // add a 1f delay onto the fall to allow the sound to play
            StartCoroutine(HandlePlayerFellInPit(pitCenter));
            
        }
        
        private IEnumerator HandlePlayerFellInPit(Vector3 pitCenter)
        {
            
            
            const float shrinkDuration = 0.3f;
            float elapsed = 0f;

            // Cache starting values
            Vector3 startScale = player.transform.localScale;
            Vector3 startPosition = player.transform.position;

            // Optional: disable player input & physics during fall
            var rb = player.GetComponent<Rigidbody2D>();
            if (rb) rb.velocity = Vector2.zero;
            playerController.enabled = false;

            // Smoothly shrink and move toward pit center
            while (elapsed < shrinkDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / shrinkDuration);

                player.transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
                player.transform.position = Vector3.Lerp(startPosition, pitCenter, t);

                yield return null;
            }
            // add a slight delay before respawning
            yield return new WaitForSeconds(0.5f);
            playerController.enabled = true;

            // Finalize shrink
            player.transform.localScale = Vector3.zero;
            player.transform.position = pitCenter;

            // Apply damage or death logic
            // we want to only do this, if we ARE NOT in a tutorial room or in a shop
            Room currentRoom = DungeonController.Instance.GetCurrentRoom();
            
            if (DungeonController.Instance.IsPlayerInAnyTutorialRoom() == false && currentRoom.GetRoomClassification() != Types.RoomClassification.Shop)
            {
                PlayerStats.Instance.UpdateCurrentHealth(-1);
            }
            // if the player is not dead, respawn them
            if (PlayerStats.Instance.GetCurrentHealth() > 0)
            {
                RespawnPlayerInCurrentRoom();
            }
        }
        

        public GameObject GetPlayer()
        {
            return player;
        }
        private void RespawnPlayerInCurrentRoom()
        {
            
            Room currentRoom = DungeonController.Instance.GetCurrentRoom();
            // Find the nearest door to the player's current position
            var Doors = currentRoom.GetComponentsInChildren<Door>();
            Door nearestDoor = null;
            float nearestDistance = float.MaxValue;
            foreach (var door in Doors)
            {
                float distance = Vector3.Distance(player.transform.position, door.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestDoor = door;
                }
            }
            if (nearestDoor != null)
            {
                DoorTriggerInteraction doorTrigger = nearestDoor.GetComponent<DoorTriggerInteraction>();
                TeleportPlayer(doorTrigger.transform.Find("Spawn_Location").position, false);
                // reset player scale
                player.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
                _isFallingIntoPit = false;
            }
            
            // reset the player orientation, incase we fell with rotation, or some input
            playerController.ResetFacingDirection(true);
            

            
        }
        public void PlayerDeath()
        {
            Debug.Log("Player Death");
            playerController.SetIsDead();
            GameStateManager.Instance.PlayerDeath();
            ScreenUIActivator.Instance.SetDeathScreen();
            playerController.ResetFacingDirection(true);
            LightingManager.Instance.SetPlayerLightIntensity(1f);

        }
        
        public void DeactivatePlayer()
        {
            player.SetActive(false);
        }
        
        public void ActivatePlayer()
        {
            player.SetActive(true);
        }
        
        private void OnPersonaChanged(Types.Persona newPersona)
        {
            // get the stats of the new persona
            PlayerStatsStruct stats = PersonaStatsLoader.GetStats(newPersona);
            // get the color of the new persona
            Color color = stats.PlayerColor;
            // set the player color to the new persona color
            // update the player sprite color to the new persona color
            player.GetComponent<SpriteRenderer>().color = color;
        }

        // handles player resurection
        public void PlayerAlive()
        {
            playerController.ResetIsDead();
            PlayerStats.Instance.SetCurrentHealth(PlayerStats.Instance.GetMaxHealth());
            // reset anything else that might have gotten changed, like player scale
            player.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
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
            camera.transform.position = new Vector3(newPosition.x, newPosition.y, -1);
            
            if (delay > 0f)
                yield return new WaitForSeconds(delay);
            
            yield return fadeManager.FadeIn(fadeDuration);
        }
    }
}