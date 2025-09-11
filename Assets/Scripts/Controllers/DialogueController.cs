using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueController : MonoBehaviour
{
    public GameObject dialogueBox;
    public Text dialogueText;
    public string[] dialogue;
    
    private int index;

    public float wordSpeed;
    public bool playerIsClose;
    
    private bool canContinue = false;
    
    void Start()
    {
        dialogueText.text = "";
    }

    // Update is called once per frame
    void Update()
    {
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

        if (Input.GetKeyDown(KeyCode.Space) && canContinue)
        {
            NextLine();
        }

        if (dialogueText.text == dialogue[index])
        {
            canContinue = true;
        }
    }

    public void ZeroText()
    {
        dialogueText.text = "";
        index = 0;
        dialogueBox.SetActive(false);
    }

    IEnumerator Typing()
    {
        foreach (char letter in dialogue[index].ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(wordSpeed);
        }
    }

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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerIsClose = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerIsClose = false;
            ZeroText();
        }
    }
}
