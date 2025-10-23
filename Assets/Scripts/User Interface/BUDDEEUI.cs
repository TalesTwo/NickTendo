using Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class BUDDEEUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _buddeeDialogue;
    [SerializeField]
    private float _wordSpeed;

    private bool _isCRRunning;

    // Start is called before the first frame update
    void Start()
    {
        _isCRRunning = false;
    }

    public void SetDialogue(string _dialogue)
    {
        if (gameObject.activeInHierarchy)
        {
            IEnumerator _dialogueCR = DialogueCoroutine(_dialogue);
            _buddeeDialogue.text = "";
            StartCoroutine(_dialogueCR);
        }              
    }

    IEnumerator DialogueCoroutine(string _dialogue)
    {
        _isCRRunning = true;
        int _talkingToneTimer = 0;
        foreach (char _letter in _dialogue)
        {
            if (!_isCRRunning)
            {
                DebugUtils.Log("This method is called");
                yield return null;
            }
            _buddeeDialogue.text += _letter;
            _talkingToneTimer++;
            if (_talkingToneTimer == 10)
            {
                _talkingToneTimer = 0;
                AudioManager.Instance.PlayPlayerTalkingTone();
            }
            yield return new WaitForSeconds(_wordSpeed);
        }
        _isCRRunning = false;
    }

    public void StopCR()
    {
        _isCRRunning = false;
        StopAllCoroutines();
    }
}
