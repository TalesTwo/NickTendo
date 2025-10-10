using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

namespace Managers
{
    public class DialogueManager : Singleton<DialogueManager>
    {
        [Header("Dialogue Box Components")]
        public GameObject dialogueBox;
        public GameObject spaceButton;
        public Image playerSprite;
        private Image _playerTransparency;
        public Image NPCSprite;
        private Image _npcTransparency;
        public Text playerNameText;
        public Text NPCNameText;
        public Text dialogueText;

        [Header("Dialogue CSV")] 
        public TextAsset csv;

        [Header("Dialogue Sprites")]
        public Sprite buddeeSmileSprite;
        public Sprite buddeeSurpriseSprite;
        public Sprite buddeeDefaultSprite;
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
        private List<string[]> _dialogue;
        private Dictionary<string, List<string[]>> _lines;
        
        [SerializeField] private GameObject interactPrompt;
    
        // optional wordspeed for if we want the text to display at a certain speed
        public float wordSpeed;
    
        // checking if the player can continue the dialogue (line must finish first)
        private bool _canContinue = false;
    
        // checking if the player is still in the dialogue
        private bool _isReading = false;

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
        }

        // not the best way to do it, but it is the most convenient
        private void FillSpriteDictionary()
        {
            _buddeeSprites = new Dictionary<string, Sprite>();
            _playerSprites = new Dictionary<string, Sprite>();
            _systemSprites = new Dictionary<string, Sprite>();

            _buddeeSprites["surprise"] = buddeeSurpriseSprite;
            _buddeeSprites["smile"] = buddeeSmileSprite;
            _buddeeSprites["default"] = buddeeDefaultSprite;
            
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
                if (Input.GetKeyDown(KeyCode.Space) && _canContinue)
                {
                    NextLine();
                    spaceButton.SetActive(false);
                }

                // checking if current line of dialogue is finished
                if (dialogueText.text == _dialogue[_index][2])
                {
                    _canContinue = true;
                    spaceButton.SetActive(true);
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
            playerSprite.gameObject.SetActive(true);
            StartCoroutine(Typing());
            StartCoroutine(CheckInput());
        }
    
        // resets the dialogue box
        public void ZeroText()
        {
            dialogueText.text = "";
            _index = 0;
            dialogueBox.SetActive(false);
            NPCSprite.gameObject.SetActive(false);
            playerSprite.gameObject.SetActive(false);
        }
    
        // types each letter in the dialogue one at a time
        IEnumerator Typing()
        {
            if (_dialogIsRandom)
            {
                Random random = new Random();
                _index = random.Next(0, _dialogue.Count);
            }
            
            playerSprite.sprite = _playerSprites["smile"];
            NPCSprite.sprite = _npcSprites["default"];
            
            if (_dialogue[_index][0] == _characterName)
            {
                NPCSprite.sprite = _npcSprites[_dialogue[_index][1]];
                
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
                // todo: add part for system images
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
            foreach (char letter in _dialogue[_index][2].ToCharArray())
            {
                dialogueText.text += letter;
                ++talkingtonetimer;
                if (talkingtonetimer == 10)
                {
                    talkingtonetimer = 0;
                    AudioManager.Instance.PlayPlayerTalkingTone();
                }
                yield return new WaitForSeconds(wordSpeed);
            }
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
                GameStateManager.Instance.SetBuddeeDialogState("IntroRandom");
                EventBroadcaster.Broadcast_StopDialogue();
                ZeroText();
            }
        }
        
        public GameObject GetInteractPrompt => interactPrompt;
    }
}
