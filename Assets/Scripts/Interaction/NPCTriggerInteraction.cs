using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCTriggerInteraction : TriggerInteractBase
{
    public string npcName;
    
    private bool _inDialogue = false;
    
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        EventBroadcaster.StopDialogue += EndDialogue;
    }

    public void EndDialogue()
    {
        _inDialogue = false;
    }
    
    public override void Interact()
    {
        // call to super
        if (!_inDialogue)
        {
            base.Interact();
            EventBroadcaster.Broadcast_StartDialogue(npcName);
            _inDialogue = true;
        }
    }
}
