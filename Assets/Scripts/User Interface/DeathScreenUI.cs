using System.Collections;
using System.Collections.Generic;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeathScreenUI : MonoBehaviour
{
    public int attemptsRemaining;
    
    public Button respawnButton;
    public TextMeshProUGUI gameLossText;
    public Button gameOverButton;
    
    public void Login()
    {
        attemptsRemaining -= 1;
        if (attemptsRemaining <= 0)
        {
            SetGameOverScreen();
        }
        DungeonGeneratorManager.Instance.LoadIntoDungeon();
        PlayerManager.Instance.PlayerAlive();
        gameObject.SetActive(false);
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
