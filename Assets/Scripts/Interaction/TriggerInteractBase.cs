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
    [SerializeField] private Vector3 interactPromptOffset = new Vector3(0, 0.1f, 0);
    
    // Offset for the interact prompt
    private GameObject interactPromptInstance;
    
    
    
    public virtual void Interact() { }

    protected virtual void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        var promptPrefab = DialogueManager.Instance.GetInteractPrompt;
        if (promptPrefab != null)
        {
            // Instantiate in world space
            interactPromptInstance = Instantiate(promptPrefab);

            // Set its scale to prefab's original
            interactPromptInstance.transform.localScale = promptPrefab.transform.localScale;

            // Set initial position
            interactPromptInstance.transform.position = transform.position + interactPromptOffset;

            // Ensure it renders on top
            var sr = interactPromptInstance.GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.sortingOrder = 999;

            interactPromptInstance.SetActive(false);
        }
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


