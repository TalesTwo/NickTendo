using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedNPCBUDDEE : AnimatedEntity
{

    public List<Sprite> idle;
    public List<Sprite> alert;
    public List<Sprite> talking;
    
    // Start is called before the first frame update
    void Start()
    {
        AnimationSetup();
        DefaultAnimationCycle = alert;
        EventBroadcaster.StartDialogue += SetTalking;
        EventBroadcaster.StopDialogue += SetIdle;
        EventBroadcaster.PlayerDeath += SetAlert;
    }

    // Update is called once per frame
    void Update()
    {
        AnimationUpdate();
    }

    public void SetTalking(string npcName)
    {
        if (npcName == "BUDDEE")
        {
            DefaultAnimationCycle = talking;
        }
    }

    public void SetIdle()
    {
        DefaultAnimationCycle = idle;
    }

    public void SetAlert()
    {
        DefaultAnimationCycle = alert;
    }
}
