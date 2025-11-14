using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;

public class BossDialogueHitbox : MonoBehaviour
{
    // Start is called before the first frame update
    // Hold a reference to what the dialogue state was before entering the hitbox
    private string _previousDialogueState = "";
    private bool _hasTriggered = false;
    // bool to hold whether we have changed the dialogue state
    [Header("Dialogue Settings")]
    [SerializeField] private string dialogueStateOnEnter = "ENTER_DIALOGUE_STATE";
    [SerializeField] private string speakerName = "BUDDEE";
    [Header("Trigger Settings")]
    [SerializeField] private bool onlyTriggerOnce = false;
    [SerializeField] private bool shouldFreezeWorld = false;

    public GameObject boss;
    public GameObject buddee;

    
    public void Start()
    {
        // hook up to the dialogue end event to unfreeze the world
        EventBroadcaster.StopDialogue += OnDialogueEnd;
    }
    
    private void OnDialogueEnd()
    {
        if (shouldFreezeWorld)
        {
            EventBroadcaster.Broadcast_SetWorldFrozen(false);
        }
        
        boss.SetActive(true);
        buddee.SetActive(false);
        
        EventBroadcaster.Broadcast_StartBossFight();
    }
    
    // create the on trigger enter method
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("DialogueHitbox detected trigger enter with: " + other.name);
        if(other == null || other.transform.parent == null){return;}
        GameObject root = other.transform.parent.gameObject;
        if(root == null){return;}
        if (onlyTriggerOnce && _hasTriggered) { return; }
        if (root.CompareTag("Player"))
        {
            // Store the previous dialogue state
            _previousDialogueState = GameStateManager.Instance.GetBuddeeDialogState();
            // Set the dialogue state to the new state
            GameStateManager.Instance.SetBuddeeDialogState(dialogueStateOnEnter);
            // start the dialogue
            EventBroadcaster.Broadcast_StartDialogue(speakerName);
            _hasTriggered = true;
            // Freeze the world if needed
            if (shouldFreezeWorld)
            {
                EventBroadcaster.Broadcast_SetWorldFrozen(true);
            }

            if (dialogueStateOnEnter == "Tutorial3")
            {
                EventBroadcaster.Broadcast_EndTutorial();
            }
        }
    }
    
    // create the on trigger exit method
    private void OnTriggerExit2D(Collider2D other)
    {
        if(other == null || other.transform.parent == null){return;}
        GameObject root = other.transform.parent.gameObject;
        if(root == null){return;}
        if (root.CompareTag("Player"))
        {
            // Restore the previous dialogue state
            GameStateManager.Instance.SetBuddeeDialogState(_previousDialogueState);
        }
    }
}
