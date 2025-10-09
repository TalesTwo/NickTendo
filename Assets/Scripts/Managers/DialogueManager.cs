using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class DialogueManager : Singleton<DialogueManager>
    {
        [Header("Dialogue Box Components")]
        public GameObject dialogueBox;
        public GameObject spaceButton;
        public Image sprite;
        public Text nameText;
        public Text dialogueText;

        [Header("Dialogue CSV")] 
        public TextAsset csv;

        [Header("Dialogue Sprites")]
        public Sprite buddeeSmileSprite;
        public Sprite buddeeSurpriseSprite;
        public Sprite playerSmileSprite;
        public Sprite capeSprite;
    
        private int _index;
        private string _characterName;
        private string[] _dialogue;
        private Dictionary<string, List<string[]>> _lines;
        
        [SerializeField] private GameObject interactPrompt;
    
        // optional wordspeed for if we want the text to display at a certain speed
        public float wordSpeed;
    
        // checking if the player can continue the dialogue (line must finish first)
        private bool _canContinue = false;
    
        // checking if the player is still in the dialogue
        private bool _isReading = false;
    
        // Start is called before the first frame update
        void Start()
        {
            EventBroadcaster.StartDialogue += ActivateDialogue;
            ParseDialogue();
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
                if (dialogueText.text == _dialogue[_index])
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
        
            //dialogue = npcDialogue;
            _characterName = npcName;
            //sprite.sprite = npcSprite.sprite;
            nameText.text = _characterName;

            _canContinue = false;
            _isReading = true;
        
            dialogueBox.SetActive(true);
            StartCoroutine(Typing());
            StartCoroutine(CheckInput());
        }
    
        // resets the dialogue box
        public void ZeroText()
        {
            dialogueText.text = "";
            _index = 0;
            dialogueBox.SetActive(false);
        }
    
        // types each letter in the dialogue one at a time
        IEnumerator Typing()
        {
            foreach (char letter in _dialogue[_index].ToCharArray())
            {
                dialogueText.text += letter;
                yield return new WaitForSeconds(wordSpeed);
            }
        }
    
        // advances to next line of dialogue
        public void NextLine()
        {
            _canContinue = false;
        
            if (_index < _dialogue.Length - 1)
            {
                _index++;
                dialogueText.text = "";
                StartCoroutine(Typing());
            }
            else
            {
                _isReading = false;
                EventBroadcaster.Broadcast_StartStopAction(); // start player inputs
                ZeroText();
            }
        }
        
        public GameObject GetInteractPrompt => interactPrompt;
    }
}
