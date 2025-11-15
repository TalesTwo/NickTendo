using UnityEngine;
using UnityEngine.UI;

namespace System
{
    /// <summary>
    /// A static utility class for broadcasting and handling activity-related events.
    /// Provides functionality for notifying subscribers when an activity starts or completes.
    /// Subscribers can listen for these events and respond accordingly.
    /// 
    /// Template for how to set up a new event
    /// 
    ///  public delegate void EventNameHandler(ParameterType parameter);
    ///  public static event EventNameHandler EventName;
    ///  public static void Broadcast_EventName(ParameterType parameter) { EventName?.Invoke(parameter); }
    /// 
    /// 
    ///  in the class that is going to SUBSCRIBE to the event, do the following:
    ///  public void start(){
    ///     EventBroadcaster.EventName += YourClassMethod;
    ///  }
    /// 
    ///  in the class that is going to BROADCAST the event, do the following:
    ///  EventBroadcaster.Broadcast_EventName(parameter);
    /// 
    ///
    /// Created by: MoonTales
    /// </summary>
    public static class EventBroadcaster
    {
        
        /* Template for how to setup a new event
         *
         * public delegate void EventNameHandler(ParameterType parameter);
         * public static event EventNameHandler EventName;
         * public static void Broadcast_EventName(ParameterType parameter) { EventName?.Invoke(parameter); }
         *
         *
         * in the class that is going to subscribe to the event, do the following:
         * public void start(){
         *    EventBroadcaster.EventName += YourClassMethod;
         * }
         *
         * in the class that is going to broadcast the event, do the following:
         * EventBroadcaster.Broadcast_EventName(parameter);
         */
        
        // These are just for show, they will be removed soon
        
        //-------------------------------- Activity Events --------------------------------//
        
        /* Define the delegate for the ActivityStarted event */
        public delegate void PlayerDamagedHandler();
        public static event PlayerDamagedHandler PlayerDamaged;
        public static void Broadcast_PlayerDamaged() { PlayerDamaged?.Invoke(); }
        
        // Start Dialogue Broadcaster
        public delegate void StartDialogueHandler(string name);
        public static event StartDialogueHandler StartDialogue;
        public static void Broadcast_StartDialogue(string name) { StartDialogue?.Invoke(name); }
        
        // end dialogue Broadcaster
        public delegate void StopDialogueHandler();
        public static event StopDialogueHandler StopDialogue;
        public static void Broadcast_StopDialogue() { StopDialogue?.Invoke(); }

        // start or stop player actions
        public delegate void StartStopActionHandler();
        public static event StartStopActionHandler StartStopAction;
        public static void Broadcast_StartStopAction() { StartStopAction?.Invoke(); }
        
        // Broadcast to set a seed across the game
        public delegate void SetSeedHandler(int seed);
        public static event SetSeedHandler SetSeed;
        public static void Broadcast_SetSeed(int seed) { SetSeed?.Invoke(seed); }
        
        // Broadcast to change the persona of the player
        public delegate void PersonaChangedHandler(Types.Persona newPersona);
        public static event PersonaChangedHandler PersonaChanged;
        public static void Broadcast_PersonaChanged(Types.Persona newPersona) { PersonaChanged?.Invoke(newPersona); }
        
        // Broadcast for when the game is loaded / started
        public delegate void GameStartedHandler();
        public static event GameStartedHandler GameStarted;
        public static void Broadcast_GameStarted() { GameStarted?.Invoke(); }
        
        // Broadcast for when the player dies
        public delegate void PlayerDeathHandler();
        public static event PlayerDeathHandler PlayerDeath;
        public static void Broadcast_PlayerDeath() { PlayerDeath?.Invoke(); }

        // Broadcast for when the player changes rooms
        public delegate void PlayerChangedRoomHandler((int row, int col) targetRoomCoords);
        public static event PlayerChangedRoomHandler PlayerChangedRoom;
        public static void Broadcast_PlayerChangedRoom((int row, int col) targetRoomCoords) { PlayerChangedRoom?.Invoke(targetRoomCoords); }

        // Broadcast for when a player stat changes
        public delegate void PlayerStatsChangedHandler(PlayerStatsEnum buffType, float buffValue);
        public static event PlayerStatsChangedHandler PlayerStatsChanged;
        public static void Broadcast_PlayerStatsChanged(PlayerStatsEnum buffType, float buffValue) { PlayerStatsChanged?.Invoke(buffType, buffValue); }
        
        
        // Broadcast for when an enemy dies
        public delegate void EnemyDeathHandler(EnemyControllerBase enemy, Room room = null);
        public static event EnemyDeathHandler EnemyDeath;
        public static void Broadcast_EnemyDeath(EnemyControllerBase enemy, Room room = null) { EnemyDeath?.Invoke(enemy, room); }
        
        // broadcast for closing the persona UI
        public delegate void ClosePersonaUIHandler();
        public static event ClosePersonaUIHandler ClosePersonaUI;
        public static void Broadcast_ClosePersonaUI() { ClosePersonaUI?.Invoke(); }
        
        // boradcast for opening the persona UI
        public delegate void OpenPersonaUIHandler();
        public static event OpenPersonaUIHandler OpenPersonaUI;
        public static void Broadcast_OpenPersonaUI() { OpenPersonaUI?.Invoke(); }
        
        // Broadcast for when the game restarts
        public delegate void GameRestartHandler();
        public static event GameRestartHandler GameRestart;
        public static void Broadcast_GameRestart() { GameRestart?.Invoke(); }
        

        // Broadcast when something collides with a Pit
        public delegate void ObjectFellInPitHandler(GameObject obj, Vector3 pitCenter);
        public static event ObjectFellInPitHandler ObjectFellInPit;
        public static void Broadcast_ObjectFellInPit(GameObject obj, Vector3 pitCenter) { ObjectFellInPit?.Invoke(obj, pitCenter); }
        
        // Broadcast for when the player finishes dashing
        public delegate void PlayerFinishedDashingHandler(GameObject obj);
        public static event PlayerFinishedDashingHandler PlayerFinishedDashing;
       
        public static void Broadcast_PlayerFinishedDashing() { PlayerFinishedDashing?.Invoke(GameObject.FindWithTag("Player")); }

        public delegate void PlayerOpenMenuHandler();
        public static event PlayerOpenMenuHandler PlayerOpenMenu;
        public static void Broadcast_PlayerOpenMenu() { PlayerOpenMenu?.Invoke(); }

        public delegate void PlayerCloseMenuHandler();
        public static event PlayerCloseMenuHandler PlayerCloseMenu;
        public static void Broadcast_PlayerCloseMenu() { PlayerCloseMenu?.Invoke(); }

        public delegate void GamePauseHandler();
        public static event GamePauseHandler GamePause;
        public static void Broadcast_GamePause() { GamePause?.Invoke(); }

        public delegate void GameUnpauseHandler();
        public static event GameUnpauseHandler GameUnpause;
        public static void Broadcast_GameUnpause() { GameUnpause?.Invoke(); }
        
        // Delegate to define when the player enters a Shop room (changes in or out of it)
        public delegate void PlayerEnteredShopRoomHandler(bool isInShop);
        public static event PlayerEnteredShopRoomHandler PlayerEnteredShopRoom;
        public static void Broadcast_PlayerEnteredShopRoom(bool isInShop) { PlayerEnteredShopRoom?.Invoke(isInShop); }
        
        // Delegate to define when the player enters the Boss room (changes in or out of it)
        public delegate void PlayerEnteredBossRoomHandler(bool isInBossRoom);

        public static event PlayerEnteredBossRoomHandler PlayerEnteredBossRoom;
        public static void Broadcast_PlayerEnteredBossRoom(bool isInBossRoom) { PlayerEnteredBossRoom?.Invoke(isInBossRoom); }

        public delegate void EndTutorialHandler();
        public static event EndTutorialHandler EndTutorial;
        public static void Broadcast_EndTutorial() { EndTutorial?.Invoke(); }
        
        // broadcaster for when the boss fight will start
        public delegate void StartBossFightHandler();
        public static event StartBossFightHandler StartBossFight;
        public static void Broadcast_StartBossFight() { StartBossFight?.Invoke(); }
        
        // Delegate to "freeze" or "unfreeze" the game world (primarily used for enemies to pause movement without setting timescale to 0)
        public delegate void SetWorldFrozenHandler(bool isFrozen);
        public static event SetWorldFrozenHandler SetWorldFrozen;
        public static void Broadcast_SetWorldFrozen(bool isFrozen) { SetWorldFrozen?.Invoke(isFrozen); }

        //-------------------------------- End Activity Events --------------------------------//
    }
}