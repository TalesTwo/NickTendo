using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

namespace Managers
{
    public class DialogueManager : Singleton<DialogueManager>
    {
        [Header("Dialogue Box Components")]
        public GameObject dialogueBox;
        public Image playerSprite;
        public Image blur;
        private Image _playerTransparency;
        public Image NPCSprite;
        private Image _npcTransparency;
        public Text playerNameText;
        public Text NPCNameText;
        public TextMeshProUGUI dialogueText;
        public GameObject animatedEButton;

        [Header("Dialogue CSV")] 
        public TextAsset csv;

        [Header("Dialogue Sprites")]
        [SerializeField] private Sprite buddee_sheepish;
        [SerializeField] private Sprite buddee_smug;
        [SerializeField] private Sprite buddee_bothered;
        [SerializeField] private Sprite buddee_speechless;
        [SerializeField] private Sprite buddee_confused;
        [SerializeField] private Sprite buddee_gremlin;
        [SerializeField] private Sprite buddee_basic1;
        [SerializeField] private Sprite buddee_basic2;
        [SerializeField] private Sprite buddee_basic3;
        [SerializeField] private Sprite buddee_worried;
        [SerializeField] private Sprite buddee_pity;
        [SerializeField] private Sprite buddee_crestfallen;
        [SerializeField] private Sprite buddee_sniffle;
        [SerializeField] private Sprite buddee_angry;
        [SerializeField] private Sprite buddee_threatening;
        [SerializeField] private Sprite buddee_pissed;
        public Sprite playerSmileSprite;
        public Sprite capeSprite;
        
        [Header("storage dictionaries")]
        private Dictionary<string, Sprite> _buddeeSprites;
        private Dictionary<string, Sprite> _playerSprites;
        private Dictionary<string, Sprite> _systemSprites;
        
        [Header("current use dictionary")]
        private Dictionary<string, Sprite> _npcSprites;
    
        private int _index;
        private string _characterName;
        private string _playerName;
        private string _spokenLine;
        private List<string[]> _dialogue;
        private Dictionary<string, List<string[]>> _lines;
        
        [SerializeField] private GameObject interactPrompt;
        
        private string _previousDialogueState;
    
        // optional wordspeed for if we want the text to display at a certain speed
        public float wordSpeed;
    
        // checking if the player can continue the dialogue (line must finish first)
        private bool _canContinue = false;
    
        // checking if the player is still in the dialogue
        private bool _isReading = false;
        
        // checking if the dialogue is currently typing
        private bool _isTyping = false;
        
        // bool to identify when Typing() should skip to the end of the line
        private bool _skipToEnd = false;

        // this dialog is random, and only one line will play at a time
        private bool _dialogIsRandom = false;
    
        // Start is called before the first frame update
        void Start()
        {
            EventBroadcaster.StartDialogue += ActivateDialogue;
            _playerTransparency = playerSprite.gameObject.GetComponent<Image>();
            _npcTransparency = NPCSprite.gameObject.GetComponent<Image>();
            FillSpriteDictionary();
            ParseDialogue();
            animatedEButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(2.5f, -2.5f);
            animatedEButton.SetActive(false);
        }

        // not the best way to do it, but it is the most convenient
        private void FillSpriteDictionary()
        {
            _buddeeSprites = new Dictionary<string, Sprite>();
            _playerSprites = new Dictionary<string, Sprite>();
            _systemSprites = new Dictionary<string, Sprite>();

            _buddeeSprites["default"] = buddee_basic1; // we will just use basic1 as the default for now
            _buddeeSprites["sheepish"] = buddee_sheepish;
            _buddeeSprites["smug"] = buddee_smug;
            _buddeeSprites["bothered"] = buddee_bothered;
            _buddeeSprites["speechless"] = buddee_speechless;
            _buddeeSprites["confused"] = buddee_confused;
            _buddeeSprites["gremlin"] = buddee_gremlin;
            _buddeeSprites["basic1"] = buddee_basic1;
            _buddeeSprites["basic2"] = buddee_basic2;
            _buddeeSprites["basic3"] = buddee_basic3;
            _buddeeSprites["worried"] = buddee_worried;
            _buddeeSprites["pity"] = buddee_pity;
            _buddeeSprites["crestfallen"] = buddee_crestfallen;
            _buddeeSprites["sniffle"] = buddee_sniffle;
            _buddeeSprites["angry"] = buddee_angry;
            _buddeeSprites["threatening"] = buddee_threatening;
            _buddeeSprites["pissed"] = buddee_pissed;
            
            _playerSprites["smile"] = playerSmileSprite;
            
            _systemSprites["cape"] = capeSprite;
        }

        private void ParseDialogue()
        {
            _lines = new Dictionary<string, List<string[]>>();
            string[] lines = csv.text.Split('\n');
            foreach (string line in lines)
            {
                string[] cells = line.Split(',');
                string[] remaining = cells.Skip(2).ToArray();
                if (!_lines.ContainsKey(cells[0]))
                {
                    _lines[cells[0]] = new List<string[]>();
                }
                _lines[cells[0]].Add(remaining);
            }
        }

        // Update is called once per frame
        IEnumerator CheckInput()
        {
            while (_isReading)
            {
                // move to next line of dialogue
                if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space))
                {
                    if (_canContinue)
                    {
                        NextLine();
                    }
                    else if (_isTyping)
                    {
                        _skipToEnd = true;
                    }
                    
                }

                // checking if current line of dialogue is finished
                if (dialogueText.text == _spokenLine)
                {
                    _canContinue = true;
                }

                yield return null;            
            }

        }

        private void ActivateDialogue(string npcName)
        {
            EventBroadcaster.Broadcast_StartStopAction(); // stop player inputs
            
            ZeroText();
            
            // find which dialogue is currently being displayed
            _characterName = npcName;
            switch (_characterName)
            {
                case "BUDDEE":
                    string state = GameStateManager.Instance.GetBuddeeDialogState();
                    _dialogIsRandom = state.Contains("Random");
                    _dialogue = _lines[state];
                    _npcSprites = _buddeeSprites;
                    break;
                default:
                    _dialogue = _lines["Example2"];
                    break;
            }
            
            _playerName = PlayerStats.Instance.GetPlayerName();
            NPCNameText.text = _characterName;
            playerNameText.text = _playerName;

            _canContinue = false;
            _isReading = true;
        
            dialogueBox.SetActive(true);
            NPCSprite.gameObject.SetActive(true);
            Color color = PersonaManager.Instance.GetPersonaColour();
            playerSprite.gameObject.GetComponent<Image>().color = color;
            playerSprite.gameObject.SetActive(true);
            blur.gameObject.SetActive(true);
            StartCoroutine(CheckInput());
            StartCoroutine(Typing());
        }
    
        // resets the dialogue box
        public void ZeroText()
        {
            dialogueText.text = "";
            _index = 0;
            dialogueBox.SetActive(false);
            NPCSprite.gameObject.SetActive(false);
            playerSprite.gameObject.SetActive(false);
            blur.gameObject.SetActive(false);
        }
    
        // types each letter in the dialogue one at a time
        IEnumerator Typing()
        {
            _isTyping = true;
            
            if (_dialogIsRandom)
            {
                Random random = new Random();
                _index = random.Next(0, _dialogue.Count);
            }
            
            playerSprite.sprite = _playerSprites["smile"];
            NPCSprite.sprite = _npcSprites["default"];
            
            if (_dialogue[_index][0] == _characterName)
            {
                
                //NPCSprite.sprite = _npcSprites[_dialogue[_index][1]];
                
                // adding edge case to avoid crashes (adding lower check to be safe lol)
                string key = _dialogue[_index][1].ToLower();

                if (_npcSprites.ContainsKey(key))
                {
                    NPCSprite.sprite = _npcSprites[key];
                }
                else
                {
                    DebugUtils.LogError($"Sprite key '{key}' not found, using default instead.");
                    NPCSprite.sprite = _npcSprites["default"];
                }
                
                NPCNameText.gameObject.SetActive(true);
                playerNameText.gameObject.SetActive(false);
                
                Color currentColor = _npcTransparency.color;
                currentColor.a = 1f;
                _npcTransparency.color = currentColor;

                currentColor = _playerTransparency.color;
                currentColor.a = 0.5f;
                _playerTransparency.color = currentColor;
            }
            else if (_dialogue[_index][0] == "Player")
            {
                playerSprite.sprite = _playerSprites[_dialogue[_index][1]];
                
                NPCNameText.gameObject.SetActive(false);
                playerNameText.gameObject.SetActive(true);
                
                Color currentColor = _npcTransparency.color;
                currentColor.a = 0.5f;
                _npcTransparency.color = currentColor;

                currentColor = _playerTransparency.color;
                currentColor.a = 1.0f;
                _playerTransparency.color = currentColor;
            }
            else
            {
                NPCNameText.gameObject.SetActive(false);
                playerNameText.gameObject.SetActive(false);
                
                Color currentColor = _npcTransparency.color;
                currentColor.a = 0.5f;
                _npcTransparency.color = currentColor;

                currentColor = _playerTransparency.color;
                currentColor.a = 0.5f;
                _playerTransparency.color = currentColor;
            }
            int talkingtonetimer = 0;
            //Debug.Log(_dialogue[_index][2]);
            _spokenLine = _dialogue[_index][2].Replace("{player_name}", _playerName);
            dialogueText.text = _spokenLine;
            dialogueText.maxVisibleCharacters = 0;
            animatedEButton.SetActive(false);
            foreach (char letter in _spokenLine)
            {
                dialogueText.maxVisibleCharacters++;
                ++talkingtonetimer;
                if (talkingtonetimer == 10)
                {
                    talkingtonetimer = 0;
                    if (!_skipToEnd)
                    {
                        AudioManager.Instance.PlayPlayerTalkingTone();
                    }
                }
                if (!_skipToEnd)
                {
                    yield return new WaitForSeconds(wordSpeed);
                }
            }
            animatedEButton.SetActive(true);
            _isTyping = false;
            _skipToEnd = false;
            talkingtonetimer = 0;
        }
    
        // advances to next line of dialogue
        public void NextLine()
        {
            _canContinue = false;
        
            if ((_index < _dialogue.Count - 1) && !_dialogIsRandom)
            {
                _index++;
                dialogueText.text = "";
                StartCoroutine(Typing());
            }
            else
            {
                _isReading = false;
                _dialogIsRandom = false;
                EventBroadcaster.Broadcast_StartStopAction(); // start player inputs
                GameStateManager.Instance.Dialogue("BUDDEE");
                EventBroadcaster.Broadcast_StopDialogue();
                ZeroText();
            }
        }
        
        public GameObject GetInteractPrompt => interactPrompt;


        public void RunSingleDialogue(string speaker, string state, bool freezeWorld = false)
        {
            /*
             * this will run a one time dialogue line for a given speaker and dialogue state (will save the current dialogue state and restore it after)
             */
            if (freezeWorld)
            {
                // Im lazy, and allowing the hitbox to handle turning this off lol
                EventBroadcaster.Broadcast_SetWorldFrozen(true);
            }
            _previousDialogueState = GameStateManager.Instance.GetBuddeeDialogState();
            GameStateManager.Instance.SetBuddeeDialogState(state);
            ActivateDialogue(speaker);
            // set the dialogue state back 
            GameStateManager.Instance.SetBuddeeDialogState(_previousDialogueState);
            
        }
    }
}
