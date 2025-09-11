using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueController : MonoBehaviour
{
    // dialogue objects
    public GameObject dialogueBox;
    public Text dialogueText;
    public string[] dialogue;
    
    private int index;

    // optional wordspeed for if we want the text to display at a certain speed
    public float wordSpeed;
    
    // tracking if the player is in range to engage the dialogue
    public bool playerIsClose;
    
    // checking if the player can continue the dialogue (line must finish first)
    private bool canContinue = false;
    
    void Start()
    {
        dialogueText.text = ""; // fixes a bug later in the code (do not touch)
    }

    // Update is called once per frame
    void Update()
    {
        // engage dialogue box
        if (Input.GetKeyDown(KeyCode.E) && playerIsClose)
        {
            if (dialogueBox.activeInHierarchy)
            {
                ZeroText();
            }
            else
            {
                dialogueBox.SetActive(true);
                StartCoroutine(Typing());
            }
        }

        // move to next line of dialogue
        if (Input.GetKeyDown(KeyCode.Space) && canContinue)
        {
            NextLine();
        }

        // checking if current line of dialogue is finished
        if (dialogueText.text == dialogue[index])
        {
            canContinue = true;
        }
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
            ZeroText();
        }
    }

    // check if player enters dialogue area
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerIsClose = true;
        }
    }

    // check if player leaves dialogue area
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerIsClose = false;
            ZeroText();
        }
    }
}
