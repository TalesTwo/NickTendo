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
    [SerializeField] private Vector3 interactPromptOffset = new Vector3(0, 1f, 0);
    [SerializeField] private bool attachedPromptToObject = false;
    
    // Offset for the interact prompt
    private GameObject interactPromptInstance;
    
    private bool _isAllowedToInteract = true;
    protected bool _currentlyInOverlap = false;
    
    // reference to the player controller
    protected PlayerController _playerController;

    private bool _isGamePaused = false;
    private bool _globalIsInInteractionBox = false;


    public void SetInteractAllowedToInteract(bool isActive)
    {
        _isAllowedToInteract = isActive;
        if (!isActive)
        {
            CanInteract = false;
            if (interactPromptInstance != null)
            {
                interactPromptInstance.SetActive(false);
            }
        }
        
        // refresh the prompt if we are currently in overlap
        // so that we dont have to leave and re-enter to update
        if (_currentlyInOverlap && isActive)
        {
            CanInteract = true;
            if (interactPromptInstance != null)
            {
                interactPromptInstance.SetActive(true);
            }
        }
        
    }

    public virtual void Interact()
    {
        DebugUtils.LogSuccess("TTT");
        // we are attempting to interact, the players interaction is on cooldown?
        //DebugUtils.Log("The players cooldown state is " + _playerController.CanInteract());
        if (!_isAllowedToInteract) return;

        AnimationOverrideOnInteract();
        AudioManager.Instance.PlayPlayerInteractSound(0.15f, 0.1f);
    }

    
    
    protected virtual void AnimationOverrideOnInteract()
    {
        // child classes can inherit this method to override the player animation on interact
    }

    protected virtual void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        var promptPrefab = DialogueManager.Instance.GetInteractPrompt;
        if (promptPrefab != null && hasInteractPrompt)
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
            if (attachedPromptToObject)
            {
                interactPromptInstance.transform.parent = transform;
            }

            interactPromptInstance.SetActive(false);
        }
        
        _playerController = Player.GetComponent<PlayerController>();
        _isGamePaused = false;

        EventBroadcaster.GamePause += HandleGamePaused;
        EventBroadcaster.GameUnpause += HandleGameUnpaused;
    }

    private void Update()
    {
        if (!CanInteract && !_globalIsInInteractionBox) { return; }
        if (_isGamePaused) { return; }
        // if we press our Interact key, we will interact with the object
        if (Input.GetKeyDown(KeyCode.E))
        {
            // check to ensure we are allowed to interact (not on cooldown)
            // check the timer
            Interact();
        }
    }
    
    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == Player)
        {
            _globalIsInInteractionBox = true;
            _currentlyInOverlap = true;
            // we shouldnt be able to see the interact prompt or interact if we arent allowed to
            if (!_isAllowedToInteract) 
            {
                CanInteract = false;  
                return;               
            }
            
            //_currentlyInOverlap = true;
            //DebugUtils.Log("Player in range to interact with " + gameObject.name);
            CanInteract = true;
            // enable the interact prompt if we have one
            if (interactPromptInstance != null)
            {
                //DebugUtils.Log("Enabling interact prompt for " + gameObject.name);
                interactPromptInstance.SetActive(true);
            }
        }
    }
    protected void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == Player)
        {
            _globalIsInInteractionBox = false;
            _currentlyInOverlap = false;
            //DebugUtils.Log("Player out of range to interact with " + gameObject.name);
            CanInteract = false;
            // disable the interact prompt if we have one
            if (interactPromptInstance != null)
            {
                interactPromptInstance.SetActive(false);
            }
        }
    }

    void HandleGamePaused()
    {
        _isGamePaused = true;
    }

    void HandleGameUnpaused()
    {
        _isGamePaused = false;
    }    
}