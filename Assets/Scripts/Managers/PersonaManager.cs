using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Managers
{
    public class PersonaManager : Singleton<PersonaManager>
    {
        /*
     * The PersonaManager is responsible for handling which "online persona" 
     * the player currently has equipped, and ensuring the player's stats are
     * updated accordingly when it changes.
     */

        // The current persona of the player
        private Types.Persona _currentPersona = Types.Persona.Normal;
        public Types.Persona GetPersona() { return _currentPersona; }

        // CSV reference (loaded from Resources)
        private TextAsset _personaStatsCSV;
    
        private Dictionary<Types.Persona, Types.PersonaState> _personas = InitializePersonas();

        private static Dictionary<Types.Persona, Types.PersonaState> InitializePersonas()
        {
            return Enum.GetValues(typeof(Types.Persona))
                .Cast<Types.Persona>()
                .ToDictionary(p => p, p => Types.PersonaState.Available);
        }

        // Getter for all personas and their states
        public Dictionary<Types.Persona, Types.PersonaState> GetAllPersonas() => _personas;
    

        // Initialization flag
        private bool _isInitialized = false;
        private string _csvPath = "CSV Files/Stats/PersonaStats";

        // --------------------------------------------------
        protected override void Awake()
        { 
            /*
           * (I guess this HAS to be in a "Resources" folder for Resources.Load to work   )
           */
            _personaStatsCSV = Resources.Load<TextAsset>(_csvPath);
            if (_personaStatsCSV == null)
            {
                DebugUtils.LogError("PersonaStats.csv not found at Resources/" + _csvPath);
                return;
            }
        
            PersonaStatsLoader.Initialize(_personaStatsCSV.text);
            _isInitialized = true;

        
            SetPersona(_currentPersona);
        }

        public void Start()
        {
            // Listen for the player death event to reset persona
            EventBroadcaster.PlayerDeath +=  OnPlayerDeath;
        }

        public void OnPlayerDeath()
        {
            // take the current persona, and mark it as inactive
            MarkAsLost(GetPersona());
        }

        public int GetNumberOfAvailablePersonas()
        {
            /*
             * This will check the dict of personas, and return the number non NON-Lost personas
             */
            int count = 0;
            for (int i = 0; i < _personas.Count; i++)
            {
                if (_personas.ElementAt(i).Value != Types.PersonaState.Lost)
                {
                    count += 1;
                }
            }

            return count;
        }

    
        // --------------------------------------------------
        public void SetPersona(Types.Persona newPersona)
        {
            /*
         * Anytime the persona is set, we want to:
         * - Update state dictionary
         * - Re-apply the player’s stats
         * - Broadcast the change to any listeners
         */

            // Reset previously selected persona(s)
            foreach (var key in _personas.Keys.ToList())
            {
                if (_personas[key] == Types.PersonaState.Selected)
                    _personas[key] = Types.PersonaState.Available;
            }

            // Update the selected persona state
            _currentPersona = newPersona;
            _personas[_currentPersona] = Types.PersonaState.Selected;

            // Update player stats if loader is ready
            if (_isInitialized)
            {
                PlayerStatsStruct personaStats = PersonaStatsLoader.GetStats(newPersona);
                PlayerStats.Instance.InitializePlayerStats(personaStats);
            }

            // Broadcast after stats are applied
            EventBroadcaster.Broadcast_PersonaChanged(newPersona);
        }

    
        public void MarkAsLost(Types.Persona persona)
        {
            if (_personas.ContainsKey(persona))
            {
                _personas[persona] = Types.PersonaState.Lost;
            }
            
            // if the lost person was not a Normal, set it to a normal person
            if (_currentPersona != Types.Persona.Normal)
            {
                SetPersona(Types.Persona.Normal);
            }
            
        
        }
        public void LockPersona(Types.Persona persona) => _personas[persona] = Types.PersonaState.Locked;
        public void UnlockPersona(Types.Persona persona) => _personas[persona] = Types.PersonaState.Available;
    
        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.O))
            {
                Debug.Log("Current Persona: " + _currentPersona);
                DebugUtils.Log("Number of active personas: " + GetNumberOfAvailablePersonas());
            }

            if (Input.GetKeyDown(KeyCode.U))
            {
                foreach (var kvp in _personas)
                {
                    Debug.Log($"Persona: {kvp.Key}, State: {kvp.Value}");
                }
            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                MarkAsLost(_currentPersona);
            }
        }

    }



    public static class PersonaStatsLoader
    {
        private static Dictionary<Types.Persona, PlayerStatsStruct> _personaStats = new();
        private static bool _isLoaded = false;

        public static void Initialize(string csvText)
        {
            if (_isLoaded) return; 

            string[] lines = csvText.Split('\n');

            for (int i = 1; i < lines.Length; i++) 
            {
                string line = lines[i].Trim();
                if (string.IsNullOrWhiteSpace(line)) continue;

                string[] values = line.Split(',');


                var persona = (Types.Persona) Enum.Parse(typeof(Types.Persona), values[0]);
            
                PlayerStatsStruct stats = new PlayerStatsStruct
                {
                    CurrentHealth = float.Parse(values[1]),
                    MaxHealth = float.Parse(values[2]),
                    MovementSpeed = float.Parse(values[3]),
                    DashSpeed = float.Parse(values[4]),
                    AttackDamage = float.Parse(values[5]),
                    AttackCooldown = float.Parse(values[6]),
                    DashDamage = float.Parse(values[7]),
                    DashCooldown = float.Parse(values[8]),
                    DashDistance = float.Parse(values[9]),
                    Keys = int.Parse(values[10]),
                    Coins = int.Parse(values[11]),
                    PlayerColor = ParseColor(values[12])
                };
                // debug the coins value

                _personaStats[persona] = stats;
        

            }

            _isLoaded = true;
        }

        public static PlayerStatsStruct GetStats(Types.Persona persona)
        {
            if (!_isLoaded)
            {
                Debug.LogError("PersonaStatsLoader not initialized!");
                return default;
            }

            if (_personaStats.TryGetValue(persona, out var stats))
            {
                // We can do specific stuff per persona here if needed
                switch (persona)
                {
                    case Types.Persona.Normal:
                        // FILL HERE IF WE EVER WANT TO DO SPECIAL HANDLING
                        break;
                
                    default:
                        break;
                }

                return stats;
            }

            Debug.LogError($"Persona not found: {persona}");
            return default;
        }
        
        private static Color ParseColor(string colorName)
        {
            switch (colorName.Trim().ToLowerInvariant())
            {
                case "white": return Color.white;
                case "black": return Color.black;
                case "red": return Color.red;
                case "green": return Color.green;
                case "blue": return Color.blue;
                case "yellow": return Color.yellow;
                case "orange": return new Color(1f, 0.5f, 0f);
                case "cyan": return Color.cyan;
                case "magenta": return Color.magenta;
                case "gray":
                case "grey": return Color.gray;
                default:
                    Debug.LogWarning($"Unknown color name '{colorName}', defaulting to white.");
                    return Color.white;
            }
        }
    }
}