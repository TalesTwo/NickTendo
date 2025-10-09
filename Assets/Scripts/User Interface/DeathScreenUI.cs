using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeathScreenUI : MonoBehaviour
{
    private int attemptsRemaining;
    
    public Button respawnButton;
    public TextMeshProUGUI gameLossText;
    public Button gameOverButton;

    private void OnEnable()
    {
        /*
        attemptsRemaining = PersonaManager.Instance.GetNumberOfAvailablePersonas();
        if (attemptsRemaining <= 0)
        {
            SetGameOverScreen();
        }
        */
        
        // failsafe to ensure that values are updated correctly,
        // we can just hookup to when the persona changes, cause it always does at death
        EventBroadcaster.PersonaChanged += OnPersonaChanged;
    }
    
    private void OnPersonaChanged(Types.Persona newPersona)
    {
        attemptsRemaining = PersonaManager.Instance.GetNumberOfAvailablePersonas();
        if (attemptsRemaining <= 0)
        {
            SetGameOverScreen();
        }
    }
    public void Login()
    {
        DungeonGeneratorManager.Instance.LoadIntoDungeon();
        PlayerManager.Instance.PlayerAlive();
        gameObject.SetActive(false);
        // unhook from the event
        EventBroadcaster.PersonaChanged -= OnPersonaChanged;
    }

    private void SetGameOverScreen()
    {
        respawnButton.gameObject.SetActive(false);
        gameLossText.text = "Game Over";
        gameOverButton.gameObject.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    
}
