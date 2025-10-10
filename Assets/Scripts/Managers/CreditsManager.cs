using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;

public class CreditsManager : Singleton<CreditsManager>
{
    
    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private GameObject canvas;
    [SerializeField] private float scrollSpeed = 50f;
    
    private RectTransform _rectTransform;
    
    private bool _isScrolling = false;
    
    // Start is called before the first frame update
    void Start()
    {
        canvas.SetActive(false);
    }

    // Update is called once per frame
    public void BeginCredits()
    {
        // reset position
        canvas.SetActive(true);
        _rectTransform  = creditsPanel.GetComponent<RectTransform>();
        // disable the player controls
        var playerController = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.enabled = false;
        }
        // swap to the title screen music
        AudioManager.Instance.PlayTitleTrack(1f, true, 0.1f, true, 0.1f);
        _isScrolling = true;
    }
    void Update()
    {
        if (_isScrolling)
        {
            canvas.SetActive(true);
            _rectTransform.anchoredPosition += new Vector2(0, scrollSpeed * Time.deltaTime);
        }
        
    }
}
