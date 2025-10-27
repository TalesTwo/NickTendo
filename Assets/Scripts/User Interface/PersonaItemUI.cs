using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PersonaItemUI : MonoBehaviour
{
    [SerializeField]
    private GameObject _checkmark;
    [SerializeField]
    private Image _personaBackground;

    public void ShowCheckmark()
    {
        _checkmark.SetActive(true);
    }

    public void SetColor(Color _color)
    {
        _personaBackground.color = _color;
    }
}
