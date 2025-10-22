using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BUDDEEController : MonoBehaviour
{
    
    
    // Start is called before the first frame update
    void Start()
    {
        EventBroadcaster.StartDialogue += IsTalking;
    }

    private void IsTalking(string npcName)
    {
        if (npcName == "BUDDEE")
        {
            
        }
    }
}
