using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCTriggerInteraction : TriggerInteractBase
{
    public TextAsset npcDialogue;
    
    public string[] dialogue;
    public Image sprite;
    public string npcName;
    
    // Start is called before the first frame update
    protected override void Start()
    {
        dialogue = npcDialogue.text.Split('\n');
        base.Start();
    }


    public override void Interact()
    {
        // call to super
        base.Interact();
        EventBroadcaster.Broadcast_StartDialogue(dialogue, sprite, npcName);
    }
}
