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
        //_personas = PersonaManager.Instance.GeneratePersonaStatesDict();
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
    
    private void GenerateContent(int numberOfPersonas = 4)
    {
        // get currently active persona
        var activePersona = PersonaManager.Instance.GetPersona();

        _personas = PersonaManager.Instance.GetAllPersonas();
        
        // Create the same number of UI elements as there are personas
        foreach (var persona in _personas)
        {

            var state = persona.Value;
            if (state == Types.PersonaState.Lost)
            {
                continue; 
            }
            // we should not be able to select the "Normal" persona
            if (persona.Key == Types.Persona.Normal || persona.Key == Types.Persona.None)
            {
                continue;
            }
                
            GameObject newPersona = Instantiate(personaTemplate, contentParent);

            // --- Fill in name ---
            TMP_Text nameText = newPersona.transform.Find("Text_PersonaName")?.GetComponent<TMP_Text>();
            if (nameText != null)
                nameText.text = "The " + persona.Key.ToString();

            // --- Fill in stats ---
            var stats = PersonaStatsLoader.GetStats(persona.Key);
            TMP_Text statsText = newPersona.transform.Find("Text_PersonaStats")?.GetComponent<TMP_Text>();
            if (statsText != null)
            {
                statsText.text = $"Health: {stats.MaxHealth}  Speed: {stats.MovementSpeed}  " +
                                 $"Attack: {stats.AttackDamage}  DashDamage: {stats.DashDamage}";
            }

            nameText.text += " | " + stats.Email;

            TMP_Text emailText = newPersona.transform.Find("Text_PersonaEmail")?.GetComponent<TMP_Text>();
            if (emailText != null)
            {
                emailText.text = stats.Email;
            }

            PersonaItemUI pItemUI = newPersona.GetComponentInChildren<PersonaItemUI>();
            pItemUI.SetColor(stats.PlayerColor);
            // --- Fill in description ---

            /*TMP_Text descriptionText = newPersona.transform.Find("Text_PersonaDescription")?.GetComponent<TMP_Text>();
            if (descriptionText != null)
            {
                descriptionText.text = stats.Description;
            }*/

            // --- Button logic ---
            Button button = newPersona.GetComponentInChildren<Button>();
            if (button != null)
            {
                var capturedPersona = persona.Key;

                if (state == Types.PersonaState.Selected)
                {
                    button.interactable = false;
                    pItemUI.ShowCheckmark();
                    buddeeUI.GetComponent<BUDDEEUI>().StopCR();
                    buddeeUI.GetComponent<BUDDEEUI>().SetDialogue(stats.Description);                                        
                    TMP_Text btnLabel = button.GetComponentInChildren<TMP_Text>();
                    if (btnLabel != null)
                        btnLabel.text = "Selected";
                }
                else if (state == Types.PersonaState.Available)
                {
                    button.interactable = true;
                    //pItemUI.HideCheckmark();
                    button.onClick.AddListener(() =>
                    {
                        PersonaManager.Instance.SetPersona(capturedPersona);
                        AudioManager.Instance.PlayUISelectSound();
                        UpdatePersonaUI();
                    });
                }
                else if(state == Types.PersonaState.Locked)
                {
                    button.interactable = false;
                    TMP_Text btnLabel = button.GetComponentInChildren<TMP_Text>();
                    if (btnLabel != null)
                        btnLabel.text = "Locked";
                }
                else
                {
                    // locked or lost, disable
                    button.interactable = false;
                }
            }
        }
    }
}
