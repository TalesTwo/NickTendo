using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PersonaItemUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private GameObject _checkmark;
    [SerializeField]
    private Image _personaBackground;

    private TMP_Text _description;

    public bool isSelected;

    private void Start()
    {
        _description = transform.Find("Text_PersonaDescription")?.GetComponent<TMP_Text>();
    }

    public void ShowCheckmark()
    {
        _checkmark.SetActive(true);
    }

    public void SetColor(Color _color)
    {
        _personaBackground.color = _color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        EventBroadcaster.Broadcast_PersonaItemStartHover(_description.text.ToString());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        EventBroadcaster.Broadcast_PersonaItemEndHover();
    }
}