using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UI;

public class PersonaUI : MonoBehaviour
{
    // Reference to the template GameObject
    [SerializeField] private GameObject personaTemplate;
    [SerializeField] private Transform contentParent; 
    [SerializeField] private GameObject buddeeUI;
    
    
    private Dictionary<Types.Persona, Types.PersonaState> _personas;

    public void Start()
    {
        // find the button named Button_CloseMenu and add a listener to it
        Button closeButton = transform.Find("Button_CloseMenu")?.GetComponent<Button>();
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(ClosePersonaUI);
        }
        
        // read the persona states from the PersonaManager
    }

    public void OpenPersonaUI()
    {
        GenerateContent();
        gameObject.SetActive(true);
        if (PersonaManager.Instance.GetNumberOfAvailablePersonas() == 1)
        {
            buddeeUI.GetComponent<BUDDEEUI>().SetDialogue("Looks like we ran out of all available accounts...");
        }
        else
        {
            buddeeUI.GetComponent<BUDDEEUI>().SetDialogue("Click on one of the accounts to learn more!!!");
        }

        // disable player movement
        //TODO: Probably make
        //this a better system later
        var playerController = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.enabled = false;
        }
        EventBroadcaster.Broadcast_OpenPersonaUI();
        EventBroadcaster.Broadcast_PlayerOpenMenu();
    }
    public void ClosePersonaUI()
    {
        AudioManager.Instance.PlayUISelectSound();
        gameObject.SetActive(false);
        // cleanup
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }
        var playerController = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.enabled = true;
        }
        // tell the SelectionInteraction that the UI is closed
        EventBroadcaster.Broadcast_ClosePersonaUI();
        EventBroadcaster.Broadcast_PlayerCloseMenu();

    }

    public void UpdatePersonaUI()
    {
        // For now, we will just destroy all of the children and recreate them
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        GenerateContent();

    }
    
    private void GenerateContent(int numberOfPersonas = 3)
{
    // Ensure personas are initialized & generated once
    PersonaManager.Instance.InitializeRandomPersonas();

    // Get persona states dictionary (Available, Locked, Selected, etc.)
    _personas = PersonaManager.Instance.GetAllPersonas();
    
    // "trim" the _personas dictionary to only only include a random selection of personas
    var trimmedPersonas = PersonaManager.Instance.GetTrimmedPersonas(numberOfPersonas);
    

    // --- Generate UI elements ---
    foreach (var kvp in trimmedPersonas)
    {
        var personaType = kvp.Key;
        var state = kvp.Value;

        // skip unusable personas
        if (personaType == Types.Persona.Normal || personaType == Types.Persona.None)
            continue;

        if (state == Types.PersonaState.Lost)
            continue;

        // get generated stats for this persona type
        var stats = PersonaManager.Instance.GetGeneratedPersona(personaType);

        // create UI element
        GameObject newPersona = Instantiate(personaTemplate, contentParent);

        // --- Fill in name ---
        TMP_Text nameText = newPersona.transform.Find("Text_PersonaName")?.GetComponent<TMP_Text>();
        if (nameText != null)
            nameText.text = $"{stats.Username}, The {personaType} | ";

        // --- Fill in stats ---
        TMP_Text statsText = newPersona.transform.Find("Text_PersonaStats")?.GetComponent<TMP_Text>();
        if (statsText != null)
        {
            // round to 2 decimal places
            statsText.text = $"Health: {stats.MaxHealth:F1}  Speed: {stats.MovementSpeed:F1}  " +
                             $"Attack: {stats.AttackDamage:F1}  DashDamage: {stats.DashDamage:F1}";

        }

        // --- Fill in email ---
        TMP_Text emailText = newPersona.transform.Find("Text_PersonaEmail")?.GetComponent<TMP_Text>();
        if (emailText != null)
            emailText.text = stats.Email;

        // --- Set color ---
        PersonaItemUI pItemUI = newPersona.GetComponentInChildren<PersonaItemUI>();
        if (pItemUI != null)
            pItemUI.SetColor(stats.PlayerColor);

        // --- Button logic ---
        Button button = newPersona.GetComponentInChildren<Button>();
        if (button == null) continue;

        var capturedPersona = personaType;

        if (state == Types.PersonaState.Selected)
        {
            button.interactable = false;
            if (pItemUI != null) pItemUI.ShowCheckmark();

            var buddee = buddeeUI.GetComponent<BUDDEEUI>();
            buddee.StopCR();
            buddee.SetDialogue(stats.Description);

            TMP_Text btnLabel = button.GetComponentInChildren<TMP_Text>();
            if (btnLabel != null)
                btnLabel.text = "Selected";
        }
        else if (state == Types.PersonaState.Available)
        {
            button.interactable = true;
            button.onClick.AddListener(() =>
            {
                PersonaManager.Instance.SetPersona(capturedPersona);
                AudioManager.Instance.PlayUISelectSound();
                UpdatePersonaUI();
            });
        }
        else if (state == Types.PersonaState.Locked)
        {
            button.interactable = false;
            TMP_Text btnLabel = button.GetComponentInChildren<TMP_Text>();
            if (btnLabel != null)
                btnLabel.text = "Locked";
        }
        else
        {
            button.interactable = false;
        }
    }
}

}
