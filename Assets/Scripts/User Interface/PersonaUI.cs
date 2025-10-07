using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PersonaUI : MonoBehaviour
{
    // Reference to the template GameObject
    [SerializeField] private GameObject personaTemplate;
    [SerializeField] private Transform contentParent; 

    public void Start()
    {
        // find the button named Button_CloseMenu and add a listener to it
        Button closeButton = transform.Find("Button_CloseMenu")?.GetComponent<Button>();
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(ClosePersonaUI);
        }
    }
    
    public void OpenPersonaUI()
    {
        GenerateContent();
        gameObject.SetActive(true);
    }
    public void ClosePersonaUI()
    {
        gameObject.SetActive(false);
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
    
    private void GenerateContent()
    {
        // get currently active persona
        var activePersona = PersonaManager.Instance.GetPersona();
        var personas = PersonaManager.Instance.GetAllPersonas();

        // Create the same number of UI elements as there are personas
        foreach (var persona in personas)
        {
            var state = persona.Value;
            if (state == Types.PersonaState.Lost)
                continue; // skip non-visible states
            GameObject newPersona = Instantiate(personaTemplate, contentParent);

            // --- Fill in name ---
            TMP_Text nameText = newPersona.transform.Find("Text_PersonaName")?.GetComponent<TMP_Text>();
            if (nameText != null)
                nameText.text = persona.Key.ToString();

            // --- Fill in stats ---
            var stats = PersonaStatsLoader.GetStats(persona.Key);
            TMP_Text statsText = newPersona.transform.Find("Text_PersonaStats")?.GetComponent<TMP_Text>();
            if (statsText != null)
            {
                statsText.text = $"Health: {stats.MaxHealth}  Speed: {stats.MovementSpeed}  " +
                                 $"Attack: {stats.AttackDamage}  DashDamage: {stats.DashDamage}";
            }

            // --- Button logic ---
            Button button = newPersona.GetComponentInChildren<Button>();
            if (button != null)
            {
                var capturedPersona = persona.Key;

                if (state == Types.PersonaState.Selected)
                {
                    button.interactable = false;
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
