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
        public delegate void PlayerDamagedHandler(float damageAmount);
        public static event PlayerDamagedHandler PlayerDamaged;
        public static void Broadcast_PlayerDamaged(float damageAmount) { PlayerDamaged?.Invoke(damageAmount); }
        
        // Start Dialogue Broadcaster
        public delegate void StartDialogueHandler(string[] message, Image sprite, string name);
        public static event StartDialogueHandler StartDialogue;
        public static void Broadcast_StartDialogue(string[] message, Image sprite, string name) { StartDialogue?.Invoke(message, sprite, name); }

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

        //-------------------------------- End Activity Events --------------------------------//


    }
}