using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChipCountUI : MonoBehaviour
{
    private TextMeshProUGUI chipCount;
    // Start is called before the first frame update
    void Start()
    {
        chipCount = gameObject.GetComponent<TextMeshProUGUI>();
        EventBroadcaster.PlayerStatsChanged += OnPickedChips;
        EventBroadcaster.PersonaChanged += HandlePersonaChanged;
        SetChips();
    }
    private void SetChips()
    {
        chipCount.text = PlayerStats.Instance.GetChips().ToString();
    }

    void OnPickedChips(PlayerStatsEnum BufFType, float BuffValue)
    {
        if (BufFType == PlayerStatsEnum.Chips)
        {
            SetChips();
        }
    }

    void HandlePersonaChanged(Types.Persona P)
    {
        SetChips();
    }
}
