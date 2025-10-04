using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

using UnityEngine;

public class PersonaManager : MonoBehaviour
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
    

    // Initialization flag
    private bool _isInitialized = false;
    private string _csvPath = "CSV Files/Stats/PersonaStats";

    // --------------------------------------------------
    public void Awake()
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

    // --------------------------------------------------
    public void SetPersona(Types.Persona newPersona, bool bLog = true)
    {
        /*
         * Anytime the persona is set, we want to:
         * Re-apply the player’s stats
         * Broadcast the change to any listeners
         */

        // Update internal state
        _currentPersona = newPersona;

        // Update player stats if loader is ready
        if (_isInitialized)
        {
            // Retrieve stat line from CSV
            PlayerStatsStruct personaStats = PersonaStatsLoader.GetStats(newPersona);

            // Apply those stats to player
            PlayerStats.Instance.InitializePlayerStats(personaStats);
        }
        
        // Broadcast after stats are applied (probably for the UI to update)
        EventBroadcaster.Broadcast_PersonaChanged(newPersona);
        if(bLog) { PlayerStats.Instance.DisplayAllStats(); }
    }

    public void Update()
    {
        // if we press O, swap to a random persona for testing
        if (Input.GetKeyDown(KeyCode.O))
        {
            Array personas = Enum.GetValues(typeof(Types.Persona));
            Types.Persona randomPersona = (Types.Persona)personas.GetValue(UnityEngine.Random.Range(0, personas.Length));
            SetPersona(randomPersona);
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
                Coins = int.Parse(values[11])
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
}





