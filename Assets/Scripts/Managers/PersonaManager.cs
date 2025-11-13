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

        public Color GetPersonaColour()
        {
            var persona = GetPersona();
            var stats = PersonaStatsLoader.GetStats(persona);
            var colour = stats.PlayerColor;
            return colour;
        }

        // CSV references (loaded from Resources)
        private TextAsset _personaStatsCSV;
        private TextAsset _personaDetailsCSV;
    
        private Dictionary<Types.Persona, Types.PersonaState> _personas = InitializePersonas();
        private Dictionary<Types.Persona, Types.PersonaState> _trimmedPersonas;
        private Dictionary<Types.Persona, PlayerStatsStruct> _generatedPersonas;
        private bool _personasGenerated = false;
        private bool _personasTrimmed = false;
        


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
        private string _detailsCSVPath = "CSV Files/Stats/PersonaDetails";

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
            
            _personaDetailsCSV = Resources.Load<TextAsset>(_detailsCSVPath);
            if (_personaDetailsCSV == null)
            {
                DebugUtils.LogError("PersonaDetails.csv not found at Resources/" + _detailsCSVPath);
                return;
            }
            PersonaDetailsLoader.Initialize(_personaDetailsCSV.text);
            
            
            _isInitialized = true;

        
            SetPersona(_currentPersona);
        }

        public void Start()
        {
            // Listen for the player death event to reset persona
            EventBroadcaster.PlayerDeath +=  OnPlayerDeath;
            EventBroadcaster.GameRestart += OnGameRestart;
            EventBroadcaster.OpenPersonaUI += OnPlayerOpenPersonaUI;
        }

        private void OnPlayerOpenPersonaUI()
        {
            // when the player opens the UI, we want to generate everything
            InitializeRandomPersonas();
            GetTrimmedPersonas(3); // for now, we will just do 3
            Managers.AudioManager.Instance.PlayPersonaMenuOpenSound(1, 0);
            
        }

        
        private void OnPlayerDeath()
        {
            // take the current persona, and mark it as inactive
            MarkAsLost(GetPersona());
            // we want to regenerate the personas
            _personasGenerated = false;
            _personasTrimmed = false;

        }
        private void OnGameRestart()
        {
            // Reset to Normal persona
            SetPersona(Types.Persona.Normal);
            // loop through all personas and set them to available
            foreach (var key in _personas.Keys.ToList())
            {
                _personas[key] = Types.PersonaState.Available;
            }
            // we want to regenerate the personas
            _personasGenerated = false;
            _personasTrimmed = false;
        }
        
        public Dictionary<Types.Persona, Types.PersonaState> GetTrimmedPersonas(int numberOfPersonas)
        {
            /*
             * Returns a dictionary of personas trimmed to the specified number
             * of available personas, with others set to Locked.
             */
            
            // ensure we havent already generated personas
            if (_personasTrimmed){
                return _personas;
            }

            _personas = InitializePersonas();

            // Get all valid personas (ignore None + Normal)
            var validPersonas = _personas.Keys
                .Where(p => p != Types.Persona.None && p != Types.Persona.Normal)
                .ToList();

            // Shuffle the list
            validPersonas = validPersonas
                .OrderBy(_ => Guid.NewGuid())
                .ToList();

            // Assign states based on how many we want
            for (int i = 0; i < validPersonas.Count; i++)
            {
                var persona = validPersonas[i];
                bool shouldBeAvailable = i < numberOfPersonas;

                _personas[persona] = shouldBeAvailable
                    ? Types.PersonaState.Available
                    : Types.PersonaState.Lost;
            }
            
            _personasTrimmed = true;
            return _personas;
        }

        public int GetNumberOfAvailablePersonas()
        {
            /*
             * This will check the dict of personas, and return the number non NON-Lost personas
             * we also wont count the Normal persona, since its infinite, and its being stupid
             */
            int count = 0;
            for (int i = 0; i < _personas.Count; i++)
            {
                if (_personas.ElementAt(i).Value != Types.PersonaState.Lost && _personas.ElementAt(i).Key != Types.Persona.None)
                {
                    count += 1;
                }
            }

            return count;
        }

        private void Update()
        {
            /*
             * For testing purposes, we can cycle through personas with the P key
             */
            if (Input.GetKeyDown(KeyCode.K))
            {
                // Generate a new persona
                PlayerStatsStruct PS = GenerateNewPersona();
            }
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
            
            //TODO: we need to handle what is gonna carry over between deaths
            int numberOfCoins = PlayerStats.Instance.GetCoins();
            PlayerStats.Instance.SetCarryOverCoins(numberOfCoins/2); // Carry over half the coins
            
            if (_personas.ContainsKey(persona))
            {
                //_personas[persona] = Types.PersonaState.Lost;
            }
            
            
            // Set us back to Normal persona by default if we arnt currently Normal
            if (GetPersona() != Types.Persona.Normal)
            {
                SetPersona(Types.Persona.Normal);
            }
            else
            {
                // Else, its a special case where we lost the Normal persona (which means game over)
                SetPersona(Types.Persona.None);
            }
            
            
            
            
        
        }
        public void LockPersona(Types.Persona persona) => _personas[persona] = Types.PersonaState.Locked;
        public void UnlockPersona(Types.Persona persona) => _personas[persona] = Types.PersonaState.Available;
        
        
        /*
         * Persona system is being updated, where we are going to create a bunch of "bases" and procedeurally generate personas
         *
         * PersonaStatsCSV will be used for the persona bases
         */

        public PlayerStatsStruct GenerateNewPersona(Types.Persona personaType = Types.Persona.None)
        {
            /*
             * Generates a new persona with random details and base stats from the CSV
             * If personaType is None, a random persona type will be selected (excluding Normal and None)
             */

            // 1. Select random persona type if none provided
            if (personaType == Types.Persona.None)
            {
                var availablePersonas = Enum.GetValues(typeof(Types.Persona))
                    .Cast<Types.Persona>()
                    .Where(p => p != Types.Persona.None && p != Types.Persona.Normal)
                    .ToList();

                int randomIndex = UnityEngine.Random.Range(0, availablePersonas.Count);
                personaType = availablePersonas[randomIndex];
            }

            // Safety check
            if (personaType == Types.Persona.None || personaType == Types.Persona.Normal)
            {
                DebugUtils.LogError("Invalid persona type for generation: " + personaType);
                return default;
            }
            
            if (!_isInitialized)
            {
                DebugUtils.LogError("PersonaManager not initialized!");
                return default;
            }

            // 2. Random details (name, email handle, domain)
            string username = PersonaDetailsLoader.GetRandomUsername();
            string emailHandle = PersonaDetailsLoader.GetRandomEmailHandle();
            string domain = PersonaDetailsLoader.GetRandomDomain();
            string email = $"{emailHandle}@{domain}";

            // 3. Base stats
            PlayerStatsStruct baseStats = PersonaStatsLoader.GetStats(personaType);
            int statVariancePercent = 10; // +/- 10% variance
            PlayerStatsStruct stats = new PlayerStatsStruct
            {
                CurrentHealth   = baseStats.CurrentHealth,
                MaxHealth       = baseStats.MaxHealth,
                MovementSpeed   = baseStats.MovementSpeed + UnityEngine.Random.Range(-baseStats.MovementSpeed * statVariancePercent / 100f, baseStats.MovementSpeed * statVariancePercent / 100f),
                DashSpeed       = baseStats.DashSpeed + UnityEngine.Random.Range(-baseStats.DashSpeed * statVariancePercent / 100f, baseStats.DashSpeed * statVariancePercent / 100f),
                AttackDamage    = baseStats.AttackDamage + UnityEngine.Random.Range(-baseStats.AttackDamage * statVariancePercent / 100f, baseStats.AttackDamage * statVariancePercent / 100f),
                AttackCooldown  = baseStats.AttackCooldown + UnityEngine.Random.Range(-baseStats.AttackCooldown * statVariancePercent / 100f, baseStats.AttackCooldown * statVariancePercent / 100f),
                DashDamage      = baseStats.DashDamage + UnityEngine.Random.Range(-baseStats.DashDamage * statVariancePercent / 100f, baseStats.DashDamage * statVariancePercent / 100f),
                DashCooldown    = baseStats.DashCooldown + UnityEngine.Random.Range(-baseStats.DashCooldown * statVariancePercent / 100f, baseStats.DashCooldown * statVariancePercent / 100f),
                DashDistance    = baseStats.DashDistance + UnityEngine.Random.Range(-baseStats.DashDistance * statVariancePercent / 100f, baseStats.DashDistance * statVariancePercent / 100f),
                Keys            = baseStats.Keys,
                Coins           = baseStats.Coins + UnityEngine.Random.Range(-baseStats.Coins * statVariancePercent / 100, baseStats.Coins * statVariancePercent / 100),
                PlayerColor     = baseStats.PlayerColor,
                Description     = baseStats.Description,
                Email           = email,
                Username        = username,
                PersonaType     = personaType
            };

            return stats;
        }


        public void InitializeRandomPersonas()
        {
            if (_personasGenerated) return;

            _generatedPersonas = new Dictionary<Types.Persona, PlayerStatsStruct>();
            foreach (Types.Persona personaType in Enum.GetValues(typeof(Types.Persona)))
            {
                if (personaType == Types.Persona.Normal || personaType == Types.Persona.None)
                    continue;

                _generatedPersonas[personaType] = GenerateNewPersona(personaType);
            }

            _personasGenerated = true;
        }

        public PlayerStatsStruct GetGeneratedPersona(Types.Persona personaType)
        {
            if (_generatedPersonas != null && _generatedPersonas.ContainsKey(personaType))
                return _generatedPersonas[personaType];

            return PersonaStatsLoader.GetStats(personaType); // fallback
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
                if (values.Length < 14)
                {
                    Debug.LogWarning($"Skipping malformed CSV line {i}: '{line}'");
                    continue;
                }

                
                if (!Enum.TryParse(values[0].Trim(), out Types.Persona persona))
                {
                    Debug.LogWarning($"Skipping unknown Persona type '{values[0]}' at line {i}.");
                    continue;
                }

                try
                {
                    PlayerStatsStruct stats = new PlayerStatsStruct
                    {
                        CurrentHealth   = float.Parse(values[1]),
                        MaxHealth       = float.Parse(values[2]),
                        MovementSpeed   = float.Parse(values[3]),
                        DashSpeed       = float.Parse(values[4]),
                        AttackDamage    = float.Parse(values[5]),
                        AttackCooldown  = float.Parse(values[6]),
                        DashDamage      = float.Parse(values[7]),
                        DashCooldown    = float.Parse(values[8]),
                        DashDistance    = float.Parse(values[9]),
                        Keys            = int.Parse(values[10]),
                        Coins           = int.Parse(values[11]),
                        PlayerColor     = ParseColor(values[12]),
                        Description     = values[13],
                        Email           = values.Length > 14 ? values[14] : "",
                        Username      = values.Length > 15 ? values[15] : ""
                    };

                    _personaStats[persona] = stats;
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"Error parsing stats for persona '{values[0]}' at line {i}: {e.Message}");
                }
            }

            _isLoaded = true;
        }


        public static PlayerStatsStruct GetStats(Types.Persona persona)
        {
            if (!_isLoaded)
            {
                //Debug.LogError("PersonaStatsLoader not initialized!");
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
                case "pink": return new Color(1f, 0.75f, 0.8f);
                case "gray":
                case "grey": return Color.gray;
                default:
                    Debug.LogWarning($"Unknown color name '{colorName}', defaulting to white.");
                    return Color.white;
            }
        }
    }
    
    
    public static class PersonaDetailsLoader
    {
        private static List<string> _Usernames = new();
        private static List<string> _emails = new();
        private static List<string> _domains = new();
        private static bool _isLoaded = false;

        public static void Initialize(string csvText)
        {
            if (_isLoaded) return;

            string[] lines = csvText.Split('\n');

            // Skip header
            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                if (string.IsNullOrWhiteSpace(line)) continue;

                string[] values = line.Split(',');

                // If name column exists and not empty
                if (values.Length > 0 && !string.IsNullOrWhiteSpace(values[0]))
                    _Usernames.Add(values[0].Trim());

                // If email handle column exists and not empty
                if (values.Length > 1 && !string.IsNullOrWhiteSpace(values[1]))
                    _emails.Add(values[1].Trim());

                // If domain column exists and not empty
                if (values.Length > 2 && !string.IsNullOrWhiteSpace(values[2]))
                {
                    string domain = values[2].Trim();
                    if (domain.StartsWith(".")) domain = domain.Substring(1); // remove leading "."
                    _domains.Add(domain);
                }
            }

            _isLoaded = true;

            DebugUtils.LogSuccess($"Loaded {_Usernames.Count} names, {_emails.Count} handles, {_domains.Count} domains.");
        }


        public static string GetRandomUsername() => _Usernames.Count > 0 ? _Usernames[UnityEngine.Random.Range(0, _Usernames.Count)] : "Unknown";
        public static string GetRandomEmailHandle() => _emails.Count > 0 ? _emails[UnityEngine.Random.Range(0, _emails.Count)] : "user";
        public static string GetRandomDomain() => _domains.Count > 0 ? _domains[UnityEngine.Random.Range(0, _domains.Count)] : "example.com";
    }


    
}