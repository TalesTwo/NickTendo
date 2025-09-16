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
    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        dialogue = npcDialogue.text.Split('\n');
    }

    public override void Interact()
    {
        EventBroadcaster.Broadcast_StartDialogue(dialogue, sprite, npcName);
    }
}
