using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonaTriggerInteraction : TriggerInteractBase
{
    [SerializeField] private GameObject personaSelectionUI;

    
    public override void Interact()
    {
        if(personaSelectionUI) personaSelectionUI.GetComponent<PersonaUI>()?.OpenPersonaUI();
    }
}
