using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonaTriggerInteraction : TriggerInteractBase
{
    [SerializeField] private GameObject personaSelectionUI;

    private bool _isPersonaUIOpen = false; public void ClosePersonaUI() { _isPersonaUIOpen = false; }
    
    public override void Interact()
    {
        if (personaSelectionUI)
        {
            Debug.Log("UI about to open");
            personaSelectionUI.GetComponent<PersonaUI>()?.OpenPersonaUI();
        } else {
            Debug.Log("No UI assigned to PersonaTriggerInteraction");
        }
    }

    protected override void Start()
    {
        // we dont care about the base start for this one
        EventBroadcaster.ClosePersonaUI += ClosePersonaUI;
    }



    private void Update()
    {
        // when the player presses P, we will "interact" with this object (assuming its on scree in some way)
        if (Input.GetKeyDown(KeyCode.P))
        {
            HandlePersonaUI();
        }
    }


    public void HandlePersonaUI()
    {
        Debug.Log($"HandlePersonaUI -> isOpen={_isPersonaUIOpen}");
        
        if (!_isPersonaUIOpen || personaSelectionUI == null)
        {
            Interact();
            _isPersonaUIOpen = true;
        }
        else
        {
            if(personaSelectionUI) personaSelectionUI.GetComponent<PersonaUI>()?.ClosePersonaUI();
            _isPersonaUIOpen = false;
        }
    }
    

    
    
}
