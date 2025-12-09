using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PersonaItemUI : MonoBehaviour
{
    [SerializeField]
    private GameObject _checkmark;
    [SerializeField]
    private Image _personaBackground;
    [SerializeField]
    private TextMeshProUGUI _description;

    public bool isSelected;

    public void ShowCheckmark()
    {
        _checkmark.SetActive(true);
    }

    public void SetColor(Color _color)
    {
        _personaBackground.color = _color;
    }

    public void EnterMouse()
    {
        EventBroadcaster.Broadcast_PersonaItemStartHover(_description.text.ToString());
    }
    public void ExitMouse()
    {
        EventBroadcaster.Broadcast_PersonaItemEndHover();
    }
}
