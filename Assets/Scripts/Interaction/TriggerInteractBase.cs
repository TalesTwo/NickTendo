using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;

public class TriggerInteractBase : MonoBehaviour, IInteractable
{
    public GameObject Player { get; set; }
    public bool CanInteract { get; set; }

    // optional variable for an interact button prompt
    private GameObject interactPrompt;
    // bool to handle if we have an interact prompt or not
    [Header("Interact Prompt Settings")]
    [SerializeField] private bool hasInteractPrompt = true;
    
    // Offset for the interact prompt
    [SerializeField]  private GameObject interactPromptInstance;
    
    
    
    public virtual void Interact() { }

    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        if (!CanInteract) { return; }
        // if we press our Interact key, we will interact with the object
        if (Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == Player)
        {
            DebugUtils.Log("Player in range to interact with " + gameObject.name);
            CanInteract = true;
            // enable the interact prompt if we have one
            if (interactPromptInstance != null)
            {
                DebugUtils.Log("Enabling interact prompt for " + gameObject.name);
                interactPromptInstance.SetActive(true);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == Player)
        {
            DebugUtils.Log("Player out of range to interact with " + gameObject.name);
            CanInteract = false;
            // disable the interact prompt if we have one
            if (interactPromptInstance != null)
            {
                interactPromptInstance.SetActive(false);
            }
        }
    }
    
}
