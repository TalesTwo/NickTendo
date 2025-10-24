using System;
using System.Collections;
using Managers;
using UnityEngine;

public class CreditsManager : Singleton<CreditsManager>
{
    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private GameObject canvas;
    [SerializeField] private float scrollSpeed = 50f;

    private RectTransform _rectTransform;
    private bool _isScrolling = false;
    private Vector2 _startPosition;

    void Start()
    {
        canvas.SetActive(false);
        _rectTransform = creditsPanel.GetComponent<RectTransform>();
        _startPosition = _rectTransform.anchoredPosition;
    }

    public void BeginCredits()
    {

        // prepare credits
        canvas.SetActive(true);
        _rectTransform.anchoredPosition = _startPosition;

        // disable player controls
        //EventBroadcaster.Broadcast_StartStopAction();

        // play title screen music
        AudioManager.Instance.PlayTitleTrack(1f, true, 0.1f, true, 0.1f);

        // start scrolling
        _isScrolling = true;
        // Disable player actions
        PlayerManager.Instance.DeactivatePlayer();
        // restart the game after a delay
        StartCoroutine(RestartGameAfterDelay(5f));
    }

    private IEnumerator RestartGameAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        ResetCredits();

        // Restart game broadcast
        EventBroadcaster.Broadcast_GameRestart();
        EventBroadcaster.Broadcast_StartStopAction();
        PlayerManager.Instance.ActivatePlayer();
        PlayerManager.Instance.PlayerAlive();
    }

    private void ResetCredits()
    {
        _isScrolling = false;
        canvas.SetActive(false);
        _rectTransform.anchoredPosition = _startPosition;
    }

    void Update()
    {
        if (_isScrolling)
        {
            _rectTransform.anchoredPosition += new Vector2(0, scrollSpeed * Time.deltaTime);
        }
    }
}