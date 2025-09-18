using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public GameObject dialogueBox;
    public GameObject spaceButton;
    public Text dialogueText;
    public Text nameText;
    public Image sprite;
    
    private int index;
    private string characterName;
    private string[] dialogue;
    
    // optional wordspeed for if we want the text to display at a certain speed
    public float wordSpeed;
    
    // checking if the player can continue the dialogue (line must finish first)
    private bool canContinue = false;
    
    // checking if the player is still in the dialogue
    private bool isReading = false;
    
    // Start is called before the first frame update
    void Start()
    {
        EventBroadcaster.StartDialogue += ActivateDialogue;
    }

    // Update is called once per frame
    IEnumerator CheckInput()
    {
        while (isReading)
        {
            // move to next line of dialogue
            if (Input.GetKeyDown(KeyCode.Space) && canContinue)
            {
                NextLine();
                spaceButton.SetActive(false);
            }

            // checking if current line of dialogue is finished
            if (dialogueText.text == dialogue[index])
            {
                canContinue = true;
                spaceButton.SetActive(true);
            }

            yield return null;            
        }

    }

    private void ActivateDialogue(string[] npcDialogue, Image npcSprite, string npcName)
    {
        ZeroText();
        
        dialogue = npcDialogue;
        characterName = npcName;
        sprite.sprite = npcSprite.sprite;
        nameText.text = characterName;

        canContinue = false;
        isReading = true;
        
        dialogueBox.SetActive(true);
        StartCoroutine(Typing());
        StartCoroutine(CheckInput());
    }
    
    // resets the dialogue box
    public void ZeroText()
    {
        dialogueText.text = "";
        index = 0;
        dialogueBox.SetActive(false);
    }
    
    // types each letter in the dialogue one at a time
    IEnumerator Typing()
    {
        foreach (char letter in dialogue[index].ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(wordSpeed);
        }
    }
    
    // advances to next line of dialogue
    public void NextLine()
    {
        canContinue = false;
        
        if (index < dialogue.Length - 1)
        {
            index++;
            dialogueText.text = "";
            StartCoroutine(Typing());
        }
        else
        {
            isReading = false;
            ZeroText();
        }
    }
}
