using Managers;
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

    // Start is called before the first frame update
    void Start()
    {
        SetDialogue("Click on one of the icons to learn more!!!");
    }

    public void SetDialogue(string _dialogue)
    {
        _buddeeDialogue.text = "";
        StartCoroutine(DialogueCoroutine(_dialogue));
        
    }

    IEnumerator DialogueCoroutine(string _dialogue)
    {
        int _talkingToneTimer = 0;
        foreach (char _letter in _dialogue)
        {
            _buddeeDialogue.text += _letter;
            _talkingToneTimer++;
            if (_talkingToneTimer == 10)
            {
                _talkingToneTimer = 0;
                AudioManager.Instance.PlayPlayerTalkingTone();
            }
            yield return new WaitForSeconds(_wordSpeed);
        }
    }
}
