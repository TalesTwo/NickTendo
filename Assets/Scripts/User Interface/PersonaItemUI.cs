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
    private void Start()
    {
    }
    public void ShowCheckmark()
    {
        _checkmark.SetActive(true);
    }
}
