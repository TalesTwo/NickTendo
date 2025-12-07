using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;

public class PersonaTriggerInteraction : TriggerInteractBase
{
    [SerializeField] private GameObject personaSelectionUI;

    private bool _isPersonaUIOpen = false; public void ClosePersonaUI() { _isPersonaUIOpen = false; }
    
    private bool _isPlayerAllowedToInteract = true; void SetPlayerAllowedToInteract(bool isAllowed) { _isPlayerAllowedToInteract = isAllowed; } public bool IsPlayerAllowedToInteract() { return _isPlayerAllowedToInteract; }
    public override void Interact()
    {
        if (personaSelectionUI && !PlayerManager.Instance.IsTeleporting() )
        {
            // edge case, we wanna make sure the player isn
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
