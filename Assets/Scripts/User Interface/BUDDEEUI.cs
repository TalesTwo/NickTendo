using Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class BUDDEEUI : MonoBehaviour
{
    [Header("Dialogue Info")]
    [SerializeField] private TextMeshProUGUI _buddeeDialogue;
    [SerializeField] private float _wordSpeed;
    [SerializeField] private GameObject _BUDDEE;

    [Header("BUDDEE Emotes")]
    [SerializeField] private Sprite _angry;
    [SerializeField] private Sprite _basic1;
    [SerializeField] private Sprite _basic2;
    [SerializeField] private Sprite _basic3;
    [SerializeField] private Sprite _bothered;
    [SerializeField] private Sprite _confused;
    [SerializeField] private Sprite _crestfallen;
    [SerializeField] private Sprite _gremlin;
    [SerializeField] private Sprite _pissed;
    [SerializeField] private Sprite _pity;
    [SerializeField] private Sprite _sheepish;
    [SerializeField] private Sprite _smug;
    [SerializeField] private Sprite _sniffle;
    [SerializeField] private Sprite _speechless;
    [SerializeField] private Sprite _threatening;
    [SerializeField] private Sprite _worried;

    private bool _isCRRunning;
    private Dictionary<BUDDEEEmotes, Sprite> _BuddeEmoteDict;

    private void Awake()
    {
        _BuddeEmoteDict = new Dictionary<BUDDEEEmotes, Sprite>();
        SetEmoteDictionary();
    }
    // Start is called before the first frame update
    void Start()
    {
        _isCRRunning = false;
    }

    private void SetEmoteDictionary()
    {
        _BuddeEmoteDict[BUDDEEEmotes.angry] = _angry;
        _BuddeEmoteDict[BUDDEEEmotes.basic1] = _basic1;
        _BuddeEmoteDict[BUDDEEEmotes.basic2] = _basic2;
        _BuddeEmoteDict[BUDDEEEmotes.basic3] = _basic3;
        _BuddeEmoteDict[BUDDEEEmotes.bothered] = _bothered;
        _BuddeEmoteDict[BUDDEEEmotes.confused] = _confused;
        _BuddeEmoteDict[BUDDEEEmotes.crestfallen] = _crestfallen;
        _BuddeEmoteDict[BUDDEEEmotes.gremlin] = _gremlin;
        _BuddeEmoteDict[BUDDEEEmotes.pissed] = _pissed;
        _BuddeEmoteDict[BUDDEEEmotes.pity] = _pity;
        _BuddeEmoteDict[BUDDEEEmotes.sheepish] = _sheepish;
        _BuddeEmoteDict[BUDDEEEmotes.smug] = _smug;
        _BuddeEmoteDict[BUDDEEEmotes.sniffle] = _sniffle;
        _BuddeEmoteDict[BUDDEEEmotes.speechless] = _speechless;
        _BuddeEmoteDict[BUDDEEEmotes.threatening] = _threatening;
        _BuddeEmoteDict[BUDDEEEmotes.worried] = _worried;
    }

    private void ChangeBUDDEEEmote(BUDDEEEmotes _emote)
    {
        _BUDDEE.GetComponent<Image>().sprite = _BuddeEmoteDict[_emote];
        _BUDDEE.GetComponent<RectTransform>().sizeDelta = new Vector2 (_BuddeEmoteDict[_emote].rect.width, _BuddeEmoteDict[_emote].rect.height);
    }

    public void SetDialogue(string _dialogue, BUDDEEEmotes _emote = BUDDEEEmotes.basic2)
    {
        if (gameObject.activeInHierarchy)
        {
            IEnumerator _dialogueCR = DialogueCoroutine(_dialogue);
            _buddeeDialogue.text = "";
            ChangeBUDDEEEmote(_emote);
            StartCoroutine(_dialogueCR);
        }              
    }

    IEnumerator DialogueCoroutine(string _dialogue)
    {
        _isCRRunning = true;
        int _talkingToneTimer = 0;
        _buddeeDialogue.maxVisibleCharacters = 0;
        _buddeeDialogue.text = _dialogue;
        for (int i = 0; i < _dialogue.Length; i++)
        {
            if (!_isCRRunning)
            {
                yield return null;
            }
            _buddeeDialogue.maxVisibleCharacters++;
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
        ChangeBUDDEEEmote(BUDDEEEmotes.basic2);
        StopAllCoroutines();
    }
}
